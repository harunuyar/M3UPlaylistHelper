namespace M3UPlaylistHelper.UI;

using M3UPlaylistHelper.Xtream;
using System;
using System.Windows.Forms;

/// <summary>
/// Asks for Xtream Codes login details (server, username, password) instead of a playlist URL.
/// </summary>
public partial class XtreamDialog : Form
{
    public XtreamAccount? Account { get; private set; }

    public bool RememberPassword => checkBoxRememberPassword.Checked;

    public XtreamDialog(string? server, string? username, string? password, string? output)
    {
        InitializeComponent();
        ThemeManager.Apply(this);

        textBoxServer.Text = server ?? string.Empty;
        textBoxUsername.Text = username ?? string.Empty;
        textBoxPassword.Text = password ?? string.Empty;
        checkBoxRememberPassword.Checked = !string.IsNullOrEmpty(password);
        comboBoxOutput.SelectedIndex = output == XtreamAccount.OutputHls ? 1 : 0;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // Start in the first empty box
        var firstEmpty = new[] { textBoxServer, textBoxUsername, textBoxPassword }.FirstOrDefault(t => t.TextLength == 0) ?? textBoxServer;
        firstEmpty.Focus();
    }

    private void CheckBoxShowPassword_CheckedChanged(object sender, EventArgs e)
    {
        textBoxPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
    }

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        var missing = new[] { (textBoxServer, "server"), (textBoxUsername, "username"), (textBoxPassword, "password") }
            .FirstOrDefault(field => string.IsNullOrWhiteSpace(field.Item1.Text));

        if (missing.Item1 != null)
        {
            MessageBox.Show(this, $"Please enter the {missing.Item2}.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            missing.Item1.Focus();
            return;
        }

        try
        {
            var output = comboBoxOutput.SelectedIndex == 1 ? XtreamAccount.OutputHls : XtreamAccount.OutputTs;
            Account = new XtreamAccount(textBoxServer.Text, textBoxUsername.Text, textBoxPassword.Text, output);
        }
        catch (FormatException ex)
        {
            MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            textBoxServer.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
