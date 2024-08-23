namespace M3UPlaylistHelper.UI;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        menuStrip = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        openFileToolStripMenuItem = new ToolStripMenuItem();
        openURLToolStripMenuItem = new ToolStripMenuItem();
        saveToolStripMenuItem = new ToolStripMenuItem();
        saveAsToolStripMenuItem = new ToolStripMenuItem();
        exitToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem = new ToolStripMenuItem();
        mainPanel = new Panel();
        splitContainer = new SplitContainer();
        buttonClearAllCategories = new Button();
        labelChannels = new Label();
        buttonSelectAllCategories = new Button();
        dataGridViewCategories = new DataGridView();
        buttonClearAllChannels = new Button();
        buttonSelectAllChannels = new Button();
        labelSelectedCategory = new Label();
        dataGridViewChannels = new DataGridView();
        checkBoxDownloadLogos = new CheckBox();
        menuStrip.SuspendLayout();
        mainPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewCategories).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dataGridViewChannels).BeginInit();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.ImageScalingSize = new Size(24, 24);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1715, 33);
        menuStrip.TabIndex = 0;
        menuStrip.Text = "menuStrip";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, openURLToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, exitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(54, 29);
        fileToolStripMenuItem.Text = "File";
        // 
        // openFileToolStripMenuItem
        // 
        openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
        openFileToolStripMenuItem.Size = new Size(194, 34);
        openFileToolStripMenuItem.Text = "Open File";
        openFileToolStripMenuItem.Click += OpenFileToolStripMenuItem_Click;
        // 
        // openURLToolStripMenuItem
        // 
        openURLToolStripMenuItem.Name = "openURLToolStripMenuItem";
        openURLToolStripMenuItem.Size = new Size(194, 34);
        openURLToolStripMenuItem.Text = "Open URL";
        openURLToolStripMenuItem.Click += openURLToolStripMenuItem_Click;
        // 
        // saveToolStripMenuItem
        // 
        saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        saveToolStripMenuItem.Size = new Size(194, 34);
        saveToolStripMenuItem.Text = "Save";
        saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
        // 
        // saveAsToolStripMenuItem
        // 
        saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
        saveAsToolStripMenuItem.Size = new Size(194, 34);
        saveAsToolStripMenuItem.Text = "Save As...";
        saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(194, 34);
        exitToolStripMenuItem.Text = "Exit";
        exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(65, 29);
        helpToolStripMenuItem.Text = "Help";
        helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
        // 
        // mainPanel
        // 
        mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        mainPanel.Controls.Add(splitContainer);
        mainPanel.Location = new Point(12, 36);
        mainPanel.Name = "mainPanel";
        mainPanel.Size = new Size(1691, 919);
        mainPanel.TabIndex = 1;
        // 
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.FixedPanel = FixedPanel.Panel1;
        splitContainer.Location = new Point(0, 0);
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(buttonClearAllCategories);
        splitContainer.Panel1.Controls.Add(labelChannels);
        splitContainer.Panel1.Controls.Add(buttonSelectAllCategories);
        splitContainer.Panel1.Controls.Add(dataGridViewCategories);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(checkBoxDownloadLogos);
        splitContainer.Panel2.Controls.Add(buttonClearAllChannels);
        splitContainer.Panel2.Controls.Add(buttonSelectAllChannels);
        splitContainer.Panel2.Controls.Add(labelSelectedCategory);
        splitContainer.Panel2.Controls.Add(dataGridViewChannels);
        splitContainer.Size = new Size(1691, 919);
        splitContainer.SplitterDistance = 450;
        splitContainer.TabIndex = 0;
        // 
        // buttonClearAllCategories
        // 
        buttonClearAllCategories.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonClearAllCategories.Location = new Point(132, 885);
        buttonClearAllCategories.Name = "buttonClearAllCategories";
        buttonClearAllCategories.Size = new Size(123, 34);
        buttonClearAllCategories.TabIndex = 2;
        buttonClearAllCategories.Text = "Clear All";
        buttonClearAllCategories.UseVisualStyleBackColor = true;
        buttonClearAllCategories.Click += ButtonClearAllCategories_Click;
        // 
        // labelChannels
        // 
        labelChannels.AutoSize = true;
        labelChannels.Location = new Point(3, 0);
        labelChannels.Name = "labelChannels";
        labelChannels.Size = new Size(96, 25);
        labelChannels.TabIndex = 2;
        labelChannels.Text = "Categories";
        // 
        // buttonSelectAllCategories
        // 
        buttonSelectAllCategories.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonSelectAllCategories.Location = new Point(3, 885);
        buttonSelectAllCategories.Name = "buttonSelectAllCategories";
        buttonSelectAllCategories.Size = new Size(123, 34);
        buttonSelectAllCategories.TabIndex = 1;
        buttonSelectAllCategories.Text = "Select All";
        buttonSelectAllCategories.UseVisualStyleBackColor = true;
        buttonSelectAllCategories.Click += buttonSelectAllCategories_Click;
        // 
        // dataGridViewCategories
        // 
        dataGridViewCategories.AllowUserToOrderColumns = true;
        dataGridViewCategories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewCategories.BackgroundColor = SystemColors.Window;
        dataGridViewCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCategories.GridColor = SystemColors.Control;
        dataGridViewCategories.Location = new Point(0, 28);
        dataGridViewCategories.Name = "dataGridViewCategories";
        dataGridViewCategories.RowHeadersVisible = false;
        dataGridViewCategories.RowHeadersWidth = 62;
        dataGridViewCategories.Size = new Size(450, 851);
        dataGridViewCategories.TabIndex = 1;
        dataGridViewCategories.Click += DataGridViewCategories_Click;
        // 
        // buttonClearAllChannels
        // 
        buttonClearAllChannels.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonClearAllChannels.Location = new Point(132, 885);
        buttonClearAllChannels.Name = "buttonClearAllChannels";
        buttonClearAllChannels.Size = new Size(123, 34);
        buttonClearAllChannels.TabIndex = 4;
        buttonClearAllChannels.Text = "Clear All";
        buttonClearAllChannels.UseVisualStyleBackColor = true;
        buttonClearAllChannels.Click += buttonClearAllChannels_Click;
        // 
        // buttonSelectAllChannels
        // 
        buttonSelectAllChannels.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonSelectAllChannels.Location = new Point(3, 885);
        buttonSelectAllChannels.Name = "buttonSelectAllChannels";
        buttonSelectAllChannels.Size = new Size(123, 34);
        buttonSelectAllChannels.TabIndex = 3;
        buttonSelectAllChannels.Text = "Select All";
        buttonSelectAllChannels.UseVisualStyleBackColor = true;
        buttonSelectAllChannels.Click += buttonSelectAllChannels_Click;
        // 
        // labelSelectedCategory
        // 
        labelSelectedCategory.AutoSize = true;
        labelSelectedCategory.Location = new Point(3, 0);
        labelSelectedCategory.Name = "labelSelectedCategory";
        labelSelectedCategory.Size = new Size(0, 25);
        labelSelectedCategory.TabIndex = 3;
        // 
        // dataGridViewChannels
        // 
        dataGridViewChannels.AllowUserToOrderColumns = true;
        dataGridViewChannels.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewChannels.BackgroundColor = SystemColors.Window;
        dataGridViewChannels.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewChannels.GridColor = SystemColors.Control;
        dataGridViewChannels.Location = new Point(0, 28);
        dataGridViewChannels.Name = "dataGridViewChannels";
        dataGridViewChannels.RowHeadersVisible = false;
        dataGridViewChannels.RowHeadersWidth = 62;
        dataGridViewChannels.Size = new Size(1237, 851);
        dataGridViewChannels.TabIndex = 0;
        // 
        // checkBoxDownloadLogos
        // 
        checkBoxDownloadLogos.AutoSize = true;
        checkBoxDownloadLogos.Checked = true;
        checkBoxDownloadLogos.CheckState = CheckState.Checked;
        checkBoxDownloadLogos.Location = new Point(1060, 889);
        checkBoxDownloadLogos.Name = "checkBoxDownloadLogos";
        checkBoxDownloadLogos.Size = new Size(174, 29);
        checkBoxDownloadLogos.TabIndex = 5;
        checkBoxDownloadLogos.Text = "Download Logos";
        checkBoxDownloadLogos.UseVisualStyleBackColor = true;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1715, 967);
        Controls.Add(mainPanel);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        Name = "MainForm";
        Text = "Form1";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        mainPanel.ResumeLayout(false);
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel1.PerformLayout();
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewCategories).EndInit();
        ((System.ComponentModel.ISupportInitialize)dataGridViewChannels).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openFileToolStripMenuItem;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private Panel mainPanel;
    private SplitContainer splitContainer;
    private DataGridView dataGridViewChannels;
    private Button buttonSelectAllCategories;
    private Button buttonClearAllCategories;
    private DataGridView dataGridViewCategories;
    private Label labelChannels;
    private Label labelSelectedCategory;
    private Button buttonClearAllChannels;
    private Button buttonSelectAllChannels;
    private ToolStripMenuItem openURLToolStripMenuItem;
    private CheckBox checkBoxDownloadLogos;
}
