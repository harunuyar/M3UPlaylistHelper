namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Epg;
using M3UPlaylistHelper.Model;
using M3UPlaylistHelper.Parser;
using M3UPlaylistHelper.Tools;
using M3UPlaylistHelper.Xtream;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private const string AppName = "M3U Playlist Helper";
    private const string PlaylistFileFilter = "M3U Playlist (*.m3u;*.m3u8)|*.m3u;*.m3u8|All Files (*.*)|*.*";
    private const string ProfileFileFilter = "Selection Profile (*.json)|*.json";
    private const string EpgFileFilter = "XMLTV Guide (*.xml;*.xml.gz;*.gz)|*.xml;*.xml.gz;*.gz|All Files (*.*)|*.*";

    private readonly AppSettings settings = AppSettings.Load();
    private readonly string? startupItem;

    // Debounces typing in the filter boxes, so large playlists are not re-filtered on every key press
    private readonly System.Windows.Forms.Timer filterTimer = new() { Interval = 250 };

    // Debounces scrolling, logos are only downloaded for the rows on screen
    private readonly System.Windows.Forms.Timer logoTimer = new() { Interval = 200 };

    // Names of the playlists that make up the current one (more than one after merging)
    private readonly List<string> sourceNames = [];

    private Playlist playlist = new();
    private Category? selectedCategory;
    private string? currentFilePath;
    private bool isDirty;
    private bool isLoading;
    private bool suppressCategorySelectionChanged;
    private CancellationTokenSource? logoCancellation;
    // What the grids show; they run in virtual mode and read rows from these lists on demand
    private List<Category> visibleCategories = [];
    private List<Channel> visibleChannels = [];
    private bool suppressSpaceKeyUp;
    private bool categoryFilterChanged;
    private bool channelFilterChanged;

    // The Xtream account the current playlist came from, for its EPG and account status
    private XtreamAccount? xtreamAccount;
    private XmlTvGuide? epg;

    // Where a dragged row would be dropped: an insertion line before a row, or a highlighted row
    private DataGridView? dropGrid;
    private int dropRow = -1;
    private bool dropOnRow;

    public MainForm(string? startupItem = null)
    {
        InitializeComponent();

        this.startupItem = startupItem;

        dataGridViewCategories.AutoGenerateColumns = false;
        dataGridViewChannels.AutoGenerateColumns = false;
        dataGridViewChannels.RowTemplate.Height = Math.Max(dataGridViewChannels.RowTemplate.Height, LogoCache.ThumbnailHeight + 8);
        dataGridViewChannels.DragDataProvider = () => SelectedChannels is { Count: > 0 } channels ? new ChannelDragData(channels) : null;
        dataGridViewCategories.DragDataProvider = () => CurrentCategory is Category category ? new CategoryDragData(category) : null;

        // Show an empty cell instead of the "missing image" icon when a channel has no logo
        columnChannelLogo.DefaultCellStyle.NullValue = null;
        columnChannelEpg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        checkBoxDownloadLogos.Checked = settings.DownloadLogos;
        columnChannelLogo.Visible = settings.DownloadLogos;

        filterTimer.Tick += FilterTimer_Tick;
        logoTimer.Tick += LogoTimer_Tick;
        SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

        EnableDragAndDrop(this);
        RestoreWindowLayout();
        UpdateRecentMenu();
        ApplyTheme();
        RefreshAll();
    }

    private sealed record ChannelDragData(List<Channel> Channels);

    private sealed record CategoryDragData(Category Category);

    private sealed record DropTarget(DataGridView Grid, int Row, bool OnRow);

    /// <summary>
    /// Where a playlist is loaded from. Xtream sources carry the account so the password never shows up in the UI.
    /// </summary>
    private sealed record PlaylistSource(string Location, string DisplayName, bool IsUrl, XtreamAccount? Xtream = null)
    {
        public static PlaylistSource FromPathOrUrl(string item) => new(item, item, AppSettings.IsUrl(item));

        public static PlaylistSource FromXtream(XtreamAccount account) => new(account.PlaylistUrl, account.DisplayName, true, account);
    }

    #region Form lifecycle

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (settings.SplitterDistance is int distance && distance > splitContainer.Panel1MinSize && distance < splitContainer.Width - splitContainer.Panel2MinSize)
        {
            splitContainer.SplitterDistance = distance;
        }
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (!string.IsNullOrWhiteSpace(startupItem))
        {
            await OpenAsync(PlaylistSource.FromPathOrUrl(startupItem), append: false);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (!e.Cancel && !ConfirmDiscardChanges())
        {
            e.Cancel = true;
            return;
        }

        logoCancellation?.Cancel();
        SaveWindowLayout();
        settings.Save();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // Static event, it would keep the form alive
        SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        base.OnFormClosed(e);
    }

    private void RestoreWindowLayout()
    {
        if (settings.WindowX is int x && settings.WindowY is int y && settings.WindowWidth is int width && settings.WindowHeight is int height)
        {
            var bounds = new Rectangle(x, y, width, height);

            // Only restore the position if the window would still be visible (e.g. a monitor was disconnected)
            if (Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(bounds)))
            {
                StartPosition = FormStartPosition.Manual;
                Bounds = bounds;
            }
        }

        if (settings.WindowMaximized)
        {
            WindowState = FormWindowState.Maximized;
        }
    }

    private void SaveWindowLayout()
    {
        var bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;
        settings.WindowX = bounds.X;
        settings.WindowY = bounds.Y;
        settings.WindowWidth = bounds.Width;
        settings.WindowHeight = bounds.Height;
        settings.WindowMaximized = WindowState == FormWindowState.Maximized;
        settings.SplitterDistance = splitContainer.SplitterDistance;
    }

    #endregion

    #region Theme

    private void ApplyTheme()
    {
        var theme = ThemeManager.Parse(settings.Theme);
        ThemeManager.SetTheme(theme);
        ThemeManager.Apply(this);

        themeSystemToolStripMenuItem.Checked = theme == AppTheme.System;
        themeLightToolStripMenuItem.Checked = theme == AppTheme.Light;
        themeDarkToolStripMenuItem.Checked = theme == AppTheme.Dark;

        dataGridViewCategories.Invalidate();
        dataGridViewChannels.Invalidate();
    }

    private void ThemeToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        var theme = sender == themeDarkToolStripMenuItem ? AppTheme.Dark : sender == themeLightToolStripMenuItem ? AppTheme.Light : AppTheme.System;
        settings.Theme = theme.ToString();
        settings.Save();
        ApplyTheme();
    }

    private void SystemEvents_UserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e)
    {
        // Follow Windows switching between light and dark mode
        if (e.Category == UserPreferenceCategory.General && ThemeManager.Parse(settings.Theme) == AppTheme.System)
        {
            BeginInvokeIfAlive(ApplyTheme);
        }
    }

    #endregion

    #region Opening and saving

    /// <returns>true if the playlist was loaded.</returns>
    private async Task<bool> OpenAsync(PlaylistSource source, bool append)
    {
        if (isLoading)
        {
            return false;
        }

        append &= playlist.Categories.Count > 0;

        if (!append && !ConfirmDiscardChanges())
        {
            return false;
        }

        bool success = false;

        string? resultMessage = null;
        SetLoading(true, source.IsUrl ? $"Downloading {source.DisplayName}..." : $"Reading {source.DisplayName}...");

        try
        {
            var loaded = source.IsUrl
                ? await Task.Run(() => M3UParser.ParseUrlAsync(source.Location, CancellationToken.None))
                : await Task.Run(() => M3UParser.ParseFileAsync(source.Location, CancellationToken.None));

            if (append)
            {
                CommitGridEdits();
                int added = PlaylistTools.Merge(playlist, loaded);
                sourceNames.Add(source.DisplayName);

                // The playlist is now a mix, Save must not silently overwrite one of the originals
                currentFilePath = null;
                isDirty = true;
                resultMessage = $"Added {added:N0} channels from {source.DisplayName}. Tip: Edit > Exclude Duplicate Channels hides channels that were in both.";
            }
            else
            {
                playlist = loaded;
                selectedCategory = null;
                currentFilePath = source.IsUrl ? null : source.Location;
                sourceNames.Clear();
                sourceNames.Add(source.DisplayName);
                isDirty = false;
                epg = null;
                xtreamAccount = null;
                toolStripAccountLabel.Visible = false;

                textBoxCategoryFilter.Clear();
                textBoxChannelFilter.Clear();
                filterTimer.Stop();
                categoryFilterChanged = channelFilterChanged = false;
                checkBoxSearchAllCategories.Checked = false;
            }

            success = true;

            if (source.Xtream != null)
            {
                xtreamAccount = source.Xtream;
                _ = ShowAccountInfoAsync(source.Xtream);
            }
            else
            {
                settings.AddRecentItem(source.Location);
                settings.Save();
                UpdateRecentMenu();
            }
        }
        catch (Exception ex)
        {
            if (!source.IsUrl && (ex is FileNotFoundException || ex is DirectoryNotFoundException))
            {
                settings.RecentItems.Remove(source.Location);
                settings.Save();
                UpdateRecentMenu();
            }

            MessageBox.Show(this, $"Could not open {source.DisplayName}:\n\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetLoading(false);
            RefreshAll();

            if (resultMessage != null)
            {
                toolStripStatusLabel.Text = resultMessage;
            }
        }

        return success;
    }

    private async Task OpenAllAsync(IReadOnlyList<PlaylistSource> sources, bool append)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            // The first one replaces the current playlist (unless adding), the rest are merged into it.
            // If that first one was cancelled or failed, don't merge the rest into the playlist the user meant to close.
            if (!await OpenAsync(sources[i], append || i > 0) && i == 0 && !append)
            {
                return;
            }
        }
    }

    private async Task ShowAccountInfoAsync(XtreamAccount account)
    {
        try
        {
            var info = await Task.Run(() => account.GetAccountInfoAsync(CancellationToken.None));

            if (account != xtreamAccount)
            {
                return;
            }

            var parts = new List<string>();

            if (!info.IsAuthenticated)
            {
                parts.Add("Login not accepted");
            }
            else
            {
                parts.Add(info.Status ?? "Active");
                parts.Add(info.ExpiresAt is DateTime expires ? $"expires {expires:d}" : "no expiry date");

                if (info.MaxConnections is int max)
                {
                    parts.Add($"{info.ActiveConnections ?? 0}/{max} connections");
                }

                if (info.IsTrial)
                {
                    parts.Add("trial");
                }
            }

            toolStripAccountLabel.Text = $"{account.Username}: {string.Join(", ", parts)}";
            toolStripAccountLabel.Visible = true;
        }
        catch (Exception ex)
        {
            // The account status is a nice extra, the playlist itself already loaded
            Console.Error.WriteLine($"Failed to get Xtream account info: {ex.Message}");
        }
    }

    /// <returns>false if the save was canceled or failed.</returns>
    private bool Save()
    {
        return currentFilePath == null ? SaveAs() : SaveTo(currentFilePath);
    }

    /// <returns>false if the save was canceled or failed.</returns>
    private bool SaveAs()
    {
        using var saveFileDialog = new SaveFileDialog
        {
            Filter = PlaylistFileFilter,
            Title = "Save M3U Playlist",
            FileName = currentFilePath != null ? Path.GetFileName(currentFilePath) : "playlist.m3u",
            InitialDirectory = currentFilePath != null ? Path.GetDirectoryName(currentFilePath) : null,
        };

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return false;
        }

        return SaveTo(saveFileDialog.FileName);
    }

    private bool SaveTo(string path)
    {
        CommitGridEdits();

        if (!playlist.ExportedChannels.Any() &&
            MessageBox.Show(this, "No channels are selected, the saved playlist will be empty. Save anyway?", AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return false;
        }

        try
        {
            // Big playlists take a moment to write
            Cursor.Current = Cursors.WaitCursor;
            M3UWriter.WriteFile(playlist, path);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not save the playlist:\n\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally
        {
            Cursor.Current = Cursors.Default;
        }

        currentFilePath = path;
        sourceNames.Clear();
        sourceNames.Add(path);
        isDirty = false;
        settings.AddRecentItem(path);
        settings.Save();
        UpdateRecentMenu();
        UpdateTitle();
        toolStripStatusLabel.Text = $"Saved {playlist.ExportedChannels.Count():N0} channels to {path}";
        return true;
    }

    /// <summary>
    /// Asks the user what to do with unsaved changes.
    /// </summary>
    /// <returns>true if it is OK to continue (changes saved or discarded).</returns>
    private bool ConfirmDiscardChanges()
    {
        CommitGridEdits();

        if (!isDirty)
        {
            return true;
        }

        var result = MessageBox.Show(this, "Do you want to save the changes to the current playlist?", AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

        return result switch
        {
            DialogResult.Yes => Save(),
            DialogResult.No => true,
            _ => false,
        };
    }

    private void SetLoading(bool loading, string? message = null)
    {
        isLoading = loading;
        UseWaitCursor = loading;
        menuStrip.Enabled = !loading;
        mainPanel.Enabled = !loading;
        toolStripProgressBar.Visible = loading;

        if (message != null)
        {
            toolStripStatusLabel.Text = message;
        }
    }

    private void CommitGridEdits()
    {
        CommitGridEdit(dataGridViewCategories);
        CommitGridEdit(dataGridViewChannels);
    }

    private static void CommitGridEdit(DataGridView grid)
    {
        if (!grid.IsCurrentCellInEditMode)
        {
            return;
        }

        if (!grid.EndEdit())
        {
            grid.CancelEdit();
        }

        // Check box cells are normally always in edit mode while current; clicking them only works that way
        var column = grid.CurrentCellAddress.X;
        if (grid.ContainsFocus && column >= 0 && grid.Columns[column] is DataGridViewCheckBoxColumn)
        {
            grid.BeginEdit(false);
        }
    }

    private void MarkDirty()
    {
        if (!isDirty)
        {
            isDirty = true;
            UpdateTitle();
        }
    }

    #endregion

    #region Display

    private void RefreshAll()
    {
        if (!RefreshCategoryList())
        {
            RefreshChannelList();
        }

        UpdateTitle();
        UpdateStatus();
        UpdateMenuState();
    }

    private void UpdateTitle()
    {
        var dirtyMarker = isDirty ? "*" : string.Empty;
        var source = sourceNames.Count switch
        {
            0 => null,
            1 => sourceNames[0],
            _ => $"{sourceNames[0]} (+{sourceNames.Count - 1} more)",
        };

        Text = source == null ? AppName : $"{AppName} - {source}{dirtyMarker}";
    }

    private void UpdateStatus()
    {
        if (isLoading)
        {
            return;
        }

        if (playlist.Categories.Count == 0)
        {
            toolStripStatusLabel.Text = "Open a playlist file, URL or Xtream Codes account to get started. You can also drag and drop files or links onto this window.";
            return;
        }

        // One pass over everything, this runs after every check box click
        int includedCategories = 0, channels = 0, exported = 0, withGuide = 0;

        foreach (var category in playlist.Categories)
        {
            if (category.IsIncluded)
            {
                includedCategories++;
            }

            foreach (var channel in category.Channels)
            {
                channels++;

                if (category.IsIncluded && channel.IsIncluded)
                {
                    exported++;
                }

                if (epg != null && epg.HasGuide(channel))
                {
                    withGuide++;
                }
            }
        }

        var text =
            $"{playlist.Categories.Count:N0} categories ({includedCategories:N0} included)  |  " +
            $"{channels:N0} channels  |  {exported:N0} channels will be saved";

        if (epg != null)
        {
            text += $"  |  EPG: {withGuide:N0} channels have a guide";
        }

        toolStripStatusLabel.Text = text;
    }

    private void UpdateMenuState()
    {
        bool hasPlaylist = playlist.Categories.Count > 0;
        saveToolStripMenuItem.Enabled = hasPlaylist;
        saveAsToolStripMenuItem.Enabled = hasPlaylist;
        excludeDuplicatesToolStripMenuItem.Enabled = hasPlaylist;
        sortCategoriesToolStripMenuItem.Enabled = hasPlaylist;
        saveSelectionProfileToolStripMenuItem.Enabled = hasPlaylist;
        applySelectionProfileToolStripMenuItem.Enabled = hasPlaylist;
        epgToolStripMenuItem.Enabled = hasPlaylist;
    }

    private void UpdateRecentMenu()
    {
        recentToolStripMenuItem.DropDownItems.Clear();

        for (int i = 0; i < settings.RecentItems.Count; i++)
        {
            var item = settings.RecentItems[i];
            var menuItem = new ToolStripMenuItem($"&{(i + 1) % 10} {item.Replace("&", "&&")}") { Tag = item };
            menuItem.Click += async (_, _) => await OpenAsync(PlaylistSource.FromPathOrUrl(item), append: false);
            recentToolStripMenuItem.DropDownItems.Add(menuItem);
        }

        if (settings.RecentItems.Count > 0)
        {
            recentToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var clearItem = new ToolStripMenuItem("&Clear Recent List");
            clearItem.Click += (_, _) =>
            {
                settings.RecentItems.Clear();
                settings.Save();
                UpdateRecentMenu();
            };
            recentToolStripMenuItem.DropDownItems.Add(clearItem);
        }

        recentToolStripMenuItem.Enabled = settings.RecentItems.Count > 0;
        ThemeManager.ApplyToolStrip(menuStrip);
    }

    /// <returns>true if the selected category changed and the channel list was refreshed too.</returns>
    private bool RefreshCategoryList()
    {
        // A pending edit must be written to the item it was made on, before the rows change meaning
        CommitGridEdit(dataGridViewCategories);

        var filter = textBoxCategoryFilter.Text.Trim();
        var visible = filter.Length == 0
            ? playlist.Categories.ToList()
            : playlist.Categories.Where(c => PlaylistTools.Matches(c.Title, filter)).ToList();

        suppressCategorySelectionChanged = true;

        try
        {
            visibleCategories = visible;
            dataGridViewCategories.ResetRows(visible.Count);

            var target = selectedCategory != null && visible.Contains(selectedCategory) ? selectedCategory : visible.FirstOrDefault();
            if (target != null)
            {
                dataGridViewCategories.CurrentCell = dataGridViewCategories[columnCategoryTitle.Index, visible.IndexOf(target)];
            }

            if (target != selectedCategory)
            {
                selectedCategory = target;
                RefreshChannelList();
                return true;
            }

            return false;
        }
        finally
        {
            suppressCategorySelectionChanged = false;
        }
    }

    private void RefreshChannelList()
    {
        CommitGridEdit(dataGridViewChannels);

        var filter = textBoxChannelFilter.Text.Trim();
        bool searchAll = checkBoxSearchAllCategories.Checked;

        IEnumerable<Channel> source = searchAll ? playlist.AllChannels : selectedCategory?.Channels ?? [];
        int total = source.Count();

        if (filter.Length > 0)
        {
            source = source.Where(c => PlaylistTools.Matches(c, filter));
        }

        var visible = source.ToList();
        visibleChannels = visible;
        dataGridViewChannels.ResetRows(visible.Count);
        dataGridViewChannels.Invalidate();
        columnChannelCategory.Visible = searchAll;
        columnChannelEpg.Visible = epg != null;

        var title = searchAll ? "All categories" : selectedCategory?.Title;
        labelSelectedCategory.Text = title == null
            ? string.Empty
            : filter.Length > 0 ? $"{title} ({visible.Count:N0} of {total:N0} channels match)" : $"{title} ({total:N0} channels)";

        LoadVisibleLogos();
    }

    private List<Category> VisibleCategories => visibleCategories;

    private List<Channel> VisibleChannels => visibleChannels;

    // CurrentCellAddress instead of CurrentCell/CurrentRow: those create a row object for every row the user visits
    private Category? CurrentCategory =>
        dataGridViewCategories.CurrentCellAddress.Y is int row && row >= 0 && row < visibleCategories.Count ? visibleCategories[row] : null;

    private Channel? CurrentChannel =>
        dataGridViewChannels.CurrentCellAddress.Y is int row && row >= 0 && row < visibleChannels.Count ? visibleChannels[row] : null;

    /// <summary>
    /// The selected channels, in the order they are shown.
    /// </summary>
    private List<Channel> SelectedChannels =>
        dataGridViewChannels.GetSelectedRowIndexes().Where(i => i < visibleChannels.Count).Select(i => visibleChannels[i]).ToList();

    private void OnIncludedStateChanged()
    {
        MarkDirty();
        dataGridViewCategories.Invalidate();
        dataGridViewChannels.Invalidate();
        UpdateStatus();
    }

    #endregion

    #region Logos

    private void LoadVisibleLogos()
    {
        logoCancellation?.Cancel();

        if (!checkBoxDownloadLogos.Checked)
        {
            return;
        }

        var channels = VisibleChannels;
        if (channels.Count == 0)
        {
            return;
        }

        int first = Math.Max(0, dataGridViewChannels.FirstDisplayedScrollingRowIndex);
        int count = dataGridViewChannels.DisplayedRowCount(true) + 10;
        var urls = channels.Skip(first).Take(count).Select(c => c.LogoUrl).ToList();

        logoCancellation = new CancellationTokenSource();
        _ = LogoCache.LoadAsync(urls, () => BeginInvokeIfAlive(() => dataGridViewChannels.InvalidateColumn(columnChannelLogo.Index)), logoCancellation.Token);
    }

    private void BeginInvokeIfAlive(Action action)
    {
        try
        {
            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke(action);
            }
        }
        catch (InvalidOperationException)
        {
            // The form was closed in the meantime
        }
    }

    private void LogoTimer_Tick(object? sender, EventArgs e)
    {
        logoTimer.Stop();
        LoadVisibleLogos();
    }

    private void DataGridViewChannels_Scroll(object? sender, ScrollEventArgs e)
    {
        logoTimer.Stop();
        logoTimer.Start();
    }

    private void DataGridViewChannels_Resize(object? sender, EventArgs e)
    {
        logoTimer.Stop();
        logoTimer.Start();
    }

    private void DataGridViewChannels_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= visibleChannels.Count)
        {
            return;
        }

        var channel = visibleChannels[e.RowIndex];

        if (e.ColumnIndex == columnChannelIncluded.Index)
        {
            e.Value = channel.IsIncluded;
        }
        else if (e.ColumnIndex == columnChannelLogo.Index)
        {
            e.Value = checkBoxDownloadLogos.Checked ? LogoCache.Get(channel.LogoUrl) : null;
        }
        else if (e.ColumnIndex == columnChannelName.Index)
        {
            e.Value = channel.Name;
        }
        else if (e.ColumnIndex == columnChannelEpg.Index)
        {
            e.Value = epg == null ? null : epg.HasGuide(channel) ? "\u2713" : "\u2717";
        }
        else if (e.ColumnIndex == columnChannelCategory.Index)
        {
            e.Value = channel.Category.Title;
        }
        else if (e.ColumnIndex == columnChannelUrl.Index)
        {
            e.Value = channel.Url;
        }
    }

    private void DataGridViewChannels_CellValuePushed(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= visibleChannels.Count)
        {
            return;
        }

        var channel = visibleChannels[e.RowIndex];

        if (e.ColumnIndex == columnChannelIncluded.Index)
        {
            channel.IsIncluded = IsChecked(e.Value);
            OnIncludedStateChanged();
        }
        else if (e.ColumnIndex == columnChannelName.Index)
        {
            var name = (e.Value as string)?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                toolStripStatusLabel.Text = "A channel name cannot be empty, the old name was kept.";
            }
            else if (name != channel.Name)
            {
                channel.Name = name;
                MarkDirty();
            }
        }
    }

    private static bool IsChecked(object? value) => value is true || value is CheckState.Checked;

    private void DataGridViewChannels_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= visibleChannels.Count || e.CellStyle == null)
        {
            return;
        }

        var channel = visibleChannels[e.RowIndex];

        if (e.ColumnIndex == columnChannelEpg.Index && epg != null)
        {
            e.CellStyle.ForeColor = epg.HasGuide(channel) ? ThemeManager.Current.Good : ThemeManager.Current.Bad;
        }

        // Grey out channels that will not be saved, either unchecked or in an unchecked category
        if (!(channel.IsIncluded && channel.Category.IsIncluded))
        {
            e.CellStyle.ForeColor = ThemeManager.Current.MutedText;
        }
    }

    private void CheckBoxDownloadLogos_CheckedChanged(object? sender, EventArgs e)
    {
        settings.DownloadLogos = checkBoxDownloadLogos.Checked;
        columnChannelLogo.Visible = checkBoxDownloadLogos.Checked;
        LoadVisibleLogos();
    }

    #endregion

    #region Grid events

    private void DataGridViewCategories_SelectionChanged(object? sender, EventArgs e)
    {
        if (suppressCategorySelectionChanged)
        {
            return;
        }

        if (CurrentCategory is Category category && category != selectedCategory)
        {
            selectedCategory = category;

            if (checkBoxSearchAllCategories.Checked)
            {
                // Picking a category means the user wants to look inside it
                checkBoxSearchAllCategories.Checked = false;
            }
            else
            {
                RefreshChannelList();
            }
        }
    }

    private void DataGridView_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // Check boxes normally only commit when the cell loses focus, which makes counts and the dirty state lag behind
        if (sender is DataGridView grid && grid.IsCurrentCellDirty && grid.CurrentCell is DataGridViewCheckBoxCell)
        {
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void DataGridViewCategories_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= visibleCategories.Count)
        {
            return;
        }

        var category = visibleCategories[e.RowIndex];

        if (e.ColumnIndex == columnCategoryIncluded.Index)
        {
            e.Value = category.IsIncluded;
        }
        else if (e.ColumnIndex == columnCategoryTitle.Index)
        {
            e.Value = category.Title;
        }
        else if (e.ColumnIndex == columnCategoryCount.Index)
        {
            e.Value = category.ChannelCountText;
        }
    }

    private void DataGridViewCategories_CellValuePushed(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= visibleCategories.Count)
        {
            return;
        }

        var category = visibleCategories[e.RowIndex];

        if (e.ColumnIndex == columnCategoryIncluded.Index)
        {
            category.IsIncluded = IsChecked(e.Value);
            OnIncludedStateChanged();
        }
        else if (e.ColumnIndex == columnCategoryTitle.Index)
        {
            var title = (e.Value as string)?.Trim();

            if (string.IsNullOrEmpty(title))
            {
                toolStripStatusLabel.Text = "A category name cannot be empty, the old name was kept.";
            }
            else if (title != category.Title)
            {
                category.Title = title;
                MarkDirty();
                RefreshChannelListHeader();
            }
        }
    }

    private void RefreshChannelListHeader()
    {
        if (!checkBoxSearchAllCategories.Checked && selectedCategory != null && textBoxChannelFilter.Text.Trim().Length == 0)
        {
            labelSelectedCategory.Text = $"{selectedCategory.Title} ({selectedCategory.Channels.Count:N0} channels)";
        }

        dataGridViewChannels.InvalidateColumn(columnChannelCategory.Index);
    }

    private void DataGridViewCategories_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < VisibleCategories.Count && e.CellStyle != null && !VisibleCategories[e.RowIndex].IsIncluded)
        {
            e.CellStyle.ForeColor = ThemeManager.Current.MutedText;
        }
    }

    private void DataGridView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not DragDataGridView grid || grid.EditingControl != null)
        {
            return;
        }

        if (e.KeyData == (Keys.Control | Keys.C))
        {
            CopySelectedRows(grid);
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        if (e.KeyData != Keys.Space)
        {
            return;
        }

        // Space is always handled here (not by the check box cell, which only reacts while in edit mode and
        // only toggles one row). The check box cell would still toggle itself on key up, so that is swallowed.
        var checkBoxColumn = grid == dataGridViewCategories ? columnCategoryIncluded.Index : columnChannelIncluded.Index;
        suppressSpaceKeyUp = grid.CurrentCellAddress.X == checkBoxColumn;

        var rows = grid.GetSelectedRowIndexes();
        if (rows.Count == 0 && grid.CurrentCellAddress.Y >= 0)
        {
            rows.Add(grid.CurrentCellAddress.Y);
        }

        var items = rows.Select(GetRowItem(grid)).ToList();
        bool newValue = !items.All(IsIncluded);

        foreach (var item in items)
        {
            SetIncluded(item, newValue);
        }

        e.Handled = true;
        e.SuppressKeyPress = true;

        // A check box cell in edit mode draws its cached value, make it read the new one
        if (grid.IsCurrentCellInEditMode)
        {
            grid.RefreshEdit();
        }

        OnIncludedStateChanged();
    }

    /// <summary>
    /// Ctrl+C copies category titles, or channel names and URLs as an M3U snippet. The grid's own copy is disabled:
    /// it builds text, CSV and HTML for every selected cell, which freezes with hundreds of thousands of rows.
    /// </summary>
    private void CopySelectedRows(DragDataGridView grid)
    {
        var rows = grid.GetSelectedRowIndexes();
        var text = new System.Text.StringBuilder();

        foreach (var row in rows)
        {
            switch (GetRowItem(grid)(row))
            {
                case Category category:
                    text.AppendLine(category.Title);
                    break;

                case Channel channel:
                    text.Append("#EXTINF:-1,").AppendLine(channel.Name).AppendLine(channel.Url);
                    break;
            }
        }

        if (text.Length > 0)
        {
            SetClipboardText(text.ToString());
            toolStripStatusLabel.Text = $"Copied {rows.Count:N0} row(s).";
        }
    }

    private void DataGridView_KeyUp(object? sender, KeyEventArgs e)
    {
        if (suppressSpaceKeyUp && e.KeyCode == Keys.Space)
        {
            suppressSpaceKeyUp = false;
            e.Handled = true;
        }
    }

    private Func<int, object?> GetRowItem(DataGridView grid) => grid == dataGridViewCategories
        ? row => row < visibleCategories.Count ? visibleCategories[row] : null
        : row => row < visibleChannels.Count ? visibleChannels[row] : null;

    private static bool IsIncluded(object? item) => item switch
    {
        Category category => category.IsIncluded,
        Channel channel => channel.IsIncluded,
        _ => false,
    };

    private static void SetIncluded(object? item, bool value)
    {
        if (item is Category category)
        {
            category.IsIncluded = value;
        }
        else if (item is Channel channel)
        {
            channel.IsIncluded = value;
        }
    }

    private void DataGridView_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
    {
        // Right-clicking a row selects it, so the context menu acts on what the user clicked
        if (e.Button != MouseButtons.Right || e.RowIndex < 0 || sender is not DataGridView grid)
        {
            return;
        }

        if (!((DragDataGridView)grid).IsRowSelected(e.RowIndex))
        {
            CommitGridEdits();
            grid.ClearSelection();
            grid.CurrentCell = grid[Math.Max(e.ColumnIndex, 0), e.RowIndex];
        }
    }

    #endregion

    #region Filtering

    private void TextBoxFilter_TextChanged(object? sender, EventArgs e)
    {
        if (sender == textBoxCategoryFilter)
        {
            categoryFilterChanged = true;
        }
        else
        {
            channelFilterChanged = true;
        }

        filterTimer.Stop();
        filterTimer.Start();
    }

    private void FilterTimer_Tick(object? sender, EventArgs e)
    {
        filterTimer.Stop();

        // Only redo the filtering that changed, searching all channels of a huge playlist is the expensive part
        bool channelsRefreshed = categoryFilterChanged && RefreshCategoryList();

        if (channelFilterChanged && !channelsRefreshed)
        {
            RefreshChannelList();
        }

        categoryFilterChanged = false;
        channelFilterChanged = false;
    }

    private void TextBoxFilter_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (e.KeyCode == Keys.Escape)
        {
            textBox.Clear();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
        {
            // Jump straight into the results
            var grid = textBox == textBoxCategoryFilter ? dataGridViewCategories : dataGridViewChannels;
            FilterTimer_Tick(null, EventArgs.Empty);
            grid.Focus();
            e.SuppressKeyPress = true;
        }
    }

    private void CheckBoxSearchAllCategories_CheckedChanged(object? sender, EventArgs e)
    {
        RefreshChannelList();

        if (checkBoxSearchAllCategories.Checked)
        {
            textBoxChannelFilter.Focus();
        }
    }

    #endregion

    #region Select all / clear all / invert

    private void SetCategoriesIncluded(Func<Category, bool> value)
    {
        CommitGridEdits();

        foreach (var category in VisibleCategories)
        {
            category.IsIncluded = value(category);
        }

        OnIncludedStateChanged();
    }

    private void SetChannelsIncluded(Func<Channel, bool> value)
    {
        CommitGridEdits();

        foreach (var channel in VisibleChannels)
        {
            channel.IsIncluded = value(channel);
        }

        OnIncludedStateChanged();
    }

    private void ButtonSelectAllCategories_Click(object sender, EventArgs e) => SetCategoriesIncluded(_ => true);

    private void ButtonClearAllCategories_Click(object sender, EventArgs e) => SetCategoriesIncluded(_ => false);

    private void ButtonInvertCategories_Click(object sender, EventArgs e) => SetCategoriesIncluded(c => !c.IsIncluded);

    private void ButtonSelectAllChannels_Click(object sender, EventArgs e) => SetChannelsIncluded(_ => true);

    private void ButtonClearAllChannels_Click(object sender, EventArgs e) => SetChannelsIncluded(_ => false);

    private void ButtonInvertChannels_Click(object sender, EventArgs e) => SetChannelsIncluded(c => !c.IsIncluded);

    #endregion

    #region Moving channels and categories

    private void MoveChannelsTo(List<Channel> channels, Category target, Channel? before)
    {
        CommitGridEdits();

        if (!PlaylistTools.MoveChannels(playlist, channels, target, before))
        {
            return;
        }

        // The category the user was looking at may have been emptied and removed
        if (selectedCategory == null || !playlist.Categories.Contains(selectedCategory))
        {
            selectedCategory = target;
        }

        MarkDirty();

        int firstRow = dataGridViewChannels.FirstDisplayedScrollingRowIndex;
        RefreshCategoryList();
        RefreshChannelList();
        RestoreScrollPosition(dataGridViewChannels, firstRow);
        SelectChannels(channels);
        UpdateStatus();

        toolStripStatusLabel.Text = $"Moved {channels.Count:N0} channel(s) to \"{target.Title}\".";
    }

    private static void RestoreScrollPosition(DataGridView grid, int firstRow)
    {
        if (firstRow >= 0 && firstRow < grid.Rows.Count)
        {
            try
            {
                grid.FirstDisplayedScrollingRowIndex = firstRow;
            }
            catch (InvalidOperationException)
            {
                // The grid is not visible yet
            }
        }
    }

    private void SelectChannels(IReadOnlyCollection<Channel> channels)
    {
        var wanted = channels.ToHashSet();
        var rows = new List<int>();

        for (int i = 0; i < visibleChannels.Count; i++)
        {
            if (wanted.Contains(visibleChannels[i]))
            {
                rows.Add(i);
            }
        }

        if (rows.Count == 0)
        {
            return;
        }

        dataGridViewChannels.CurrentCell = dataGridViewChannels[columnChannelName.Index, rows[0]];

        foreach (var row in rows.Skip(1))
        {
            dataGridViewChannels.SelectRow(row);
        }
    }

    private void MoveToCategoryToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        var channels = SelectedChannels;
        if (channels.Count == 0)
        {
            return;
        }

        using var dialog = new CategoryPickerDialog(playlist.Categories, channels.Count);

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var target = dialog.SelectedCategory ?? PlaylistTools.FindCategory(playlist, dialog.NewCategoryName!) ?? new Category(dialog.NewCategoryName!);

        if (channels.All(c => c.Category == target))
        {
            toolStripStatusLabel.Text = $"The channels are already in \"{target.Title}\".";
            return;
        }

        MoveChannelsTo(channels, target, null);
    }

    #endregion

    #region Context menus

    private void ContextMenuCategories_Opening(object? sender, CancelEventArgs e)
    {
        e.Cancel = CurrentCategory == null;
    }

    private void MoveCurrentCategory(int offset)
    {
        if (CurrentCategory is not Category category)
        {
            return;
        }

        CommitGridEdits();
        PlaylistTools.MoveCategory(playlist, category, offset);
        selectedCategory = category;
        MarkDirty();
        RefreshCategoryList();
    }

    private void MoveCategoryUpToolStripMenuItem_Click(object sender, EventArgs e) => MoveCurrentCategory(-1);

    private void MoveCategoryDownToolStripMenuItem_Click(object sender, EventArgs e) => MoveCurrentCategory(1);

    private void MoveCategoryToTopToolStripMenuItem_Click(object sender, EventArgs e) => MoveCurrentCategory(-playlist.Categories.Count);

    private void MoveCategoryToBottomToolStripMenuItem_Click(object sender, EventArgs e) => MoveCurrentCategory(playlist.Categories.Count);

    private void ContextMenuChannels_Opening(object? sender, CancelEventArgs e)
    {
        // Keep this cheap: it runs on every right-click, also with hundreds of thousands of rows
        int selected = dataGridViewChannels.SelectedRowCount;
        e.Cancel = selected == 0 || CurrentChannel == null;

        if (!e.Cancel)
        {
            goToCategoryToolStripMenuItem.Visible = checkBoxSearchAllCategories.Checked;
            moveToCategoryToolStripMenuItem.Text = selected == 1 ? "Move to Category..." : $"Move {selected:N0} Channels to Category...";
        }
    }

    private void PlayChannelToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentChannel is not Channel channel)
        {
            return;
        }

        // Open a one-channel playlist with the default player (VLC, MPC-HC, ...), so #EXTVLCOPT options are applied too
        var category = new Category(channel.Category.Title);
        category.Channels.Add(new Channel(category, channel.Name, channel.Url)
        {
            Duration = channel.Duration,
            Attributes = channel.Attributes,
            ExtraLines = channel.ExtraLines,
        });

        var previewPath = Path.Combine(Path.GetTempPath(), "M3UPlaylistHelper-preview.m3u");

        try
        {
            M3UWriter.WriteFile(new Playlist { Categories = [category] }, previewPath);
            Process.Start(new ProcessStartInfo(previewPath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Could not start a player:\n\n{ex.Message}\n\nMake sure a media player such as VLC is installed and associated with .m3u files.",
                AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void CopyChannelUrlToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentChannel is Channel channel)
        {
            SetClipboardText(channel.Url);
        }
    }

    private void CopyChannelNameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentChannel is Channel channel)
        {
            SetClipboardText(channel.Name);
        }
    }

    private void SetClipboardText(string text)
    {
        try
        {
            Clipboard.SetText(text);
        }
        catch (Exception ex)
        {
            // The clipboard can be locked by another application
            toolStripStatusLabel.Text = $"Could not copy to the clipboard: {ex.Message}";
        }
    }

    private void GoToCategoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentChannel is not Channel channel)
        {
            return;
        }

        CommitGridEdits();
        selectedCategory = channel.Category;
        textBoxCategoryFilter.Clear();
        textBoxChannelFilter.Clear();
        filterTimer.Stop();
        categoryFilterChanged = channelFilterChanged = false;
        checkBoxSearchAllCategories.Checked = false;
        RefreshCategoryList();
        RefreshChannelList();
        SelectChannels([channel]);
    }

    #endregion

    #region Drag and drop

    private void EnableDragAndDrop(Control control)
    {
        // Text boxes keep their normal text drop behavior, the grids have their own handlers (they also accept rows)
        if (control is TextBoxBase or DataGridView)
        {
            return;
        }

        control.AllowDrop = true;
        control.DragEnter += Control_DragEnter;
        control.DragDrop += Control_DragDrop;

        foreach (Control child in control.Controls)
        {
            EnableDragAndDrop(child);
        }
    }

    /// <summary>
    /// Playlist files or a link dragged in from outside the app.
    /// </summary>
    private static List<PlaylistSource> GetDroppedSources(IDataObject? data)
    {
        if (data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
        {
            return files.Where(File.Exists).Select(PlaylistSource.FromPathOrUrl).ToList();
        }

        if (data?.GetData(DataFormats.UnicodeText) is string text && AppSettings.IsUrl(text.Trim()))
        {
            return [PlaylistSource.FromPathOrUrl(text.Trim())];
        }

        return [];
    }

    private void Control_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = !isLoading && GetDroppedSources(e.Data).Count > 0 ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void Control_DragDrop(object? sender, DragEventArgs e)
    {
        var sources = GetDroppedSources(e.Data);
        if (sources.Count > 0)
        {
            // Let the drag source return before showing any dialogs
            BeginInvoke(async () => await OpenDroppedAsync(sources));
        }
    }

    private async Task OpenDroppedAsync(List<PlaylistSource> sources)
    {
        bool append = false;

        if (playlist.Categories.Count > 0)
        {
            var answer = MessageBox.Show(
                this,
                "Add the dropped playlist(s) to the current playlist?\n\nYes: add them to the current playlist\nNo: close the current playlist and open them instead",
                AppName,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (answer == DialogResult.Cancel)
            {
                return;
            }

            append = answer == DialogResult.Yes;
        }

        await OpenAllAsync(sources, append);
    }

    private void DataGridView_DragOver(object? sender, DragEventArgs e)
    {
        if (sender is not DataGridView grid)
        {
            return;
        }

        if (GetDroppedSources(e.Data).Count > 0)
        {
            e.Effect = isLoading ? DragDropEffects.None : DragDropEffects.Copy;
            SetDropIndicator(null, -1, false);
            return;
        }

        var point = grid.PointToClient(new Point(e.X, e.Y));
        ScrollWhileDragging(grid, point);

        var target = GetDropTarget(grid, e.Data, point);
        e.Effect = target == null ? DragDropEffects.None : DragDropEffects.Move;
        SetDropIndicator(target?.Grid, target?.Row ?? -1, target?.OnRow ?? false);
    }

    private void DataGridView_DragLeave(object? sender, EventArgs e)
    {
        SetDropIndicator(null, -1, false);
    }

    private void DataGridView_DragDrop(object? sender, DragEventArgs e)
    {
        SetDropIndicator(null, -1, false);

        var sources = GetDroppedSources(e.Data);
        if (sources.Count > 0)
        {
            BeginInvoke(async () => await OpenDroppedAsync(sources));
            return;
        }

        if (sender is not DataGridView grid || GetDropTarget(grid, e.Data, grid.PointToClient(new Point(e.X, e.Y))) is not DropTarget target)
        {
            return;
        }

        if (e.Data?.GetData(typeof(ChannelDragData)) is ChannelDragData channelData)
        {
            if (grid == dataGridViewCategories)
            {
                MoveChannelsTo(channelData.Channels, VisibleCategories[target.Row], null);
                return;
            }

            // Insert in front of the first row below the drop position that is not being dragged itself
            var visible = VisibleChannels;
            var dragged = channelData.Channels.ToHashSet();
            int row = target.Row;
            while (row < visible.Count && dragged.Contains(visible[row]))
            {
                row++;
            }

            var before = row < visible.Count ? visible[row] : null;
            Channel? above = null;
            for (int i = Math.Min(target.Row, visible.Count) - 1; i >= 0 && above == null; i--)
            {
                above = dragged.Contains(visible[i]) ? null : visible[i];
            }
            var category = before?.Category ?? above?.Category ?? selectedCategory ?? channelData.Channels[0].Category;

            MoveChannelsTo(channelData.Channels, category, before);
        }
        else if (e.Data?.GetData(typeof(CategoryDragData)) is CategoryDragData categoryData)
        {
            var visible = VisibleCategories;
            var before = target.Row < visible.Count ? visible[target.Row] : null;

            CommitGridEdits();

            if (PlaylistTools.MoveCategoryBefore(playlist, categoryData.Category, before))
            {
                selectedCategory = categoryData.Category;
                MarkDirty();
                RefreshCategoryList();
            }
        }
    }

    private DropTarget? GetDropTarget(DataGridView grid, IDataObject? data, Point point)
    {
        var hit = grid.HitTest(point.X, point.Y);

        // Rows are inserted before the row under the mouse, or after it when over its lower half
        int insertRow = -1;
        if (hit.RowIndex >= 0)
        {
            var bounds = grid.GetRowDisplayRectangle(hit.RowIndex, false);
            insertRow = point.Y > bounds.Top + bounds.Height / 2 ? hit.RowIndex + 1 : hit.RowIndex;
        }
        else if (hit.Type == DataGridViewHitTestType.None && point.Y > grid.ColumnHeadersHeight)
        {
            // Below the last row
            insertRow = grid.Rows.Count;
        }

        if (data?.GetDataPresent(typeof(ChannelDragData)) == true)
        {
            if (grid == dataGridViewCategories)
            {
                // Dropping channels on the category they are already in would do nothing
                bool isOtherCategory = hit.RowIndex >= 0 && hit.RowIndex < visibleCategories.Count &&
                    data.GetData(typeof(ChannelDragData)) is ChannelDragData dragged &&
                    dragged.Channels.Any(c => c.Category != visibleCategories[hit.RowIndex]);

                return isOtherCategory ? new DropTarget(grid, hit.RowIndex, true) : null;
            }

            bool hasTargetCategory = grid.Rows.Count > 0 || selectedCategory != null;
            return insertRow >= 0 && hasTargetCategory ? new DropTarget(grid, insertRow, false) : null;
        }

        if (data?.GetDataPresent(typeof(CategoryDragData)) == true && grid == dataGridViewCategories && insertRow >= 0)
        {
            return new DropTarget(grid, insertRow, false);
        }

        return null;
    }

    private void SetDropIndicator(DataGridView? grid, int row, bool onRow)
    {
        if (grid == dropGrid && row == dropRow && onRow == dropOnRow)
        {
            return;
        }

        dropGrid?.Invalidate();
        dropGrid = grid;
        dropRow = row;
        dropOnRow = onRow;
        dropGrid?.Invalidate();
    }

    private static void ScrollWhileDragging(DataGridView grid, Point point)
    {
        const int edge = 30;

        try
        {
            if (point.Y < grid.ColumnHeadersHeight + edge && grid.FirstDisplayedScrollingRowIndex > 0)
            {
                grid.FirstDisplayedScrollingRowIndex--;
            }
            else if (point.Y > grid.ClientSize.Height - edge && grid.FirstDisplayedScrollingRowIndex + grid.DisplayedRowCount(false) < grid.Rows.Count)
            {
                grid.FirstDisplayedScrollingRowIndex++;
            }
        }
        catch (InvalidOperationException)
        {
            // No rows to scroll
        }
    }

    private void DataGridView_RowPostPaint(object? sender, DataGridViewRowPostPaintEventArgs e)
    {
        if (sender != dropGrid || sender is not DataGridView grid || dropRow < 0)
        {
            return;
        }

        var bounds = e.RowBounds;
        bounds.Width = Math.Min(bounds.Width, grid.ClientSize.Width);
        using var pen = new Pen(ThemeManager.Current.Accent, 3);

        if (dropOnRow && e.RowIndex == dropRow)
        {
            e.Graphics.DrawRectangle(pen, bounds.X + 1, bounds.Y + 1, bounds.Width - 3, bounds.Height - 3);
        }
        else if (!dropOnRow && e.RowIndex == dropRow)
        {
            e.Graphics.DrawLine(pen, bounds.Left, bounds.Top + 1, bounds.Right, bounds.Top + 1);
        }
        else if (!dropOnRow && dropRow == grid.Rows.Count && e.RowIndex == dropRow - 1)
        {
            e.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom - 2, bounds.Right, bounds.Bottom - 2);
        }
    }

    #endregion

    #region EPG

    private List<string> ProviderEpgSources
    {
        get
        {
            var sources = playlist.EpgUrls.ToList();

            if (xtreamAccount != null && !sources.Contains(xtreamAccount.EpgUrl, StringComparer.OrdinalIgnoreCase))
            {
                sources.Add(xtreamAccount.EpgUrl);
            }

            return sources;
        }
    }

    private void EpgToolStripMenuItem_DropDownOpening(object? sender, EventArgs e)
    {
        int providerSources = ProviderEpgSources.Count;
        loadProviderEpgToolStripMenuItem.Enabled = providerSources > 0;
        loadProviderEpgToolStripMenuItem.Text = providerSources > 1
            ? $"Load EPG from &Playlist / Provider ({providerSources} sources)"
            : "Load EPG from &Playlist / Provider";

        excludeChannelsWithoutEpgToolStripMenuItem.Enabled = epg != null;
        fillTvgIdsToolStripMenuItem.Enabled = epg != null;
        clearEpgToolStripMenuItem.Enabled = epg != null;
    }

    /// <summary>
    /// Only the host is shown for URLs, provider EPG URLs contain the account password.
    /// </summary>
    private static string DescribeEpgSource(string source) =>
        AppSettings.IsUrl(source) ? new Uri(source).Host : Path.GetFileName(source);

    private async Task LoadEpgAsync(IReadOnlyList<string> sources, bool offerToAddToHeader)
    {
        if (isLoading || sources.Count == 0)
        {
            return;
        }

        var guide = new XmlTvGuide();
        var errors = new List<string>();
        string? resultMessage = null;

        SetLoading(true, "Loading EPG...");

        try
        {
            foreach (var source in sources)
            {
                toolStripStatusLabel.Text = $"Loading EPG from {DescribeEpgSource(source)}...";

                try
                {
                    var loaded = AppSettings.IsUrl(source)
                        ? await Task.Run(() => XmlTvGuide.LoadUrlAsync(source, CancellationToken.None))
                        : await Task.Run(() => XmlTvGuide.LoadFileAsync(source, CancellationToken.None));

                    guide.Add(loaded);
                }
                catch (Exception ex)
                {
                    errors.Add($"{DescribeEpgSource(source)}: {ex.Message}");
                }
            }

            if (guide.ChannelCount == 0)
            {
                var reason = errors.Count > 0 ? string.Join("\n", errors) : "The guide does not list any channels.";
                MessageBox.Show(this, $"Could not load the EPG:\n\n{reason}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            epg = guide;
            int total = playlist.AllChannels.Count();
            int withGuide = playlist.AllChannels.Count(guide.HasGuide);
            resultMessage = $"EPG loaded with {guide.ChannelCount:N0} guide channels. {withGuide:N0} of {total:N0} playlist channels have program information.";

            if (errors.Count > 0)
            {
                MessageBox.Show(this, $"Some EPG sources could not be loaded:\n\n{string.Join("\n", errors)}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var missingFromHeader = sources.Where(AppSettings.IsUrl).Except(playlist.EpgUrls, StringComparer.OrdinalIgnoreCase).ToList();

            if (offerToAddToHeader && missingFromHeader.Count > 0 &&
                MessageBox.Show(
                    this,
                    "Add this EPG to the playlist header (url-tvg), so players load the guide automatically?",
                    AppName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                missingFromHeader.ForEach(playlist.AddEpgUrl);
                MarkDirty();
            }
        }
        finally
        {
            SetLoading(false);
            RefreshChannelList();
            UpdateStatus();

            if (resultMessage != null)
            {
                toolStripStatusLabel.Text = resultMessage;
            }
        }
    }

    private async void LoadProviderEpgToolStripMenuItem_Click(object sender, EventArgs e)
    {
        await LoadEpgAsync(ProviderEpgSources, offerToAddToHeader: true);
    }

    private async void LoadEpgFromUrlToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Provider EPG URLs carry the account password: keep them out of the history and the settings file
        using var dialog = new OpenURLDialog("Load EPG from URL", settings.LastEpgUrl, playlist.EpgUrls.Where(url => !ContainsPassword(url)));

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        settings.LastEpgUrl = ContainsPassword(dialog.Url) ? null : dialog.Url;
        settings.Save();
        await LoadEpgAsync([dialog.Url], offerToAddToHeader: true);
    }

    private static bool ContainsPassword(string url) =>
        url.Contains("password=", StringComparison.OrdinalIgnoreCase);

    private async void LoadEpgFromFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = EpgFileFilter,
            Title = "Load EPG (XMLTV) File",
            Multiselect = true,
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            await LoadEpgAsync(dialog.FileNames, offerToAddToHeader: false);
        }
    }

    private void ExcludeChannelsWithoutEpgToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (epg == null)
        {
            return;
        }

        CommitGridEdits();
        int count = epg.ExcludeChannelsWithoutGuide(playlist);

        if (count > 0)
        {
            OnIncludedStateChanged();
        }

        MessageBox.Show(
            this,
            count == 0 ? "All selected channels have EPG data." : $"{count:N0} channels without EPG data were unchecked.",
            AppName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void FillTvgIdsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (epg == null)
        {
            return;
        }

        CommitGridEdits();
        int count = epg.FillMissingTvgIds(playlist);

        if (count > 0)
        {
            MarkDirty();
            dataGridViewChannels.Invalidate();
            UpdateStatus();
        }

        MessageBox.Show(
            this,
            count == 0
                ? "No channel names could be matched to the EPG."
                : $"{count:N0} channels got a tvg-id from the EPG by matching their name. Check the EPG column to review them.",
            AppName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ClearEpgToolStripMenuItem_Click(object sender, EventArgs e)
    {
        epg = null;
        RefreshChannelList();
        UpdateStatus();
    }

    #endregion

    #region Menu handlers

    private string[]? PromptForPlaylistFiles(string title)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Filter = PlaylistFileFilter,
            Title = title,
            Multiselect = true,
        };

        return openFileDialog.ShowDialog(this) == DialogResult.OK ? openFileDialog.FileNames : null;
    }

    private string? PromptForPlaylistUrl(string title)
    {
        using var openUrlDialog = new OpenURLDialog(title, settings.LastUrl, settings.RecentUrls);

        if (openUrlDialog.ShowDialog(this) != DialogResult.OK)
        {
            return null;
        }

        // Remembered even if loading fails, so a typo can be fixed next time
        settings.LastUrl = openUrlDialog.Url;
        settings.Save();
        return openUrlDialog.Url;
    }

    private XtreamAccount? PromptForXtreamAccount()
    {
        using var dialog = new XtreamDialog(settings.XtreamServer, settings.XtreamUsername, settings.GetXtreamPassword(), settings.XtreamOutput);

        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Account is not XtreamAccount account)
        {
            return null;
        }

        settings.XtreamServer = account.Server;
        settings.XtreamUsername = account.Username;
        settings.XtreamOutput = account.Output;

        try
        {
            settings.SetXtreamPassword(dialog.RememberPassword ? account.Password : null);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to store the password: {ex.Message}");
        }

        settings.Save();
        return account;
    }

    private async void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PromptForPlaylistFiles("Open M3U Playlist") is string[] files)
        {
            await OpenAllAsync(files.Select(PlaylistSource.FromPathOrUrl).ToList(), append: false);
        }
    }

    private async void OpenURLToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PromptForPlaylistUrl("Open M3U URL") is string url)
        {
            await OpenAsync(PlaylistSource.FromPathOrUrl(url), append: false);
        }
    }

    private async void OpenXtreamToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PromptForXtreamAccount() is XtreamAccount account)
        {
            await OpenAsync(PlaylistSource.FromXtream(account), append: false);
        }
    }

    private async void AddFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PromptForPlaylistFiles("Add M3U Playlist") is string[] files)
        {
            await OpenAllAsync(files.Select(PlaylistSource.FromPathOrUrl).ToList(), append: true);
        }
    }

    private async void AddURLToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PromptForPlaylistUrl("Add M3U URL") is string url)
        {
            await OpenAsync(PlaylistSource.FromPathOrUrl(url), append: true);
        }
    }

    private async void AddXtreamToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (PromptForXtreamAccount() is XtreamAccount account)
        {
            await OpenAsync(PlaylistSource.FromXtream(account), append: true);
        }
    }

    private void SaveToolStripMenuItem_Click(object sender, EventArgs e) => Save();

    private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) => SaveAs();

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

    private void FindToolStripMenuItem_Click(object sender, EventArgs e)
    {
        textBoxChannelFilter.Focus();
        textBoxChannelFilter.SelectAll();
    }

    private void ExcludeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CommitGridEdits();
        int count = PlaylistTools.ExcludeDuplicateChannels(playlist);

        if (count > 0)
        {
            OnIncludedStateChanged();
        }

        MessageBox.Show(
            this,
            count == 0 ? "No duplicate channels found." : $"{count:N0} duplicate channels (same URL as an earlier channel) were unchecked.",
            AppName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void SortCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CommitGridEdits();
        PlaylistTools.SortCategories(playlist);
        MarkDirty();
        RefreshCategoryList();
    }

    private async void SaveSelectionProfileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CommitGridEdits();

        var includeNew = MessageBox.Show(
            this,
            "When this profile is applied to an updated playlist, should categories that don't exist yet be included?\n\n" +
            "Yes: include new categories\nNo: exclude new categories",
            "Save Selection Profile",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        if (includeNew == DialogResult.Cancel)
        {
            return;
        }

        using var saveFileDialog = new SaveFileDialog
        {
            Filter = ProfileFileFilter,
            Title = "Save Selection Profile",
            FileName = "selection-profile.json",
        };

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            await SelectionProfile.Capture(playlist, includeNew == DialogResult.Yes).SaveAsync(saveFileDialog.FileName, CancellationToken.None);
            toolStripStatusLabel.Text = $"Selection profile saved to {saveFileDialog.FileName}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not save the selection profile:\n\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void ApplySelectionProfileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Filter = ProfileFileFilter,
            Title = "Apply Selection Profile",
        };

        if (openFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var profile = await SelectionProfile.LoadAsync(openFileDialog.FileName, CancellationToken.None);
            CommitGridEdits();
            profile.Apply(playlist);
            OnIncludedStateChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not apply the selection profile:\n\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void HowToUseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            this,
            "1. Open a playlist: File > Open File (Ctrl+O), Open URL (Ctrl+U) or Open Xtream Codes, or drag and drop files or links onto the window.\n" +
            "   File > Add to Current Playlist merges more playlists into the open one.\n" +
            "2. Pick the categories and channels to keep with the check boxes. Space toggles the selected rows.\n" +
            "   Select All / Clear All / Invert only affect the rows that match the current filter.\n" +
            "3. Use the search boxes to find categories and channels. Tick \"All categories\" to search every channel.\n" +
            "4. Double-click (or F2) a category or channel name to rename it.\n" +
            "5. Drag channels to reorder them, or drop them on a category to move them there. Drag categories to reorder them.\n" +
            "   Right-click a channel to play it, copy its URL or move it to another (or a new) category.\n" +
            "6. EPG > Load EPG shows which channels have program information and can fill in missing tvg-ids.\n" +
            "7. Save (Ctrl+S) or Save As (Ctrl+Shift+S). Only checked items are written.\n\n" +
            "Tip: Edit > Save Selection Profile remembers your choices. When your provider updates the playlist, " +
            "open the new one and use Edit > Apply Selection Profile instead of selecting everything again.",
            "How to Use",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var mode = AppSettings.IsPortable ? "\n\nPortable mode: settings are stored next to the program." : string.Empty;
        MessageBox.Show(this, $"{AppName} {version?.ToString(3)}\n\nFilter, rename, reorder and merge M3U/M3U8 playlists.{mode}", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion
}
