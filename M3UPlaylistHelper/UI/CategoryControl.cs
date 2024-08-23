namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Model;
using System.Windows.Forms;

public partial class CategoryControl : UserControl
{
    public Category Category { get; }

    public Label Label => label;

    public CheckBox CheckBox => checkBox;

    public CategoryControl(Category category)
    {
        Category = category;
        InitializeComponent();
        checkBox.Checked = category.IsIncluded;
        checkBox.CheckedChanged += (sender, e) => category.IsIncluded = checkBox.Checked;
        label.Text = category.Title;
    }
}
