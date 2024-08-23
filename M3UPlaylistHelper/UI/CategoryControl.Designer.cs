namespace M3UPlaylistHelper.UI;

partial class CategoryControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        checkBox = new CheckBox();
        label = new Label();
        SuspendLayout();
        // 
        // checkBox
        // 
        checkBox.AutoSize = true;
        checkBox.Location = new Point(6, 14);
        checkBox.Name = "checkBox";
        checkBox.Size = new Size(22, 21);
        checkBox.TabIndex = 0;
        checkBox.UseVisualStyleBackColor = true;
        // 
        // label
        // 
        label.AutoSize = true;
        label.Location = new Point(31, 12);
        label.Name = "label";
        label.Size = new Size(49, 25);
        label.TabIndex = 1;
        label.Text = "label";
        // 
        // CategoryControl
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(label);
        Controls.Add(checkBox);
        Name = "CategoryControl";
        Size = new Size(544, 52);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private CheckBox checkBox;
    private Label label;
}
