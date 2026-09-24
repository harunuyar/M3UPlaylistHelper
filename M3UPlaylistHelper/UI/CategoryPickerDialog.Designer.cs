namespace M3UPlaylistHelper.UI;

partial class CategoryPickerDialog
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
        textBoxSearch = new TextBox();
        listBoxCategories = new ListBox();
        labelHint = new Label();
        buttonOk = new Button();
        buttonCancel = new Button();
        SuspendLayout();
        //
        // textBoxSearch
        //
        textBoxSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxSearch.Location = new Point(12, 12);
        textBoxSearch.Name = "textBoxSearch";
        textBoxSearch.PlaceholderText = "Search or type a new category name...";
        textBoxSearch.Size = new Size(536, 31);
        textBoxSearch.TabIndex = 0;
        textBoxSearch.TextChanged += TextBoxSearch_TextChanged;
        textBoxSearch.KeyDown += TextBoxSearch_KeyDown;
        //
        // listBoxCategories
        //
        listBoxCategories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        listBoxCategories.FormattingEnabled = true;
        listBoxCategories.IntegralHeight = false;
        listBoxCategories.ItemHeight = 25;
        listBoxCategories.Location = new Point(12, 52);
        listBoxCategories.Name = "listBoxCategories";
        listBoxCategories.Size = new Size(536, 400);
        listBoxCategories.TabIndex = 1;
        listBoxCategories.SelectedIndexChanged += ListBoxCategories_SelectedIndexChanged;
        listBoxCategories.DoubleClick += ListBoxCategories_DoubleClick;
        //
        // labelHint
        //
        labelHint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        labelHint.AutoEllipsis = true;
        labelHint.Location = new Point(12, 462);
        labelHint.Name = "labelHint";
        labelHint.Size = new Size(536, 25);
        labelHint.TabIndex = 2;
        //
        // buttonOk
        //
        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonOk.Location = new Point(294, 496);
        buttonOk.Name = "buttonOk";
        buttonOk.Size = new Size(124, 34);
        buttonOk.TabIndex = 3;
        buttonOk.Text = "Move";
        buttonOk.UseVisualStyleBackColor = true;
        buttonOk.Click += ButtonOk_Click;
        //
        // buttonCancel
        //
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.DialogResult = DialogResult.Cancel;
        buttonCancel.Location = new Point(424, 496);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(124, 34);
        buttonCancel.TabIndex = 4;
        buttonCancel.Text = "Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        //
        // CategoryPickerDialog
        //
        AcceptButton = buttonOk;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = buttonCancel;
        ClientSize = new Size(560, 542);
        Controls.Add(buttonCancel);
        Controls.Add(buttonOk);
        Controls.Add(labelHint);
        Controls.Add(listBoxCategories);
        Controls.Add(textBoxSearch);
        MinimizeBox = false;
        MinimumSize = new Size(400, 350);
        Name = "CategoryPickerDialog";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Move to Category";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox textBoxSearch;
    private ListBox listBoxCategories;
    private Label labelHint;
    private Button buttonOk;
    private Button buttonCancel;
}
