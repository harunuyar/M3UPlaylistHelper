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
        comboBoxUrl = new ComboBox();
        buttonOk = new Button();
        buttonCancel = new Button();
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
        // comboBoxUrl
        // 
        comboBoxUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboBoxUrl.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        comboBoxUrl.AutoCompleteSource = AutoCompleteSource.ListItems;
        comboBoxUrl.FormattingEnabled = true;
        comboBoxUrl.Location = new Point(12, 37);
        comboBoxUrl.Name = "comboBoxUrl";
        comboBoxUrl.Size = new Size(786, 33);
        comboBoxUrl.TabIndex = 1;
        // 
        // buttonOk
        // 
        buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonOk.Location = new Point(544, 80);
        buttonOk.Name = "buttonOk";
        buttonOk.Size = new Size(124, 34);
        buttonOk.TabIndex = 2;
        buttonOk.Text = "Open";
        buttonOk.UseVisualStyleBackColor = true;
        buttonOk.Click += ButtonOk_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonCancel.DialogResult = DialogResult.Cancel;
        buttonCancel.Location = new Point(674, 80);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(124, 34);
        buttonCancel.TabIndex = 3;
        buttonCancel.Text = "Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        // 
        // OpenURLDialog
        // 
        AcceptButton = buttonOk;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(810, 126);
        Controls.Add(buttonCancel);
        Controls.Add(buttonOk);
        Controls.Add(comboBoxUrl);
        Controls.Add(labelUrl);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OpenURLDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Open M3U URL";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelUrl;
    private ComboBox comboBoxUrl;
    private Button buttonOk;
    private Button buttonCancel;
}
