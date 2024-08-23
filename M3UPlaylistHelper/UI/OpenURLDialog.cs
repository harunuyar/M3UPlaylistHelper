namespace M3UPlaylistHelper.UI;

using System;
using System.Windows.Forms;

public partial class OpenURLDialog : Form
{
    public string Url => textBoxUrl.Text;

    public OpenURLDialog()
    {
        InitializeComponent();
    }

    private void buttonOk_Click(object sender, EventArgs e)
    {
        Close();
    }
}
