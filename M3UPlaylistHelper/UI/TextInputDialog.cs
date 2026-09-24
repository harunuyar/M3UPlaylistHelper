namespace M3UPlaylistHelper.UI;

using System;
using System.Windows.Forms;

/// <summary>
/// Asks for a single line of text, e.g. the name of a new category.
/// </summary>
public partial class TextInputDialog : Form
{
    public string Value => textBoxValue.Text.Trim();

    public TextInputDialog(string title, string prompt, string initialValue = "")
    {
        InitializeComponent();
        ThemeManager.Apply(this);

        Text = title;
        labelPrompt.Text = prompt;
        textBoxValue.Text = initialValue;
    }

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        if (Value.Length == 0)
        {
            textBoxValue.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
