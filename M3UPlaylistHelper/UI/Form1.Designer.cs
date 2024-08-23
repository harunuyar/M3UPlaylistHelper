namespace M3UPlaylistHelper.UI;

partial class Form1
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
        menuStrip1 = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        openToolStripMenuItem = new ToolStripMenuItem();
        saveToolStripMenuItem = new ToolStripMenuItem();
        saveAsToolStripMenuItem = new ToolStripMenuItem();
        exitToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem = new ToolStripMenuItem();
        panel1 = new Panel();
        buttonClearAll = new Button();
        buttonSelectAll = new Button();
        splitContainer1 = new SplitContainer();
        flowLayoutPanelCategories = new FlowLayoutPanel();
        dataGridView = new DataGridView();
        menuStrip1.SuspendLayout();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(24, 24);
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(1351, 33);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, exitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(54, 29);
        fileToolStripMenuItem.Text = "File";
        // 
        // openToolStripMenuItem
        // 
        openToolStripMenuItem.Name = "openToolStripMenuItem";
        openToolStripMenuItem.Size = new Size(188, 34);
        openToolStripMenuItem.Text = "Open";
        openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
        // 
        // saveToolStripMenuItem
        // 
        saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        saveToolStripMenuItem.Size = new Size(188, 34);
        saveToolStripMenuItem.Text = "Save";
        saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
        // 
        // saveAsToolStripMenuItem
        // 
        saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
        saveAsToolStripMenuItem.Size = new Size(188, 34);
        saveAsToolStripMenuItem.Text = "Save As...";
        saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(188, 34);
        exitToolStripMenuItem.Text = "Exit";
        exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(65, 29);
        helpToolStripMenuItem.Text = "Help";
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        panel1.Controls.Add(buttonClearAll);
        panel1.Controls.Add(buttonSelectAll);
        panel1.Controls.Add(splitContainer1);
        panel1.Location = new Point(12, 36);
        panel1.Name = "panel1";
        panel1.Size = new Size(1327, 919);
        panel1.TabIndex = 1;
        // 
        // buttonClearAll
        // 
        buttonClearAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonClearAll.Location = new Point(132, 882);
        buttonClearAll.Name = "buttonClearAll";
        buttonClearAll.Size = new Size(123, 34);
        buttonClearAll.TabIndex = 2;
        buttonClearAll.Text = "Clear All";
        buttonClearAll.UseVisualStyleBackColor = true;
        buttonClearAll.Click += ButtonClearAll_Click;
        // 
        // buttonSelectAll
        // 
        buttonSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonSelectAll.Location = new Point(3, 882);
        buttonSelectAll.Name = "buttonSelectAll";
        buttonSelectAll.Size = new Size(123, 34);
        buttonSelectAll.TabIndex = 1;
        buttonSelectAll.Text = "Select All";
        buttonSelectAll.UseVisualStyleBackColor = true;
        buttonSelectAll.Click += buttonSelectAll_Click;
        // 
        // splitContainer1
        // 
        splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        splitContainer1.FixedPanel = FixedPanel.Panel1;
        splitContainer1.Location = new Point(0, 0);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(flowLayoutPanelCategories);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.Controls.Add(dataGridView);
        splitContainer1.Size = new Size(1327, 876);
        splitContainer1.SplitterDistance = 450;
        splitContainer1.TabIndex = 0;
        // 
        // flowLayoutPanelCategories
        // 
        flowLayoutPanelCategories.AutoScroll = true;
        flowLayoutPanelCategories.BackColor = SystemColors.Window;
        flowLayoutPanelCategories.Dock = DockStyle.Fill;
        flowLayoutPanelCategories.Location = new Point(0, 0);
        flowLayoutPanelCategories.Name = "flowLayoutPanelCategories";
        flowLayoutPanelCategories.Size = new Size(450, 876);
        flowLayoutPanelCategories.TabIndex = 0;
        // 
        // dataGridView
        // 
        dataGridView.AllowUserToOrderColumns = true;
        dataGridView.BackgroundColor = SystemColors.Window;
        dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView.Dock = DockStyle.Fill;
        dataGridView.GridColor = SystemColors.Control;
        dataGridView.Location = new Point(0, 0);
        dataGridView.Name = "dataGridView";
        dataGridView.RowHeadersWidth = 62;
        dataGridView.Size = new Size(873, 876);
        dataGridView.TabIndex = 0;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1351, 967);
        Controls.Add(panel1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "Form1";
        Text = "Form1";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        panel1.ResumeLayout(false);
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private Panel panel1;
    private SplitContainer splitContainer1;
    private FlowLayoutPanel flowLayoutPanelCategories;
    private DataGridView dataGridView;
    private Button buttonSelectAll;
    private Button buttonClearAll;
}
