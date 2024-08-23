namespace M3UPlaylistHelper.UI;

partial class OpenURLDialog
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        labelUrl = new Label();
        textBoxUrl = new TextBox();
        buttonOk = new Button();
        SuspendLayout();
        // 
        // labelUrl
        // 
        labelUrl.AutoSize = true;
        labelUrl.Location = new Point(12, 9);
        labelUrl.Name = "labelUrl";
        labelUrl.Size = new Size(47, 25);
        labelUrl.TabIndex = 0;
        labelUrl.Text = "URL:";
        // 
        // textBoxUrl
        // 
        textBoxUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxUrl.Location = new Point(12, 37);
        textBoxUrl.Name = "textBoxUrl";
        textBoxUrl.Size = new Size(586, 31);
        textBoxUrl.TabIndex = 1;
        // 
        // buttonOk
        // 
        buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonOk.DialogResult = DialogResult.OK;
        buttonOk.Location = new Point(474, 74);
        buttonOk.Name = "buttonOk";
        buttonOk.Size = new Size(124, 34);
        buttonOk.TabIndex = 2;
        buttonOk.Text = "OK";
        buttonOk.UseVisualStyleBackColor = true;
        buttonOk.Click += buttonOk_Click;
        // 
        // OpenURLDialog
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(610, 118);
        Controls.Add(buttonOk);
        Controls.Add(textBoxUrl);
        Controls.Add(labelUrl);
        Name = "OpenURLDialog";
        Text = "Open M3U URL";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelUrl;
    private TextBox textBoxUrl;
    private Button buttonOk;
}