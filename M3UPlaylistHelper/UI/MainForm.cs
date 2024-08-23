namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Model;
using M3UPlaylistHelper.Parser;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private readonly List<Category> categories = [];

    private string? CurrentFilePath { get; set; }

    public MainForm()
    {
        InitializeComponent();
        SetTitle();
    }

    public void SetTitle(string? title = null)
    {
        if (title == null)
        {
            Text = "M3U Playlist Helper";
        }
        else
        {
            Text = $"M3U Playlist Helper - {title}";
        }
    }

    private async void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "M3U Playlist|*.m3u",
            Title = "Open M3U Playlist"
        };

        DialogResult dialogResult = openFileDialog.ShowDialog();

        if (dialogResult != DialogResult.OK)
        {
            return;
        }

        Invoke(new Action(() => Cursor = Cursors.WaitCursor));

        Clear();

        try
        {
            CurrentFilePath = openFileDialog.FileName;

            // Parse M3U playlist
            categories.AddRange(await M3UParser.ParseFileAsync(CurrentFilePath, CancellationToken.None));

            // Set application title
            SetTitle(CurrentFilePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        finally
        {
            DisplayCategories();

            Invoke(new Action(() => Cursor = Cursors.Default));
        }
    }

    private async void openURLToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var openUrlDialog = new OpenURLDialog()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen
        };

        DialogResult dialogResult = openUrlDialog.ShowDialog();

        if (dialogResult != DialogResult.OK)
        {
            return;
        }

        Invoke(new Action(() => Cursor = Cursors.WaitCursor));

        Clear();

        // Set application title
        SetTitle();

        try
        {
            // Parse M3U playlist
            categories.AddRange(await M3UParser.ParseM3ULink(openUrlDialog.Url, CancellationToken.None));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        finally
        {
            DisplayCategories();

            Invoke(new Action(() => Cursor = Cursors.Default));
        }
    }

    private void DisplayCategories()
    {
        dataGridViewChannels.DataSource = null;
        dataGridViewCategories.DataSource = null;
        dataGridViewCategories.Refresh();

        labelSelectedCategory.Text = string.Empty;
        dataGridViewCategories.DataSource = categories;
        dataGridViewCategories.Columns["IsIncluded"].HeaderText = "Include";
        dataGridViewCategories.Columns["IsIncluded"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dataGridViewCategories.Columns["IsIncluded"].DisplayIndex = 0;
        dataGridViewCategories.Columns["Title"].HeaderText = "Title";
        dataGridViewCategories.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dataGridViewCategories.Columns["Title"].DisplayIndex = 1;
        dataGridViewCategories.Refresh();
        dataGridViewChannels.Refresh();
    }

    private void DataGridViewCategories_Click(object? sender, EventArgs e)
    {
        if (dataGridViewCategories?.CurrentRow?.DataBoundItem is Category category)
        {
            SelectCategory(category);
        }
    }

    private void Clear()
    {
        CurrentFilePath = null;
        SetTitle();
        categories.Clear();
        dataGridViewChannels.DataSource = null;
        dataGridViewCategories.DataSource = null;
        labelSelectedCategory.Text = string.Empty;
    }

    private void SelectCategory(Category category)
    {
        labelSelectedCategory.Text = category.Title;

        dataGridViewChannels.DataSource = category.Channels;
        dataGridViewChannels.Columns["Category"].Visible = false;
        dataGridViewChannels.Columns["TvgId"].Visible = false;
        dataGridViewChannels.Columns["TvgName"].Visible = false;
        dataGridViewChannels.Columns["LogoUrl"].Visible = false;
        dataGridViewChannels.Columns["IsIncluded"].HeaderText = "Include";
        dataGridViewChannels.Columns["IsIncluded"].DisplayIndex = 0;
        dataGridViewChannels.Columns["IsIncluded"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dataGridViewChannels.Columns["Logo"].HeaderText = "Logo";
        dataGridViewChannels.Columns["Logo"].DisplayIndex = 1;
        dataGridViewChannels.Columns["Logo"].Width = Channel.LogoWidth + 10;
        dataGridViewChannels.Columns["Name"].HeaderText = "Name";
        dataGridViewChannels.Columns["Name"].DisplayIndex = 2;
        dataGridViewChannels.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dataGridViewChannels.Columns["Url"].HeaderText = "URL";
        dataGridViewChannels.Columns["Url"].DisplayIndex = 3;
        dataGridViewChannels.Columns["Url"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        Task.Run(async () =>
        {
            await Task.WhenAll(category.Channels.Select(c => c.LoadLogoAsync()));
            Invoke(new Action(() => dataGridViewChannels.Refresh()));
        });
    }

    private async void SaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentFilePath == null)
        {
            SaveAsToolStripMenuItem_Click(sender, e);
        }
        else
        {
            await M3UParser.CreateM3UFileAsync(categories, CurrentFilePath, CancellationToken.None);
        }

        categories.RemoveAll(c => !c.IsIncluded || !c.Channels.Any(ch => ch.IsIncluded));
        categories.ForEach(c => c.Channels.RemoveAll(ch => !ch.IsIncluded));
        DisplayCategories();
    }

    private async void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "M3U Playlist|*.m3u",
            Title = "Save M3U Playlist"
        };

        DialogResult dialogResult = saveFileDialog.ShowDialog();

        if (dialogResult != DialogResult.OK)
        {
            return;
        }

        await M3UParser.CreateM3UFileAsync(categories, saveFileDialog.FileName, CancellationToken.None);

        if (CurrentFilePath == null)
        {
            CurrentFilePath = saveFileDialog.FileName;
            SetTitle(CurrentFilePath);
        }
    }

    private void buttonSelectAllCategories_Click(object sender, EventArgs e)
    {
        foreach (Category category in categories)
        {
            category.IsIncluded = true;
        }

        dataGridViewCategories.Refresh();
    }

    private void ButtonClearAllCategories_Click(object sender, EventArgs e)
    {
        foreach (Category category in categories)
        {
            category.IsIncluded = false;
        }

        dataGridViewCategories.Refresh();
    }

    private void buttonSelectAllChannels_Click(object sender, EventArgs e)
    {
        if (dataGridViewChannels.DataSource is IEnumerable<Channel> channels)
        {
            foreach (Channel channel in channels)
            {
                channel.IsIncluded = true;
            }

            dataGridViewChannels.Refresh();
        }
    }

    private void buttonClearAllChannels_Click(object sender, EventArgs e)
    {
        if (dataGridViewChannels.DataSource is IEnumerable<Channel> channels)
        {
            foreach (Channel channel in channels)
            {
                channel.IsIncluded = false;
            }

            dataGridViewChannels.Refresh();
        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
}
