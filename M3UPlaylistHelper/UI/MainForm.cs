namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Model;
using M3UPlaylistHelper.Parser;
using M3UPlaylistHelper.Tools;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private const string AppName = "M3U Playlist Helper";
    private const string PlaylistFileFilter = "M3U Playlist (*.m3u;*.m3u8)|*.m3u;*.m3u8|All Files (*.*)|*.*";
    private const string ProfileFileFilter = "Selection Profile (*.json)|*.json";

    private readonly AppSettings settings = AppSettings.Load();
    private readonly string? startupItem;

    // Debounces typing in the filter boxes, so large playlists are not re-filtered on every key press
    private readonly System.Windows.Forms.Timer filterTimer = new() { Interval = 250 };

    // Debounces scrolling, logos are only downloaded for the rows on screen
    private readonly System.Windows.Forms.Timer logoTimer = new() { Interval = 200 };

    private Playlist playlist = new();
    private Category? selectedCategory;
    private string? currentFilePath;
    private string? currentSource;
    private bool isDirty;
    private bool isLoading;
    private bool suppressCategorySelectionChanged;
    private CancellationTokenSource? logoCancellation;

    public MainForm(string? startupItem = null)
    {
        InitializeComponent();

        this.startupItem = startupItem;

        dataGridViewCategories.AutoGenerateColumns = false;
        dataGridViewChannels.AutoGenerateColumns = false;
        dataGridViewChannels.RowTemplate.Height = Math.Max(dataGridViewChannels.RowTemplate.Height, LogoCache.ThumbnailHeight + 8);

        // Show an empty cell instead of the "missing image" icon when a channel has no logo
        columnChannelLogo.DefaultCellStyle.NullValue = null;

        checkBoxDownloadLogos.Checked = settings.DownloadLogos;
        columnChannelLogo.Visible = settings.DownloadLogos;

        filterTimer.Tick += FilterTimer_Tick;
        logoTimer.Tick += LogoTimer_Tick;

        EnableDragAndDrop(this);
        RestoreWindowLayout();
        UpdateRecentMenu();
        RefreshAll();
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
            await OpenAsync(startupItem);
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

    #region Opening and saving

    private async Task OpenAsync(string source)
    {
        if (isLoading || !ConfirmDiscardChanges())
        {
            return;
        }

        bool isUrl = AppSettings.IsUrl(source);
        SetLoading(true, isUrl ? "Downloading playlist..." : "Reading playlist...");

        try
        {
            var loaded = isUrl
                ? await M3UParser.ParseUrlAsync(source, CancellationToken.None)
                : await Task.Run(() => M3UParser.ParseFileAsync(source, CancellationToken.None));

            playlist = loaded;
            selectedCategory = null;
            currentFilePath = isUrl ? null : source;
            currentSource = source;
            isDirty = false;

            settings.AddRecentItem(source);
            settings.Save();
            UpdateRecentMenu();

            textBoxCategoryFilter.Clear();
            textBoxChannelFilter.Clear();
            checkBoxSearchAllCategories.Checked = false;
        }
        catch (Exception ex)
        {
            if (!isUrl && (ex is FileNotFoundException || ex is DirectoryNotFoundException))
            {
                settings.RecentItems.Remove(source);
                settings.Save();
                UpdateRecentMenu();
            }

            MessageBox.Show(this, $"Could not open the playlist:\n\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetLoading(false);
            RefreshAll();
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
            M3UWriter.WriteFile(playlist, path);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not save the playlist:\n\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        currentFilePath = path;
        currentSource = path;
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
        dataGridViewCategories.EndEdit();
        dataGridViewChannels.EndEdit();
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
        RefreshCategoryList();
        RefreshChannelList();
        UpdateTitle();
        UpdateStatus();
        UpdateMenuState();
    }

    private void UpdateTitle()
    {
        var dirtyMarker = isDirty ? "*" : string.Empty;
        Text = currentSource == null ? AppName : $"{AppName} - {currentSource}{dirtyMarker}";
    }

    private void UpdateStatus()
    {
        if (isLoading)
        {
            return;
        }

        if (playlist.Categories.Count == 0)
        {
            toolStripStatusLabel.Text = "Open a playlist file or URL to get started. You can also drag and drop a file or link onto this window.";
            return;
        }

        int includedCategories = playlist.Categories.Count(c => c.IsIncluded);
        int channels = playlist.AllChannels.Count();
        int exported = playlist.ExportedChannels.Count();

        toolStripStatusLabel.Text =
            $"{playlist.Categories.Count:N0} categories ({includedCategories:N0} included)  |  " +
            $"{channels:N0} channels  |  {exported:N0} channels will be saved";
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
    }

    private void UpdateRecentMenu()
    {
        recentToolStripMenuItem.DropDownItems.Clear();

        for (int i = 0; i < settings.RecentItems.Count; i++)
        {
            var item = settings.RecentItems[i];
            var menuItem = new ToolStripMenuItem($"&{(i + 1) % 10} {item.Replace("&", "&&")}") { Tag = item };
            menuItem.Click += async (_, _) => await OpenAsync(item);
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
    }

    private void RefreshCategoryList()
    {
        var filter = textBoxCategoryFilter.Text.Trim();
        var visible = filter.Length == 0
            ? playlist.Categories.ToList()
            : playlist.Categories.Where(c => PlaylistTools.Matches(c.Title, filter)).ToList();

        suppressCategorySelectionChanged = true;

        try
        {
            dataGridViewCategories.DataSource = visible;

            var target = selectedCategory != null && visible.Contains(selectedCategory) ? selectedCategory : visible.FirstOrDefault();
            if (target != null)
            {
                dataGridViewCategories.CurrentCell = dataGridViewCategories.Rows[visible.IndexOf(target)].Cells[columnCategoryTitle.Index];
            }

            if (target != selectedCategory)
            {
                selectedCategory = target;
                RefreshChannelList();
            }
        }
        finally
        {
            suppressCategorySelectionChanged = false;
        }
    }

    private void RefreshChannelList()
    {
        var filter = textBoxChannelFilter.Text.Trim();
        bool searchAll = checkBoxSearchAllCategories.Checked;

        IEnumerable<Channel> source = searchAll ? playlist.AllChannels : selectedCategory?.Channels ?? [];
        int total = source.Count();

        if (filter.Length > 0)
        {
            source = source.Where(c =>
                PlaylistTools.Matches(c.Name, filter) ||
                PlaylistTools.Matches(c.TvgName, filter) ||
                PlaylistTools.Matches(c.TvgId, filter));
        }

        var visible = source.ToList();
        dataGridViewChannels.DataSource = visible;
        columnChannelCategory.Visible = searchAll;

        var title = searchAll ? "All categories" : selectedCategory?.Title;
        labelSelectedCategory.Text = title == null
            ? string.Empty
            : filter.Length > 0 ? $"{title} ({visible.Count:N0} of {total:N0} channels match)" : $"{title} ({total:N0} channels)";

        LoadVisibleLogos();
    }

    private List<Category> VisibleCategories =>
        dataGridViewCategories.DataSource as List<Category> ?? [];

    private List<Channel> VisibleChannels =>
        dataGridViewChannels.DataSource as List<Channel> ?? [];

    private Category? CurrentCategory =>
        dataGridViewCategories.CurrentRow?.DataBoundItem as Category;

    private Channel? CurrentChannel =>
        dataGridViewChannels.CurrentRow?.DataBoundItem as Channel;

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
            // The form was closed while logos were downloading
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

    private void DataGridViewChannels_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= VisibleChannels.Count)
        {
            return;
        }

        var channel = VisibleChannels[e.RowIndex];

        if (e.ColumnIndex == columnChannelLogo.Index)
        {
            e.Value = checkBoxDownloadLogos.Checked ? LogoCache.Get(channel.LogoUrl) : null;
            e.FormattingApplied = true;
        }

        // Grey out channels that will not be saved, either unchecked or in an unchecked category
        if (e.CellStyle != null && !(channel.IsIncluded && channel.Category.IsIncluded))
        {
            e.CellStyle.ForeColor = SystemColors.GrayText;
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

    private void DataGridViewCategories_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        if (e.ColumnIndex == columnCategoryTitle.Index)
        {
            MarkDirty();
            RefreshChannelListHeader();
        }
        else
        {
            OnIncludedStateChanged();
        }
    }

    private void DataGridViewChannels_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            OnIncludedStateChanged();
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

    private void DataGridView_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (sender is not DataGridView grid || !grid.IsCurrentCellInEditMode)
        {
            return;
        }

        bool isNameColumn = e.ColumnIndex == columnCategoryTitle.Index && grid == dataGridViewCategories
            || e.ColumnIndex == columnChannelName.Index && grid == dataGridViewChannels;

        if (isNameColumn && string.IsNullOrWhiteSpace(e.FormattedValue?.ToString()))
        {
            // Stay in edit mode until a name is typed, Esc restores the old name
            grid.Rows[e.RowIndex].ErrorText = "The name cannot be empty. Press Esc to undo.";
            e.Cancel = true;
        }
    }

    private void DataGridView_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (sender is DataGridView grid && e.RowIndex >= 0)
        {
            grid.Rows[e.RowIndex].ErrorText = string.Empty;
        }
    }

    private void DataGridViewCategories_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex >= 0 && e.RowIndex < VisibleCategories.Count && e.CellStyle != null && !VisibleCategories[e.RowIndex].IsIncluded)
        {
            e.CellStyle.ForeColor = SystemColors.GrayText;
        }
    }

    private void DataGridView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not DataGridView grid || grid.IsCurrentCellInEditMode || e.KeyCode != Keys.Space || e.Modifiers != Keys.None)
        {
            return;
        }

        var checkBoxColumn = grid == dataGridViewCategories ? columnCategoryIncluded.Index : columnChannelIncluded.Index;

        // A single check box cell already toggles itself with space
        if (grid.SelectedRows.Count <= 1 && grid.CurrentCell?.ColumnIndex == checkBoxColumn)
        {
            return;
        }

        var rows = grid.SelectedRows.Cast<DataGridViewRow>().ToList();
        if (rows.Count == 0 && grid.CurrentRow != null)
        {
            rows.Add(grid.CurrentRow);
        }

        var items = rows.Select(r => r.DataBoundItem).ToList();
        bool newValue = !items.All(IsIncluded);

        foreach (var item in items)
        {
            SetIncluded(item, newValue);
        }

        e.Handled = true;
        e.SuppressKeyPress = true;
        OnIncludedStateChanged();
    }

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

        var row = grid.Rows[e.RowIndex];
        if (!row.Selected)
        {
            grid.ClearSelection();
            grid.CurrentCell = row.Cells[Math.Max(e.ColumnIndex, 0)];
            row.Selected = true;
        }
    }

    #endregion

    #region Filtering

    private void TextBoxFilter_TextChanged(object? sender, EventArgs e)
    {
        filterTimer.Stop();
        filterTimer.Start();
    }

    private void FilterTimer_Tick(object? sender, EventArgs e)
    {
        filterTimer.Stop();
        RefreshCategoryList();
        RefreshChannelList();
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
        e.Cancel = CurrentChannel == null;
        goToCategoryToolStripMenuItem.Visible = checkBoxSearchAllCategories.Checked;
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
            Clipboard.SetText(channel.Url);
        }
    }

    private void CopyChannelNameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentChannel is Channel channel)
        {
            Clipboard.SetText(channel.Name);
        }
    }

    private void GoToCategoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentChannel is not Channel channel)
        {
            return;
        }

        selectedCategory = channel.Category;
        textBoxCategoryFilter.Clear();
        textBoxChannelFilter.Clear();
        checkBoxSearchAllCategories.Checked = false;
        RefreshCategoryList();
        RefreshChannelList();

        int row = VisibleChannels.IndexOf(channel);
        if (row >= 0)
        {
            dataGridViewChannels.CurrentCell = dataGridViewChannels.Rows[row].Cells[columnChannelName.Index];
        }
    }

    #endregion

    #region Drag and drop

    private void EnableDragAndDrop(Control control)
    {
        // Text boxes keep their normal text drop behavior
        if (control is TextBoxBase)
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

    private static string? GetDroppedItem(IDataObject? data)
    {
        if (data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
        {
            return files[0];
        }

        if (data?.GetData(DataFormats.UnicodeText) is string text && AppSettings.IsUrl(text.Trim()))
        {
            return text.Trim();
        }

        return null;
    }

    private void Control_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = !isLoading && GetDroppedItem(e.Data) != null ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void Control_DragDrop(object? sender, DragEventArgs e)
    {
        if (GetDroppedItem(e.Data) is string item)
        {
            // Let the drag source return before showing any dialogs
            BeginInvoke(async () => await OpenAsync(item));
        }
    }

    #endregion

    #region Menu handlers

    private async void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Filter = PlaylistFileFilter,
            Title = "Open M3U Playlist",
        };

        if (openFileDialog.ShowDialog(this) == DialogResult.OK)
        {
            await OpenAsync(openFileDialog.FileName);
        }
    }

    private async void OpenURLToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var openUrlDialog = new OpenURLDialog(settings.RecentUrls);

        if (openUrlDialog.ShowDialog(this) == DialogResult.OK)
        {
            await OpenAsync(openUrlDialog.Url);
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
            "1. Open a playlist: File > Open File (Ctrl+O), File > Open URL (Ctrl+U), or drag and drop a file or link onto the window.\n" +
            "2. Pick the categories and channels to keep with the check boxes. Space toggles the selected rows.\n" +
            "   Select All / Clear All / Invert only affect the rows that match the current filter.\n" +
            "3. Use the search boxes to find categories and channels. Tick \"All categories\" to search every channel.\n" +
            "4. Double-click (or F2) a category or channel name to rename it. Right-click a category to reorder it.\n" +
            "5. Right-click a channel to play it in your default media player or copy its URL.\n" +
            "6. Save (Ctrl+S) or Save As (Ctrl+Shift+S). Only checked items are written.\n\n" +
            "Tip: Edit > Save Selection Profile remembers your choices. When your provider updates the playlist, " +
            "open the new one and use Edit > Apply Selection Profile instead of selecting everything again.",
            "How to Use",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        MessageBox.Show(this, $"{AppName} {version?.ToString(3)}\n\nFilter, rename and reorder M3U/M3U8 playlists.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion
}
