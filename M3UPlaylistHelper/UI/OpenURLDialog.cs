namespace M3UPlaylistHelper.UI;

using System;
using System.Windows.Forms;

public partial class OpenURLDialog : Form
{
    public string Url => comboBoxUrl.Text.Trim();

    public OpenURLDialog(IEnumerable<string> recentUrls)
    {
        InitializeComponent();

        comboBoxUrl.Items.AddRange(recentUrls.Cast<object>().ToArray());

        // Pre-fill with a URL from the clipboard, it is usually where the link comes from
        var clipboardText = GetClipboardText();
        if (IsValidUrl(clipboardText))
        {
            comboBoxUrl.Text = clipboardText;
        }
        else if (comboBoxUrl.Items.Count > 0)
        {
            comboBoxUrl.SelectedIndex = 0;
        }
    }

    private static string GetClipboardText()
    {
        try
        {
            return Clipboard.ContainsText() ? Clipboard.GetText().Trim() : string.Empty;
        }
        catch (Exception)
        {
            // The clipboard can be locked by another application
            return string.Empty;
        }
    }

    private static bool IsValidUrl(string text) =>
        Uri.TryCreate(text, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        if (!IsValidUrl(Url))
        {
            MessageBox.Show(this, "Please enter a valid http:// or https:// URL.", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            comboBoxUrl.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
