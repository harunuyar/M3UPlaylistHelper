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
        components = new System.ComponentModel.Container();
        menuStrip = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        openFileToolStripMenuItem = new ToolStripMenuItem();
        openURLToolStripMenuItem = new ToolStripMenuItem();
        openXtreamToolStripMenuItem = new ToolStripMenuItem();
        addToPlaylistToolStripMenuItem = new ToolStripMenuItem();
        addFileToolStripMenuItem = new ToolStripMenuItem();
        addURLToolStripMenuItem = new ToolStripMenuItem();
        addXtreamToolStripMenuItem = new ToolStripMenuItem();
        recentToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorFile1 = new ToolStripSeparator();
        saveToolStripMenuItem = new ToolStripMenuItem();
        saveAsToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorFile2 = new ToolStripSeparator();
        exitToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem = new ToolStripMenuItem();
        findToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorEdit1 = new ToolStripSeparator();
        excludeDuplicatesToolStripMenuItem = new ToolStripMenuItem();
        sortCategoriesToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorEdit2 = new ToolStripSeparator();
        saveSelectionProfileToolStripMenuItem = new ToolStripMenuItem();
        applySelectionProfileToolStripMenuItem = new ToolStripMenuItem();
        epgToolStripMenuItem = new ToolStripMenuItem();
        loadProviderEpgToolStripMenuItem = new ToolStripMenuItem();
        loadEpgFromUrlToolStripMenuItem = new ToolStripMenuItem();
        loadEpgFromFileToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorEpg1 = new ToolStripSeparator();
        excludeChannelsWithoutEpgToolStripMenuItem = new ToolStripMenuItem();
        fillTvgIdsToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorEpg2 = new ToolStripSeparator();
        clearEpgToolStripMenuItem = new ToolStripMenuItem();
        viewToolStripMenuItem = new ToolStripMenuItem();
        themeToolStripMenuItem = new ToolStripMenuItem();
        themeSystemToolStripMenuItem = new ToolStripMenuItem();
        themeLightToolStripMenuItem = new ToolStripMenuItem();
        themeDarkToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem = new ToolStripMenuItem();
        howToUseToolStripMenuItem = new ToolStripMenuItem();
        aboutToolStripMenuItem = new ToolStripMenuItem();
        statusStrip = new StatusStrip();
        toolStripStatusLabel = new ToolStripStatusLabel();
        toolStripAccountLabel = new ToolStripStatusLabel();
        toolStripProgressBar = new ToolStripProgressBar();
        mainPanel = new Panel();
        splitContainer = new SplitContainer();
        buttonInvertCategories = new Button();
        buttonClearAllCategories = new Button();
        labelCategories = new Label();
        buttonSelectAllCategories = new Button();
        textBoxCategoryFilter = new TextBox();
        dataGridViewCategories = new DragDataGridView();
        columnCategoryIncluded = new DataGridViewCheckBoxColumn();
        columnCategoryTitle = new DataGridViewTextBoxColumn();
        columnCategoryCount = new DataGridViewTextBoxColumn();
        contextMenuCategories = new ContextMenuStrip(components);
        moveCategoryUpToolStripMenuItem = new ToolStripMenuItem();
        moveCategoryDownToolStripMenuItem = new ToolStripMenuItem();
        moveCategoryToTopToolStripMenuItem = new ToolStripMenuItem();
        moveCategoryToBottomToolStripMenuItem = new ToolStripMenuItem();
        checkBoxSearchAllCategories = new CheckBox();
        textBoxChannelFilter = new TextBox();
        buttonInvertChannels = new Button();
        checkBoxDownloadLogos = new CheckBox();
        buttonClearAllChannels = new Button();
        buttonSelectAllChannels = new Button();
        labelSelectedCategory = new Label();
        dataGridViewChannels = new DragDataGridView();
        columnChannelIncluded = new DataGridViewCheckBoxColumn();
        columnChannelLogo = new DataGridViewImageColumn();
        columnChannelName = new DataGridViewTextBoxColumn();
        columnChannelEpg = new DataGridViewTextBoxColumn();
        columnChannelCategory = new DataGridViewTextBoxColumn();
        columnChannelUrl = new DataGridViewTextBoxColumn();
        contextMenuChannels = new ContextMenuStrip(components);
        playChannelToolStripMenuItem = new ToolStripMenuItem();
        copyChannelUrlToolStripMenuItem = new ToolStripMenuItem();
        copyChannelNameToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparatorChannels = new ToolStripSeparator();
        moveToCategoryToolStripMenuItem = new ToolStripMenuItem();
        goToCategoryToolStripMenuItem = new ToolStripMenuItem();
        menuStrip.SuspendLayout();
        statusStrip.SuspendLayout();
        mainPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewCategories).BeginInit();
        contextMenuCategories.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewChannels).BeginInit();
        contextMenuChannels.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip
        //
        menuStrip.ImageScalingSize = new Size(24, 24);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, epgToolStripMenuItem, viewToolStripMenuItem, helpToolStripMenuItem });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1715, 33);
        menuStrip.TabIndex = 0;
        menuStrip.Text = "menuStrip";
        //
        // fileToolStripMenuItem
        //
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, openURLToolStripMenuItem, openXtreamToolStripMenuItem, addToPlaylistToolStripMenuItem, recentToolStripMenuItem, toolStripSeparatorFile1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparatorFile2, exitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(54, 29);
        fileToolStripMenuItem.Text = "&File";
        //
        // openFileToolStripMenuItem
        //
        openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
        openFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        openFileToolStripMenuItem.Size = new Size(320, 34);
        openFileToolStripMenuItem.Text = "&Open File...";
        openFileToolStripMenuItem.Click += OpenFileToolStripMenuItem_Click;
        //
        // openURLToolStripMenuItem
        //
        openURLToolStripMenuItem.Name = "openURLToolStripMenuItem";
        openURLToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.U;
        openURLToolStripMenuItem.Size = new Size(320, 34);
        openURLToolStripMenuItem.Text = "Open &URL...";
        openURLToolStripMenuItem.Click += OpenURLToolStripMenuItem_Click;
        //
        // openXtreamToolStripMenuItem
        //
        openXtreamToolStripMenuItem.Name = "openXtreamToolStripMenuItem";
        openXtreamToolStripMenuItem.Size = new Size(320, 34);
        openXtreamToolStripMenuItem.Text = "Open &Xtream Codes...";
        openXtreamToolStripMenuItem.Click += OpenXtreamToolStripMenuItem_Click;
        //
        // addToPlaylistToolStripMenuItem
        //
        addToPlaylistToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addFileToolStripMenuItem, addURLToolStripMenuItem, addXtreamToolStripMenuItem });
        addToPlaylistToolStripMenuItem.Name = "addToPlaylistToolStripMenuItem";
        addToPlaylistToolStripMenuItem.Size = new Size(320, 34);
        addToPlaylistToolStripMenuItem.Text = "A&dd to Current Playlist";
        //
        // addFileToolStripMenuItem
        //
        addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
        addFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
        addFileToolStripMenuItem.Size = new Size(380, 34);
        addFileToolStripMenuItem.Text = "Playlist &File...";
        addFileToolStripMenuItem.Click += AddFileToolStripMenuItem_Click;
        //
        // addURLToolStripMenuItem
        //
        addURLToolStripMenuItem.Name = "addURLToolStripMenuItem";
        addURLToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.U;
        addURLToolStripMenuItem.Size = new Size(380, 34);
        addURLToolStripMenuItem.Text = "Playlist &URL...";
        addURLToolStripMenuItem.Click += AddURLToolStripMenuItem_Click;
        //
        // addXtreamToolStripMenuItem
        //
        addXtreamToolStripMenuItem.Name = "addXtreamToolStripMenuItem";
        addXtreamToolStripMenuItem.Size = new Size(380, 34);
        addXtreamToolStripMenuItem.Text = "&Xtream Codes...";
        addXtreamToolStripMenuItem.Click += AddXtreamToolStripMenuItem_Click;
        //
        // recentToolStripMenuItem
        //
        recentToolStripMenuItem.Name = "recentToolStripMenuItem";
        recentToolStripMenuItem.Size = new Size(320, 34);
        recentToolStripMenuItem.Text = "&Recent";
        //
        // toolStripSeparatorFile1
        //
        toolStripSeparatorFile1.Name = "toolStripSeparatorFile1";
        toolStripSeparatorFile1.Size = new Size(317, 6);
        //
        // saveToolStripMenuItem
        //
        saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        saveToolStripMenuItem.Size = new Size(320, 34);
        saveToolStripMenuItem.Text = "&Save";
        saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
        //
        // saveAsToolStripMenuItem
        //
        saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
        saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
        saveAsToolStripMenuItem.Size = new Size(320, 34);
        saveAsToolStripMenuItem.Text = "Save &As...";
        saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
        //
        // toolStripSeparatorFile2
        //
        toolStripSeparatorFile2.Name = "toolStripSeparatorFile2";
        toolStripSeparatorFile2.Size = new Size(317, 6);
        //
        // exitToolStripMenuItem
        //
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(320, 34);
        exitToolStripMenuItem.Text = "E&xit";
        exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
        //
        // editToolStripMenuItem
        //
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { findToolStripMenuItem, toolStripSeparatorEdit1, excludeDuplicatesToolStripMenuItem, sortCategoriesToolStripMenuItem, toolStripSeparatorEdit2, saveSelectionProfileToolStripMenuItem, applySelectionProfileToolStripMenuItem });
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.Size = new Size(58, 29);
        editToolStripMenuItem.Text = "&Edit";
        //
        // findToolStripMenuItem
        //
        findToolStripMenuItem.Name = "findToolStripMenuItem";
        findToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
        findToolStripMenuItem.Size = new Size(380, 34);
        findToolStripMenuItem.Text = "&Find Channel";
        findToolStripMenuItem.Click += FindToolStripMenuItem_Click;
        //
        // toolStripSeparatorEdit1
        //
        toolStripSeparatorEdit1.Name = "toolStripSeparatorEdit1";
        toolStripSeparatorEdit1.Size = new Size(377, 6);
        //
        // excludeDuplicatesToolStripMenuItem
        //
        excludeDuplicatesToolStripMenuItem.Name = "excludeDuplicatesToolStripMenuItem";
        excludeDuplicatesToolStripMenuItem.Size = new Size(380, 34);
        excludeDuplicatesToolStripMenuItem.Text = "Exclude &Duplicate Channels";
        excludeDuplicatesToolStripMenuItem.Click += ExcludeDuplicatesToolStripMenuItem_Click;
        //
        // sortCategoriesToolStripMenuItem
        //
        sortCategoriesToolStripMenuItem.Name = "sortCategoriesToolStripMenuItem";
        sortCategoriesToolStripMenuItem.Size = new Size(380, 34);
        sortCategoriesToolStripMenuItem.Text = "Sort &Categories A-Z";
        sortCategoriesToolStripMenuItem.Click += SortCategoriesToolStripMenuItem_Click;
        //
        // toolStripSeparatorEdit2
        //
        toolStripSeparatorEdit2.Name = "toolStripSeparatorEdit2";
        toolStripSeparatorEdit2.Size = new Size(377, 6);
        //
        // saveSelectionProfileToolStripMenuItem
        //
        saveSelectionProfileToolStripMenuItem.Name = "saveSelectionProfileToolStripMenuItem";
        saveSelectionProfileToolStripMenuItem.Size = new Size(380, 34);
        saveSelectionProfileToolStripMenuItem.Text = "Save Selection &Profile...";
        saveSelectionProfileToolStripMenuItem.Click += SaveSelectionProfileToolStripMenuItem_Click;
        //
        // applySelectionProfileToolStripMenuItem
        //
        applySelectionProfileToolStripMenuItem.Name = "applySelectionProfileToolStripMenuItem";
        applySelectionProfileToolStripMenuItem.Size = new Size(380, 34);
        applySelectionProfileToolStripMenuItem.Text = "&Apply Selection Profile...";
        applySelectionProfileToolStripMenuItem.Click += ApplySelectionProfileToolStripMenuItem_Click;
        //
        // epgToolStripMenuItem
        //
        epgToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadProviderEpgToolStripMenuItem, loadEpgFromUrlToolStripMenuItem, loadEpgFromFileToolStripMenuItem, toolStripSeparatorEpg1, excludeChannelsWithoutEpgToolStripMenuItem, fillTvgIdsToolStripMenuItem, toolStripSeparatorEpg2, clearEpgToolStripMenuItem });
        epgToolStripMenuItem.Name = "epgToolStripMenuItem";
        epgToolStripMenuItem.Size = new Size(62, 29);
        epgToolStripMenuItem.Text = "E&PG";
        epgToolStripMenuItem.DropDownOpening += EpgToolStripMenuItem_DropDownOpening;
        //
        // loadProviderEpgToolStripMenuItem
        //
        loadProviderEpgToolStripMenuItem.Name = "loadProviderEpgToolStripMenuItem";
        loadProviderEpgToolStripMenuItem.Size = new Size(420, 34);
        loadProviderEpgToolStripMenuItem.Text = "Load EPG from &Playlist / Provider";
        loadProviderEpgToolStripMenuItem.Click += LoadProviderEpgToolStripMenuItem_Click;
        //
        // loadEpgFromUrlToolStripMenuItem
        //
        loadEpgFromUrlToolStripMenuItem.Name = "loadEpgFromUrlToolStripMenuItem";
        loadEpgFromUrlToolStripMenuItem.Size = new Size(420, 34);
        loadEpgFromUrlToolStripMenuItem.Text = "Load EPG from &URL...";
        loadEpgFromUrlToolStripMenuItem.Click += LoadEpgFromUrlToolStripMenuItem_Click;
        //
        // loadEpgFromFileToolStripMenuItem
        //
        loadEpgFromFileToolStripMenuItem.Name = "loadEpgFromFileToolStripMenuItem";
        loadEpgFromFileToolStripMenuItem.Size = new Size(420, 34);
        loadEpgFromFileToolStripMenuItem.Text = "Load EPG from &File...";
        loadEpgFromFileToolStripMenuItem.Click += LoadEpgFromFileToolStripMenuItem_Click;
        //
        // toolStripSeparatorEpg1
        //
        toolStripSeparatorEpg1.Name = "toolStripSeparatorEpg1";
        toolStripSeparatorEpg1.Size = new Size(417, 6);
        //
        // excludeChannelsWithoutEpgToolStripMenuItem
        //
        excludeChannelsWithoutEpgToolStripMenuItem.Name = "excludeChannelsWithoutEpgToolStripMenuItem";
        excludeChannelsWithoutEpgToolStripMenuItem.Size = new Size(420, 34);
        excludeChannelsWithoutEpgToolStripMenuItem.Text = "&Exclude Channels Without EPG";
        excludeChannelsWithoutEpgToolStripMenuItem.Click += ExcludeChannelsWithoutEpgToolStripMenuItem_Click;
        //
        // fillTvgIdsToolStripMenuItem
        //
        fillTvgIdsToolStripMenuItem.Name = "fillTvgIdsToolStripMenuItem";
        fillTvgIdsToolStripMenuItem.Size = new Size(420, 34);
        fillTvgIdsToolStripMenuItem.Text = "Fill &Missing tvg-id by Channel Name";
        fillTvgIdsToolStripMenuItem.Click += FillTvgIdsToolStripMenuItem_Click;
        //
        // toolStripSeparatorEpg2
        //
        toolStripSeparatorEpg2.Name = "toolStripSeparatorEpg2";
        toolStripSeparatorEpg2.Size = new Size(417, 6);
        //
        // clearEpgToolStripMenuItem
        //
        clearEpgToolStripMenuItem.Name = "clearEpgToolStripMenuItem";
        clearEpgToolStripMenuItem.Size = new Size(420, 34);
        clearEpgToolStripMenuItem.Text = "&Clear EPG";
        clearEpgToolStripMenuItem.Click += ClearEpgToolStripMenuItem_Click;
        //
        // viewToolStripMenuItem
        //
        viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { themeToolStripMenuItem });
        viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        viewToolStripMenuItem.Size = new Size(65, 29);
        viewToolStripMenuItem.Text = "&View";
        //
        // themeToolStripMenuItem
        //
        themeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { themeSystemToolStripMenuItem, themeLightToolStripMenuItem, themeDarkToolStripMenuItem });
        themeToolStripMenuItem.Name = "themeToolStripMenuItem";
        themeToolStripMenuItem.Size = new Size(270, 34);
        themeToolStripMenuItem.Text = "&Theme";
        //
        // themeSystemToolStripMenuItem
        //
        themeSystemToolStripMenuItem.Name = "themeSystemToolStripMenuItem";
        themeSystemToolStripMenuItem.Size = new Size(270, 34);
        themeSystemToolStripMenuItem.Text = "Use &System Setting";
        themeSystemToolStripMenuItem.Click += ThemeToolStripMenuItem_Click;
        //
        // themeLightToolStripMenuItem
        //
        themeLightToolStripMenuItem.Name = "themeLightToolStripMenuItem";
        themeLightToolStripMenuItem.Size = new Size(270, 34);
        themeLightToolStripMenuItem.Text = "&Light";
        themeLightToolStripMenuItem.Click += ThemeToolStripMenuItem_Click;
        //
        // themeDarkToolStripMenuItem
        //
        themeDarkToolStripMenuItem.Name = "themeDarkToolStripMenuItem";
        themeDarkToolStripMenuItem.Size = new Size(270, 34);
        themeDarkToolStripMenuItem.Text = "&Dark";
        themeDarkToolStripMenuItem.Click += ThemeToolStripMenuItem_Click;
        //
        // helpToolStripMenuItem
        //
        helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { howToUseToolStripMenuItem, aboutToolStripMenuItem });
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(65, 29);
        helpToolStripMenuItem.Text = "&Help";
        //
        // howToUseToolStripMenuItem
        //
        howToUseToolStripMenuItem.Name = "howToUseToolStripMenuItem";
        howToUseToolStripMenuItem.ShortcutKeys = Keys.F1;
        howToUseToolStripMenuItem.Size = new Size(270, 34);
        howToUseToolStripMenuItem.Text = "&How to Use";
        howToUseToolStripMenuItem.Click += HowToUseToolStripMenuItem_Click;
        //
        // aboutToolStripMenuItem
        //
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Size = new Size(270, 34);
        aboutToolStripMenuItem.Text = "&About";
        aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
        //
        // statusStrip
        //
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, toolStripAccountLabel, toolStripProgressBar });
        statusStrip.Location = new Point(0, 935);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1715, 32);
        statusStrip.TabIndex = 2;
        statusStrip.Text = "statusStrip";
        //
        // toolStripStatusLabel
        //
        toolStripStatusLabel.Name = "toolStripStatusLabel";
        toolStripStatusLabel.Size = new Size(1698, 25);
        toolStripStatusLabel.Spring = true;
        toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // toolStripAccountLabel
        //
        toolStripAccountLabel.Name = "toolStripAccountLabel";
        toolStripAccountLabel.Size = new Size(0, 25);
        toolStripAccountLabel.Visible = false;
        //
        // toolStripProgressBar
        //
        toolStripProgressBar.MarqueeAnimationSpeed = 30;
        toolStripProgressBar.Name = "toolStripProgressBar";
        toolStripProgressBar.Size = new Size(150, 24);
        toolStripProgressBar.Style = ProgressBarStyle.Marquee;
        toolStripProgressBar.Visible = false;
        //
        // mainPanel
        //
        mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        mainPanel.Controls.Add(splitContainer);
        mainPanel.Location = new Point(12, 36);
        mainPanel.Name = "mainPanel";
        mainPanel.Size = new Size(1691, 893);
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
        splitContainer.Panel1.Controls.Add(buttonInvertCategories);
        splitContainer.Panel1.Controls.Add(buttonClearAllCategories);
        splitContainer.Panel1.Controls.Add(labelCategories);
        splitContainer.Panel1.Controls.Add(buttonSelectAllCategories);
        splitContainer.Panel1.Controls.Add(textBoxCategoryFilter);
        splitContainer.Panel1.Controls.Add(dataGridViewCategories);
        //
        // splitContainer.Panel2
        //
        splitContainer.Panel2.Controls.Add(checkBoxSearchAllCategories);
        splitContainer.Panel2.Controls.Add(textBoxChannelFilter);
        splitContainer.Panel2.Controls.Add(buttonInvertChannels);
        splitContainer.Panel2.Controls.Add(checkBoxDownloadLogos);
        splitContainer.Panel2.Controls.Add(buttonClearAllChannels);
        splitContainer.Panel2.Controls.Add(buttonSelectAllChannels);
        splitContainer.Panel2.Controls.Add(labelSelectedCategory);
        splitContainer.Panel2.Controls.Add(dataGridViewChannels);
        splitContainer.Size = new Size(1691, 893);
        splitContainer.SplitterDistance = 450;
        splitContainer.TabIndex = 0;
        //
        // buttonInvertCategories
        //
        buttonInvertCategories.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonInvertCategories.Location = new Point(261, 859);
        buttonInvertCategories.Name = "buttonInvertCategories";
        buttonInvertCategories.Size = new Size(123, 34);
        buttonInvertCategories.TabIndex = 5;
        buttonInvertCategories.Text = "Invert";
        buttonInvertCategories.UseVisualStyleBackColor = true;
        buttonInvertCategories.Click += ButtonInvertCategories_Click;
        //
        // buttonClearAllCategories
        //
        buttonClearAllCategories.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonClearAllCategories.Location = new Point(132, 859);
        buttonClearAllCategories.Name = "buttonClearAllCategories";
        buttonClearAllCategories.Size = new Size(123, 34);
        buttonClearAllCategories.TabIndex = 4;
        buttonClearAllCategories.Text = "Clear All";
        buttonClearAllCategories.UseVisualStyleBackColor = true;
        buttonClearAllCategories.Click += ButtonClearAllCategories_Click;
        //
        // labelCategories
        //
        labelCategories.AutoSize = true;
        labelCategories.Location = new Point(3, 0);
        labelCategories.Name = "labelCategories";
        labelCategories.Size = new Size(96, 25);
        labelCategories.TabIndex = 0;
        labelCategories.Text = "Categories";
        //
        // buttonSelectAllCategories
        //
        buttonSelectAllCategories.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonSelectAllCategories.Location = new Point(3, 859);
        buttonSelectAllCategories.Name = "buttonSelectAllCategories";
        buttonSelectAllCategories.Size = new Size(123, 34);
        buttonSelectAllCategories.TabIndex = 3;
        buttonSelectAllCategories.Text = "Select All";
        buttonSelectAllCategories.UseVisualStyleBackColor = true;
        buttonSelectAllCategories.Click += ButtonSelectAllCategories_Click;
        //
        // textBoxCategoryFilter
        //
        textBoxCategoryFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxCategoryFilter.Location = new Point(0, 28);
        textBoxCategoryFilter.Name = "textBoxCategoryFilter";
        textBoxCategoryFilter.PlaceholderText = "Filter categories...";
        textBoxCategoryFilter.Size = new Size(450, 31);
        textBoxCategoryFilter.TabIndex = 1;
        textBoxCategoryFilter.TextChanged += TextBoxFilter_TextChanged;
        textBoxCategoryFilter.KeyDown += TextBoxFilter_KeyDown;
        //
        // dataGridViewCategories
        //
        dataGridViewCategories.AllowDrop = true;
        dataGridViewCategories.AllowUserToAddRows = false;
        dataGridViewCategories.AllowUserToDeleteRows = false;
        dataGridViewCategories.AllowUserToResizeRows = false;
        dataGridViewCategories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewCategories.BackgroundColor = SystemColors.Window;
        dataGridViewCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCategories.Columns.AddRange(new DataGridViewColumn[] { columnCategoryIncluded, columnCategoryTitle, columnCategoryCount });
        dataGridViewCategories.ContextMenuStrip = contextMenuCategories;
        dataGridViewCategories.GridColor = SystemColors.Control;
        dataGridViewCategories.Location = new Point(0, 65);
        dataGridViewCategories.MultiSelect = false;
        dataGridViewCategories.Name = "dataGridViewCategories";
        dataGridViewCategories.RowHeadersVisible = false;
        dataGridViewCategories.RowHeadersWidth = 62;
        dataGridViewCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewCategories.Size = new Size(450, 788);
        dataGridViewCategories.VirtualMode = true;
        dataGridViewCategories.TabIndex = 2;
        dataGridViewCategories.CellFormatting += DataGridViewCategories_CellFormatting;
        dataGridViewCategories.CellMouseDown += DataGridView_CellMouseDown;
        dataGridViewCategories.CellValueNeeded += DataGridViewCategories_CellValueNeeded;
        dataGridViewCategories.CellValuePushed += DataGridViewCategories_CellValuePushed;
        dataGridViewCategories.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
        dataGridViewCategories.SelectionChanged += DataGridViewCategories_SelectionChanged;
        dataGridViewCategories.KeyDown += DataGridView_KeyDown;
        dataGridViewCategories.KeyUp += DataGridView_KeyUp;
        dataGridViewCategories.DragEnter += DataGridView_DragOver;
        dataGridViewCategories.DragOver += DataGridView_DragOver;
        dataGridViewCategories.DragDrop += DataGridView_DragDrop;
        dataGridViewCategories.DragLeave += DataGridView_DragLeave;
        dataGridViewCategories.RowPostPaint += DataGridView_RowPostPaint;
        //
        // columnCategoryIncluded
        //
        columnCategoryIncluded.HeaderText = "Include";
        columnCategoryIncluded.MinimumWidth = 8;
        columnCategoryIncluded.Name = "columnCategoryIncluded";
        columnCategoryIncluded.Width = 80;
        //
        // columnCategoryTitle
        //
        columnCategoryTitle.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        columnCategoryTitle.HeaderText = "Title";
        columnCategoryTitle.MinimumWidth = 8;
        columnCategoryTitle.Name = "columnCategoryTitle";
        columnCategoryTitle.ToolTipText = "Double-click or press F2 to rename";
        //
        // columnCategoryCount
        //
        columnCategoryCount.HeaderText = "Channels";
        columnCategoryCount.MinimumWidth = 8;
        columnCategoryCount.Name = "columnCategoryCount";
        columnCategoryCount.ReadOnly = true;
        columnCategoryCount.Width = 120;
        columnCategoryCount.ToolTipText = "Included / total channels";
        //
        // contextMenuCategories
        //
        contextMenuCategories.ImageScalingSize = new Size(24, 24);
        contextMenuCategories.Items.AddRange(new ToolStripItem[] { moveCategoryUpToolStripMenuItem, moveCategoryDownToolStripMenuItem, moveCategoryToTopToolStripMenuItem, moveCategoryToBottomToolStripMenuItem });
        contextMenuCategories.Name = "contextMenuCategories";
        contextMenuCategories.Size = new Size(221, 132);
        contextMenuCategories.Opening += ContextMenuCategories_Opening;
        //
        // moveCategoryUpToolStripMenuItem
        //
        moveCategoryUpToolStripMenuItem.Name = "moveCategoryUpToolStripMenuItem";
        moveCategoryUpToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Up;
        moveCategoryUpToolStripMenuItem.Size = new Size(220, 32);
        moveCategoryUpToolStripMenuItem.Text = "Move Up";
        moveCategoryUpToolStripMenuItem.Click += MoveCategoryUpToolStripMenuItem_Click;
        //
        // moveCategoryDownToolStripMenuItem
        //
        moveCategoryDownToolStripMenuItem.Name = "moveCategoryDownToolStripMenuItem";
        moveCategoryDownToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Down;
        moveCategoryDownToolStripMenuItem.Size = new Size(220, 32);
        moveCategoryDownToolStripMenuItem.Text = "Move Down";
        moveCategoryDownToolStripMenuItem.Click += MoveCategoryDownToolStripMenuItem_Click;
        //
        // moveCategoryToTopToolStripMenuItem
        //
        moveCategoryToTopToolStripMenuItem.Name = "moveCategoryToTopToolStripMenuItem";
        moveCategoryToTopToolStripMenuItem.Size = new Size(220, 32);
        moveCategoryToTopToolStripMenuItem.Text = "Move to Top";
        moveCategoryToTopToolStripMenuItem.Click += MoveCategoryToTopToolStripMenuItem_Click;
        //
        // moveCategoryToBottomToolStripMenuItem
        //
        moveCategoryToBottomToolStripMenuItem.Name = "moveCategoryToBottomToolStripMenuItem";
        moveCategoryToBottomToolStripMenuItem.Size = new Size(220, 32);
        moveCategoryToBottomToolStripMenuItem.Text = "Move to Bottom";
        moveCategoryToBottomToolStripMenuItem.Click += MoveCategoryToBottomToolStripMenuItem_Click;
        //
        // checkBoxSearchAllCategories
        //
        checkBoxSearchAllCategories.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        checkBoxSearchAllCategories.AutoSize = true;
        checkBoxSearchAllCategories.Location = new Point(1072, 30);
        checkBoxSearchAllCategories.Name = "checkBoxSearchAllCategories";
        checkBoxSearchAllCategories.Size = new Size(162, 29);
        checkBoxSearchAllCategories.TabIndex = 2;
        checkBoxSearchAllCategories.Text = "All categories";
        checkBoxSearchAllCategories.UseVisualStyleBackColor = true;
        checkBoxSearchAllCategories.CheckedChanged += CheckBoxSearchAllCategories_CheckedChanged;
        //
        // textBoxChannelFilter
        //
        textBoxChannelFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxChannelFilter.Location = new Point(0, 28);
        textBoxChannelFilter.Name = "textBoxChannelFilter";
        textBoxChannelFilter.PlaceholderText = "Search channels by name or tvg-id... (Ctrl+F)";
        textBoxChannelFilter.Size = new Size(1060, 31);
        textBoxChannelFilter.TabIndex = 1;
        textBoxChannelFilter.TextChanged += TextBoxFilter_TextChanged;
        textBoxChannelFilter.KeyDown += TextBoxFilter_KeyDown;
        //
        // buttonInvertChannels
        //
        buttonInvertChannels.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonInvertChannels.Location = new Point(261, 859);
        buttonInvertChannels.Name = "buttonInvertChannels";
        buttonInvertChannels.Size = new Size(123, 34);
        buttonInvertChannels.TabIndex = 6;
        buttonInvertChannels.Text = "Invert";
        buttonInvertChannels.UseVisualStyleBackColor = true;
        buttonInvertChannels.Click += ButtonInvertChannels_Click;
        //
        // checkBoxDownloadLogos
        //
        checkBoxDownloadLogos.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        checkBoxDownloadLogos.AutoSize = true;
        checkBoxDownloadLogos.Checked = true;
        checkBoxDownloadLogos.CheckState = CheckState.Checked;
        checkBoxDownloadLogos.Location = new Point(1060, 863);
        checkBoxDownloadLogos.Name = "checkBoxDownloadLogos";
        checkBoxDownloadLogos.Size = new Size(174, 29);
        checkBoxDownloadLogos.TabIndex = 7;
        checkBoxDownloadLogos.Text = "Download Logos";
        checkBoxDownloadLogos.UseVisualStyleBackColor = true;
        checkBoxDownloadLogos.CheckedChanged += CheckBoxDownloadLogos_CheckedChanged;
        //
        // buttonClearAllChannels
        //
        buttonClearAllChannels.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonClearAllChannels.Location = new Point(132, 859);
        buttonClearAllChannels.Name = "buttonClearAllChannels";
        buttonClearAllChannels.Size = new Size(123, 34);
        buttonClearAllChannels.TabIndex = 5;
        buttonClearAllChannels.Text = "Clear All";
        buttonClearAllChannels.UseVisualStyleBackColor = true;
        buttonClearAllChannels.Click += ButtonClearAllChannels_Click;
        //
        // buttonSelectAllChannels
        //
        buttonSelectAllChannels.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        buttonSelectAllChannels.Location = new Point(3, 859);
        buttonSelectAllChannels.Name = "buttonSelectAllChannels";
        buttonSelectAllChannels.Size = new Size(123, 34);
        buttonSelectAllChannels.TabIndex = 4;
        buttonSelectAllChannels.Text = "Select All";
        buttonSelectAllChannels.UseVisualStyleBackColor = true;
        buttonSelectAllChannels.Click += ButtonSelectAllChannels_Click;
        //
        // labelSelectedCategory
        //
        labelSelectedCategory.AutoSize = true;
        labelSelectedCategory.Location = new Point(3, 0);
        labelSelectedCategory.Name = "labelSelectedCategory";
        labelSelectedCategory.Size = new Size(0, 25);
        labelSelectedCategory.TabIndex = 0;
        //
        // dataGridViewChannels
        //
        dataGridViewChannels.AllowDrop = true;
        dataGridViewChannels.AllowUserToAddRows = false;
        dataGridViewChannels.AllowUserToDeleteRows = false;
        dataGridViewChannels.AllowUserToResizeRows = false;
        dataGridViewChannels.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewChannels.BackgroundColor = SystemColors.Window;
        dataGridViewChannels.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewChannels.Columns.AddRange(new DataGridViewColumn[] { columnChannelIncluded, columnChannelLogo, columnChannelName, columnChannelEpg, columnChannelCategory, columnChannelUrl });
        dataGridViewChannels.ContextMenuStrip = contextMenuChannels;
        dataGridViewChannels.GridColor = SystemColors.Control;
        dataGridViewChannels.Location = new Point(0, 65);
        dataGridViewChannels.Name = "dataGridViewChannels";
        dataGridViewChannels.RowHeadersVisible = false;
        dataGridViewChannels.RowHeadersWidth = 62;
        dataGridViewChannels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewChannels.Size = new Size(1237, 788);
        dataGridViewChannels.VirtualMode = true;
        dataGridViewChannels.TabIndex = 3;
        dataGridViewChannels.CellFormatting += DataGridViewChannels_CellFormatting;
        dataGridViewChannels.CellMouseDown += DataGridView_CellMouseDown;
        dataGridViewChannels.CellValueNeeded += DataGridViewChannels_CellValueNeeded;
        dataGridViewChannels.CellValuePushed += DataGridViewChannels_CellValuePushed;
        dataGridViewChannels.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
        dataGridViewChannels.Scroll += DataGridViewChannels_Scroll;
        dataGridViewChannels.KeyDown += DataGridView_KeyDown;
        dataGridViewChannels.KeyUp += DataGridView_KeyUp;
        dataGridViewChannels.DragEnter += DataGridView_DragOver;
        dataGridViewChannels.DragOver += DataGridView_DragOver;
        dataGridViewChannels.DragDrop += DataGridView_DragDrop;
        dataGridViewChannels.DragLeave += DataGridView_DragLeave;
        dataGridViewChannels.RowPostPaint += DataGridView_RowPostPaint;
        dataGridViewChannels.Resize += DataGridViewChannels_Resize;
        //
        // columnChannelIncluded
        //
        columnChannelIncluded.HeaderText = "Include";
        columnChannelIncluded.MinimumWidth = 8;
        columnChannelIncluded.Name = "columnChannelIncluded";
        columnChannelIncluded.Width = 80;
        //
        // columnChannelLogo
        //
        columnChannelLogo.HeaderText = "Logo";
        columnChannelLogo.ImageLayout = DataGridViewImageCellLayout.Zoom;
        columnChannelLogo.MinimumWidth = 8;
        columnChannelLogo.Name = "columnChannelLogo";
        columnChannelLogo.ReadOnly = true;
        columnChannelLogo.Resizable = DataGridViewTriState.False;
        columnChannelLogo.Width = 80;
        //
        // columnChannelName
        //
        columnChannelName.HeaderText = "Name";
        columnChannelName.MinimumWidth = 8;
        columnChannelName.Name = "columnChannelName";
        columnChannelName.ToolTipText = "Double-click or press F2 to rename";
        columnChannelName.Width = 350;
        //
        // columnChannelEpg
        //
        columnChannelEpg.HeaderText = "EPG";
        columnChannelEpg.MinimumWidth = 8;
        columnChannelEpg.Name = "columnChannelEpg";
        columnChannelEpg.ReadOnly = true;
        columnChannelEpg.ToolTipText = "Whether the loaded EPG has program information for the channel's tvg-id";
        columnChannelEpg.Visible = false;
        columnChannelEpg.Width = 70;
        //
        // columnChannelCategory
        //
        columnChannelCategory.HeaderText = "Category";
        columnChannelCategory.MinimumWidth = 8;
        columnChannelCategory.Name = "columnChannelCategory";
        columnChannelCategory.ReadOnly = true;
        columnChannelCategory.Visible = false;
        columnChannelCategory.Width = 200;
        //
        // columnChannelUrl
        //
        columnChannelUrl.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        columnChannelUrl.HeaderText = "URL";
        columnChannelUrl.MinimumWidth = 8;
        columnChannelUrl.Name = "columnChannelUrl";
        columnChannelUrl.ReadOnly = true;
        //
        // contextMenuChannels
        //
        contextMenuChannels.ImageScalingSize = new Size(24, 24);
        contextMenuChannels.Items.AddRange(new ToolStripItem[] { playChannelToolStripMenuItem, copyChannelUrlToolStripMenuItem, copyChannelNameToolStripMenuItem, toolStripSeparatorChannels, moveToCategoryToolStripMenuItem, goToCategoryToolStripMenuItem });
        contextMenuChannels.Name = "contextMenuChannels";
        contextMenuChannels.Size = new Size(241, 132);
        contextMenuChannels.Opening += ContextMenuChannels_Opening;
        //
        // playChannelToolStripMenuItem
        //
        playChannelToolStripMenuItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        playChannelToolStripMenuItem.Name = "playChannelToolStripMenuItem";
        playChannelToolStripMenuItem.Size = new Size(240, 32);
        playChannelToolStripMenuItem.Text = "Play";
        playChannelToolStripMenuItem.Click += PlayChannelToolStripMenuItem_Click;
        //
        // copyChannelUrlToolStripMenuItem
        //
        copyChannelUrlToolStripMenuItem.Name = "copyChannelUrlToolStripMenuItem";
        copyChannelUrlToolStripMenuItem.Size = new Size(240, 32);
        copyChannelUrlToolStripMenuItem.Text = "Copy URL";
        copyChannelUrlToolStripMenuItem.Click += CopyChannelUrlToolStripMenuItem_Click;
        //
        // copyChannelNameToolStripMenuItem
        //
        copyChannelNameToolStripMenuItem.Name = "copyChannelNameToolStripMenuItem";
        copyChannelNameToolStripMenuItem.Size = new Size(240, 32);
        copyChannelNameToolStripMenuItem.Text = "Copy Name";
        copyChannelNameToolStripMenuItem.Click += CopyChannelNameToolStripMenuItem_Click;
        //
        // toolStripSeparatorChannels
        //
        toolStripSeparatorChannels.Name = "toolStripSeparatorChannels";
        toolStripSeparatorChannels.Size = new Size(237, 6);
        //
        // moveToCategoryToolStripMenuItem
        //
        moveToCategoryToolStripMenuItem.Name = "moveToCategoryToolStripMenuItem";
        moveToCategoryToolStripMenuItem.Size = new Size(240, 32);
        moveToCategoryToolStripMenuItem.Text = "Move to Category...";
        moveToCategoryToolStripMenuItem.Click += MoveToCategoryToolStripMenuItem_Click;
        //
        // goToCategoryToolStripMenuItem
        //
        goToCategoryToolStripMenuItem.Name = "goToCategoryToolStripMenuItem";
        goToCategoryToolStripMenuItem.Size = new Size(240, 32);
        goToCategoryToolStripMenuItem.Text = "Go to Category";
        goToCategoryToolStripMenuItem.Click += GoToCategoryToolStripMenuItem_Click;
        //
        // MainForm
        //
        AllowDrop = true;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1715, 967);
        Controls.Add(mainPanel);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(900, 500);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "M3U Playlist Helper";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        mainPanel.ResumeLayout(false);
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel1.PerformLayout();
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewCategories).EndInit();
        contextMenuCategories.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewChannels).EndInit();
        contextMenuChannels.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openFileToolStripMenuItem;
    private ToolStripMenuItem openURLToolStripMenuItem;
    private ToolStripMenuItem openXtreamToolStripMenuItem;
    private ToolStripMenuItem addToPlaylistToolStripMenuItem;
    private ToolStripMenuItem addFileToolStripMenuItem;
    private ToolStripMenuItem addURLToolStripMenuItem;
    private ToolStripMenuItem addXtreamToolStripMenuItem;
    private ToolStripMenuItem recentToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorFile1;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorFile2;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem findToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorEdit1;
    private ToolStripMenuItem excludeDuplicatesToolStripMenuItem;
    private ToolStripMenuItem sortCategoriesToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorEdit2;
    private ToolStripMenuItem saveSelectionProfileToolStripMenuItem;
    private ToolStripMenuItem applySelectionProfileToolStripMenuItem;
    private ToolStripMenuItem epgToolStripMenuItem;
    private ToolStripMenuItem loadProviderEpgToolStripMenuItem;
    private ToolStripMenuItem loadEpgFromUrlToolStripMenuItem;
    private ToolStripMenuItem loadEpgFromFileToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorEpg1;
    private ToolStripMenuItem excludeChannelsWithoutEpgToolStripMenuItem;
    private ToolStripMenuItem fillTvgIdsToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorEpg2;
    private ToolStripMenuItem clearEpgToolStripMenuItem;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem themeToolStripMenuItem;
    private ToolStripMenuItem themeSystemToolStripMenuItem;
    private ToolStripMenuItem themeLightToolStripMenuItem;
    private ToolStripMenuItem themeDarkToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem howToUseToolStripMenuItem;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel toolStripStatusLabel;
    private ToolStripStatusLabel toolStripAccountLabel;
    private ToolStripProgressBar toolStripProgressBar;
    private Panel mainPanel;
    private SplitContainer splitContainer;
    private Label labelCategories;
    private TextBox textBoxCategoryFilter;
    private DragDataGridView dataGridViewCategories;
    private DataGridViewCheckBoxColumn columnCategoryIncluded;
    private DataGridViewTextBoxColumn columnCategoryTitle;
    private DataGridViewTextBoxColumn columnCategoryCount;
    private Button buttonSelectAllCategories;
    private Button buttonClearAllCategories;
    private Button buttonInvertCategories;
    private ContextMenuStrip contextMenuCategories;
    private ToolStripMenuItem moveCategoryUpToolStripMenuItem;
    private ToolStripMenuItem moveCategoryDownToolStripMenuItem;
    private ToolStripMenuItem moveCategoryToTopToolStripMenuItem;
    private ToolStripMenuItem moveCategoryToBottomToolStripMenuItem;
    private Label labelSelectedCategory;
    private TextBox textBoxChannelFilter;
    private CheckBox checkBoxSearchAllCategories;
    private DragDataGridView dataGridViewChannels;
    private DataGridViewCheckBoxColumn columnChannelIncluded;
    private DataGridViewImageColumn columnChannelLogo;
    private DataGridViewTextBoxColumn columnChannelName;
    private DataGridViewTextBoxColumn columnChannelEpg;
    private DataGridViewTextBoxColumn columnChannelCategory;
    private DataGridViewTextBoxColumn columnChannelUrl;
    private Button buttonSelectAllChannels;
    private Button buttonClearAllChannels;
    private Button buttonInvertChannels;
    private CheckBox checkBoxDownloadLogos;
    private ContextMenuStrip contextMenuChannels;
    private ToolStripMenuItem playChannelToolStripMenuItem;
    private ToolStripMenuItem copyChannelUrlToolStripMenuItem;
    private ToolStripMenuItem copyChannelNameToolStripMenuItem;
    private ToolStripSeparator toolStripSeparatorChannels;
    private ToolStripMenuItem moveToCategoryToolStripMenuItem;
    private ToolStripMenuItem goToCategoryToolStripMenuItem;
}
