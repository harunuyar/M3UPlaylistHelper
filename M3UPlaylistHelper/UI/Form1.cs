namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Model;
using M3UPlaylistHelper.Parser;

public partial class Form1 : Form
{
    private readonly List<Category> categories = [];

    private string? CurrentFilePath { get; set; }

    public Form1()
    {
        InitializeComponent();
        Text = "M3U Playlist Helper";
    }

    private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Open file dialog

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

        categories.Clear();
        dataGridView.DataSource = null;
        CurrentFilePath = openFileDialog.FileName;

        // Set application title
        Text = $"M3U Playlist Helper - {CurrentFilePath}";

        // Parse M3U playlist
        categories.AddRange(await M3UParser.ParseAsync(CurrentFilePath, CancellationToken.None));

        DisplayCategories();

        Invoke(new Action(() => Cursor = Cursors.Default));
    }

    private void DisplayCategories()
    {
        flowLayoutPanelCategories.Controls.Clear();
        foreach (var category in categories)
        {
            var categoryControl = new CategoryControl(category);
            categoryControl.Label.Click += (sender, e) => SelectCategory(category);
            flowLayoutPanelCategories.Controls.Add(categoryControl);
        }
    }

    private void SelectCategory(Category category)
    {
        dataGridView.DataSource = category.Channels;
        dataGridView.Columns["Category"].Visible = false;
        dataGridView.Columns["TvgId"].Visible = false;
        dataGridView.Columns["TvgName"].Visible = false;
        dataGridView.Columns["LogoUrl"].Visible = false;
        dataGridView.Columns["IsIncluded"].HeaderText = "Include";
        dataGridView.Columns["IsIncluded"].DisplayIndex = 0;
        dataGridView.Columns["IsIncluded"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dataGridView.Columns["Name"].HeaderText = "Name";
        dataGridView.Columns["Name"].DisplayIndex = 1;
        dataGridView.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dataGridView.Columns["Url"].HeaderText = "URL";
        dataGridView.Columns["Url"].DisplayIndex = 2;
        dataGridView.Columns["Url"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    }

    private async void SaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentFilePath == null)
        {
            MessageBox.Show("Please open a file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await M3UParser.CreateM3UFileAsync(categories, CurrentFilePath, CancellationToken.None);
    }

    private async void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (CurrentFilePath == null)
        {
            MessageBox.Show("Please open a file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

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
    }

    private void buttonSelectAll_Click(object sender, EventArgs e)
    {
        foreach (CategoryControl categoryControl in flowLayoutPanelCategories.Controls)
        {
            categoryControl.CheckBox.Checked = true;
        }
    }

    private void ButtonClearAll_Click(object sender, EventArgs e)
    {
        foreach (CategoryControl categoryControl in flowLayoutPanelCategories.Controls)
        {
            categoryControl.CheckBox.Checked = false;
        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
}
