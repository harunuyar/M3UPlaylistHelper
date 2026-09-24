namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Model;
using M3UPlaylistHelper.Tools;
using System;
using System.Windows.Forms;

/// <summary>
/// Picks the category to move channels to. Searchable, because IPTV playlists often have thousands of categories;
/// typing a name that doesn't exist creates a new category.
/// </summary>
public partial class CategoryPickerDialog : Form
{
    private readonly IReadOnlyList<Category> categories;

    /// <summary>
    /// The chosen existing category, or null when <see cref="NewCategoryName"/> is set.
    /// </summary>
    public Category? SelectedCategory { get; private set; }

    public string? NewCategoryName { get; private set; }

    public CategoryPickerDialog(IReadOnlyList<Category> categories, int channelCount)
    {
        InitializeComponent();
        ThemeManager.Apply(this);

        this.categories = categories;
        Text = channelCount == 1 ? "Move Channel to Category" : $"Move {channelCount:N0} Channels to Category";
        UpdateList();
    }

    /// <summary>
    /// Shows the title in the list box without a wrapper class per item.
    /// </summary>
    private sealed record Item(Category Category)
    {
        public override string ToString() => Category.Title;
    }

    private string SearchText => textBoxSearch.Text.Trim();

    private Category? ExactMatch =>
        SearchText.Length == 0 ? null : categories.FirstOrDefault(c => string.Equals(c.Title, SearchText, StringComparison.CurrentCultureIgnoreCase));

    private void UpdateList()
    {
        var search = SearchText;
        var matches = search.Length == 0 ? categories : categories.Where(c => PlaylistTools.Matches(c.Title, search)).ToList();

        listBoxCategories.BeginUpdate();
        listBoxCategories.Items.Clear();
        listBoxCategories.Items.AddRange(matches.Select(c => (object)new Item(c)).ToArray());

        // Preselect the exact match, else the first match, so Enter moves to what the user is looking at.
        // Creating a new category is a separate, explicit button.
        if (listBoxCategories.Items.Count > 0)
        {
            var exact = ExactMatch;
            int index = exact == null ? 0 : Math.Max(0, matches.ToList().IndexOf(exact));
            listBoxCategories.SelectedIndex = search.Length > 0 ? index : -1;
        }

        listBoxCategories.EndUpdate();
        UpdateHint();
    }

    private void UpdateHint()
    {
        bool canCreate = SearchText.Length > 0 && ExactMatch == null;
        buttonCreate.Enabled = canCreate;
        buttonCreate.Text = canCreate ? $"New \"{Shorten(SearchText)}\"" : "New Category";

        if (listBoxCategories.SelectedItem is Item item)
        {
            labelHint.Text = $"Move to \"{item.Category.Title}\"";
            buttonOk.Enabled = true;
        }
        else if (canCreate)
        {
            labelHint.Text = $"No category matches, press Enter to create \"{SearchText}\"";
            buttonOk.Enabled = true;
        }
        else
        {
            labelHint.Text = "Type to search, or type a new name and click New to create a category";
            buttonOk.Enabled = false;
        }
    }

    private static string Shorten(string text) => text.Length <= 20 ? text : text[..19] + "\u2026";

    private void ButtonCreate_Click(object sender, EventArgs e)
    {
        if (SearchText.Length > 0 && ExactMatch == null)
        {
            NewCategoryName = SearchText;
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void TextBoxSearch_TextChanged(object sender, EventArgs e) => UpdateList();

    private void ListBoxCategories_SelectedIndexChanged(object sender, EventArgs e) => UpdateHint();

    private void ListBoxCategories_DoubleClick(object sender, EventArgs e)
    {
        if (listBoxCategories.SelectedItem != null)
        {
            ButtonOk_Click(sender, e);
        }
    }

    private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
    {
        // Arrow keys move through the list while typing
        if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) && listBoxCategories.Items.Count > 0)
        {
            int index = listBoxCategories.SelectedIndex + (e.KeyCode == Keys.Down ? 1 : -1);
            listBoxCategories.SelectedIndex = Math.Clamp(index, 0, listBoxCategories.Items.Count - 1);
            e.SuppressKeyPress = true;
        }
    }

    private void ButtonOk_Click(object? sender, EventArgs e)
    {
        if (listBoxCategories.SelectedItem is Item item)
        {
            SelectedCategory = item.Category;
        }
        else if (ExactMatch is Category exact)
        {
            SelectedCategory = exact;
        }
        else if (SearchText.Length > 0)
        {
            NewCategoryName = SearchText;
        }
        else
        {
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
