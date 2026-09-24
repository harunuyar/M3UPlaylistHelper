namespace M3UPlaylistHelper.UI;

partial class XtreamDialog
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
        labelServer = new Label();
        textBoxServer = new TextBox();
        labelUsername = new Label();
        textBoxUsername = new TextBox();
        labelPassword = new Label();
        textBoxPassword = new TextBox();
        checkBoxShowPassword = new CheckBox();
        labelOutput = new Label();
        comboBoxOutput = new ComboBox();
        checkBoxRememberPassword = new CheckBox();
        buttonOk = new Button();
        buttonCancel = new Button();
        SuspendLayout();
        // 
        // labelServer
        // 
        labelServer.AutoSize = true;
        labelServer.Location = new Point(12, 15);
        labelServer.Name = "labelServer";
        labelServer.Size = new Size(64, 25);
        labelServer.TabIndex = 0;
        labelServer.Text = "Server:";
        // 
        // textBoxServer
        // 
        textBoxServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxServer.Location = new Point(130, 12);
        textBoxServer.Name = "textBoxServer";
        textBoxServer.PlaceholderText = "http://example.com:8080";
        textBoxServer.Size = new Size(468, 31);
        textBoxServer.TabIndex = 1;
        // 
        // labelUsername
        // 
        labelUsername.AutoSize = true;
        labelUsername.Location = new Point(12, 55);
        labelUsername.Name = "labelUsername";
        labelUsername.Size = new Size(95, 25);
        labelUsername.TabIndex = 2;
        labelUsername.Text = "Username:";
        // 
        // textBoxUsername
        // 
        textBoxUsername.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxUsername.Location = new Point(130, 52);
        textBoxUsername.Name = "textBoxUsername";
        textBoxUsername.Size = new Size(468, 31);
        textBoxUsername.TabIndex = 3;
        // 
        // labelPassword
        // 
        labelPassword.AutoSize = true;
        labelPassword.Location = new Point(12, 95);
        labelPassword.Name = "labelPassword";
        labelPassword.Size = new Size(91, 25);
        labelPassword.TabIndex = 4;
        labelPassword.Text = "Password:";
        // 
        // textBoxPassword
        // 
        textBoxPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxPassword.Location = new Point(130, 92);
        textBoxPassword.Name = "textBoxPassword";
        textBoxPassword.Size = new Size(330, 31);
        textBoxPassword.TabIndex = 5;
        textBoxPassword.UseSystemPasswordChar = true;
        // 
        // checkBoxShowPassword
        // 
        checkBoxShowPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        checkBoxShowPassword.AutoSize = true;
        checkBoxShowPassword.Location = new Point(472, 94);
        checkBoxShowPassword.Name = "checkBoxShowPassword";
        checkBoxShowPassword.Size = new Size(79, 29);
        checkBoxShowPassword.TabIndex = 6;
        checkBoxShowPassword.Text = "Show";
        checkBoxShowPassword.UseVisualStyleBackColor = true;
        checkBoxShowPassword.CheckedChanged += CheckBoxShowPassword_CheckedChanged;
        // 
        // labelOutput
        // 
        labelOutput.AutoSize = true;
        labelOutput.Location = new Point(12, 135);
        labelOutput.Name = "labelOutput";
        labelOutput.Size = new Size(73, 25);
        labelOutput.TabIndex = 7;
        labelOutput.Text = "Format:";
        // 
        // comboBoxOutput
        // 
        comboBoxOutput.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxOutput.FormattingEnabled = true;
        comboBoxOutput.Items.AddRange(new object[] { "MPEG-TS (.ts)", "HLS (.m3u8)" });
        comboBoxOutput.Location = new Point(130, 132);
        comboBoxOutput.Name = "comboBoxOutput";
        comboBoxOutput.Size = new Size(220, 33);
        comboBoxOutput.TabIndex = 8;
        // 
        // checkBoxRememberPassword
        // 
        checkBoxRememberPassword.AutoSize = true;
        checkBoxRememberPassword.Location = new Point(130, 175);
        checkBoxRememberPassword.Name = "checkBoxRememberPassword";
        checkBoxRememberPassword.Size = new Size(207, 29);
        checkBoxRememberPassword.TabIndex = 9;
        checkBoxRememberPassword.Text = "Remember password";
        checkBoxRememberPassword.UseVisualStyleBackColor = true;
        // 
        // buttonOk
        // 
        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonOk.Location = new Point(344, 218);
        buttonOk.Name = "buttonOk";
        buttonOk.Size = new Size(124, 34);
        buttonOk.TabIndex = 10;
        buttonOk.Text = "Open";
        buttonOk.UseVisualStyleBackColor = true;
        buttonOk.Click += ButtonOk_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.DialogResult = DialogResult.Cancel;
        buttonCancel.Location = new Point(474, 218);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(124, 34);
        buttonCancel.TabIndex = 11;
        buttonCancel.Text = "Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        // 
        // XtreamDialog
        // 
        AcceptButton = buttonOk;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(610, 264);
        Controls.Add(buttonCancel);
        Controls.Add(buttonOk);
        Controls.Add(checkBoxRememberPassword);
        Controls.Add(comboBoxOutput);
        Controls.Add(labelOutput);
        Controls.Add(checkBoxShowPassword);
        Controls.Add(textBoxPassword);
        Controls.Add(labelPassword);
        Controls.Add(textBoxUsername);
        Controls.Add(labelUsername);
        Controls.Add(textBoxServer);
        Controls.Add(labelServer);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "XtreamDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Xtream Codes Login";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelServer;
    private TextBox textBoxServer;
    private Label labelUsername;
    private TextBox textBoxUsername;
    private Label labelPassword;
    private TextBox textBoxPassword;
    private CheckBox checkBoxShowPassword;
    private Label labelOutput;
    private ComboBox comboBoxOutput;
    private CheckBox checkBoxRememberPassword;
    private Button buttonOk;
    private Button buttonCancel;
}
