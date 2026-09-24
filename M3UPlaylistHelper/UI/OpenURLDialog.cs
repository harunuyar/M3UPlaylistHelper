namespace M3UPlaylistHelper.UI;

using System;
using System.Windows.Forms;

public partial class OpenURLDialog : Form
{
    public string Url => comboBoxUrl.Text.Trim();

    /// <param name="title">Window title, the dialog is used for playlists and EPG guides.</param>
    /// <param name="lastUrl">The URL entered last time, pre-filled so it can be opened again or corrected.</param>
    /// <param name="history">Earlier URLs, offered in the drop-down.</param>
    public OpenURLDialog(string title, string? lastUrl, IEnumerable<string> history)
    {
        InitializeComponent();
        ThemeManager.Apply(this);

        Text = title;
        comboBoxUrl.Items.AddRange(history.Distinct(StringComparer.OrdinalIgnoreCase).Cast<object>().ToArray());

        if (!string.IsNullOrWhiteSpace(lastUrl))
        {
            comboBoxUrl.Text = lastUrl;
        }
        else if (IsValidUrl(GetClipboardText()))
        {
            // Nothing entered before, the link is probably on the clipboard
            comboBoxUrl.Text = GetClipboardText();
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        comboBoxUrl.Focus();
        comboBoxUrl.SelectAll();
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
