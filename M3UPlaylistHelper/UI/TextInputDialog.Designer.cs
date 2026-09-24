namespace M3UPlaylistHelper.UI;

partial class TextInputDialog
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
        labelPrompt = new Label();
        textBoxValue = new TextBox();
        buttonOk = new Button();
        buttonCancel = new Button();
        SuspendLayout();
        // 
        // labelPrompt
        // 
        labelPrompt.AutoSize = true;
        labelPrompt.Location = new Point(12, 9);
        labelPrompt.Name = "labelPrompt";
        labelPrompt.Size = new Size(59, 25);
        labelPrompt.TabIndex = 0;
        labelPrompt.Text = "Name:";
        // 
        // textBoxValue
        // 
        textBoxValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxValue.Location = new Point(12, 37);
        textBoxValue.Name = "textBoxValue";
        textBoxValue.Size = new Size(486, 31);
        textBoxValue.TabIndex = 1;
        // 
        // buttonOk
        // 
        buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonOk.Location = new Point(244, 80);
        buttonOk.Name = "buttonOk";
        buttonOk.Size = new Size(124, 34);
        buttonOk.TabIndex = 2;
        buttonOk.Text = "OK";
        buttonOk.UseVisualStyleBackColor = true;
        buttonOk.Click += ButtonOk_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonCancel.DialogResult = DialogResult.Cancel;
        buttonCancel.Location = new Point(374, 80);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(124, 34);
        buttonCancel.TabIndex = 3;
        buttonCancel.Text = "Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        // 
        // TextInputDialog
        // 
        AcceptButton = buttonOk;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(510, 126);
        Controls.Add(buttonCancel);
        Controls.Add(buttonOk);
        Controls.Add(textBoxValue);
        Controls.Add(labelPrompt);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "TextInputDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Input";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelPrompt;
    private TextBox textBoxValue;
    private Button buttonOk;
    private Button buttonCancel;
}
