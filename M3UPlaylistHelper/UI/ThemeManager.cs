namespace M3UPlaylistHelper.UI;

using Microsoft.Win32;
using System.Runtime.InteropServices;

public enum AppTheme
{
    System,
    Light,
    Dark,
}

/// <summary>
/// Colors of the light or dark theme. WinForms on .NET 8 has no built-in dark mode, so every control is colored here.
/// </summary>
public sealed class ThemeColors
{
    public required bool IsDark { get; init; }
    public required Color Window { get; init; }
    public required Color Surface { get; init; }
    public required Color Header { get; init; }
    public required Color Text { get; init; }
    public required Color MutedText { get; init; }
    public required Color Border { get; init; }
    public required Color Selection { get; init; }
    public required Color SelectionText { get; init; }
    public required Color Accent { get; init; }
    public required Color Good { get; init; }
    public required Color Bad { get; init; }

    public static readonly ThemeColors Light = new()
    {
        IsDark = false,
        Window = SystemColors.Control,
        Surface = SystemColors.Window,
        Header = SystemColors.Control,
        Text = SystemColors.ControlText,
        MutedText = SystemColors.GrayText,
        Border = SystemColors.Control,
        Selection = SystemColors.Highlight,
        SelectionText = SystemColors.HighlightText,
        Accent = SystemColors.Highlight,
        Good = Color.FromArgb(22, 128, 60),
        Bad = Color.FromArgb(196, 43, 28),
    };

    public static readonly ThemeColors Dark = new()
    {
        IsDark = true,
        Window = Color.FromArgb(32, 32, 32),
        Surface = Color.FromArgb(43, 43, 43),
        Header = Color.FromArgb(51, 51, 51),
        Text = Color.FromArgb(240, 240, 240),
        MutedText = Color.FromArgb(135, 135, 135),
        Border = Color.FromArgb(64, 64, 64),
        Selection = Color.FromArgb(38, 79, 120),
        SelectionText = Color.White,
        Accent = Color.FromArgb(55, 148, 255),
        Good = Color.FromArgb(87, 199, 120),
        Bad = Color.FromArgb(255, 107, 94),
    };
}

public static class ThemeManager
{
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaUseImmersiveDarkModeBefore20H1 = 19;

    public static ThemeColors Current { get; private set; } = ThemeColors.Light;

    public static void SetTheme(AppTheme theme)
    {
        bool dark = theme == AppTheme.Dark || (theme == AppTheme.System && IsSystemDark());
        Current = dark ? ThemeColors.Dark : ThemeColors.Light;
    }

    public static AppTheme Parse(string? value) =>
        Enum.TryParse<AppTheme>(value, ignoreCase: true, out var theme) ? theme : AppTheme.System;

    /// <summary>
    /// Whether Windows is set to dark mode for apps (Settings > Personalization > Colors).
    /// </summary>
    public static bool IsSystemDark()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Colors a form and everything on it with the current theme.
    /// </summary>
    public static void Apply(Form form)
    {
        ApplyTitleBar(form);
        ApplyControl(form);

        // Context menus are not in the Controls tree
        foreach (var strip in GetContextMenus(form))
        {
            ApplyToolStrip(strip);
        }
    }

    public static void ApplyToolStrip(ToolStrip strip)
    {
        var colors = Current;

        if (colors.IsDark)
        {
            strip.Renderer = new DarkToolStripRenderer();
        }
        else
        {
            strip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        }

        strip.BackColor = colors.IsDark ? colors.Window : default;
        strip.ForeColor = colors.Text;
        ApplyToolStripItems(strip.Items);
    }

    private static void ApplyToolStripItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            item.ForeColor = Current.Text;

            if (item is ToolStripDropDownItem dropDownItem)
            {
                dropDownItem.DropDown.BackColor = Current.IsDark ? Current.Surface : default;
                ApplyToolStripItems(dropDownItem.DropDownItems);
            }
        }
    }

    private static IEnumerable<ContextMenuStrip> GetContextMenus(Control control)
    {
        if (control.ContextMenuStrip != null)
        {
            yield return control.ContextMenuStrip;
        }

        foreach (Control child in control.Controls)
        {
            foreach (var menu in GetContextMenus(child))
            {
                yield return menu;
            }
        }
    }

    private static void ApplyControl(Control control)
    {
        var colors = Current;

        switch (control)
        {
            case ToolStrip strip:
                ApplyToolStrip(strip);
                return;

            case DataGridView grid:
                ApplyGrid(grid);
                break;

            case Button button:
                if (colors.IsDark)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = colors.Border;
                    button.BackColor = colors.Surface;
                    button.ForeColor = colors.Text;
                }
                else
                {
                    button.FlatStyle = FlatStyle.Standard;
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.UseVisualStyleBackColor = true;
                }

                break;

            case TextBoxBase or ComboBox:
                control.BackColor = colors.Surface;
                control.ForeColor = colors.Text;

                if (control is ComboBox comboBox)
                {
                    comboBox.FlatStyle = colors.IsDark ? FlatStyle.Flat : FlatStyle.Standard;
                }

                if (control is TextBox textBox)
                {
                    textBox.BorderStyle = colors.IsDark ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
                }

                break;

            default:
                control.BackColor = colors.Window;
                control.ForeColor = colors.Text;
                break;
        }

        ApplyScrollBarTheme(control);

        foreach (Control child in control.Controls)
        {
            ApplyControl(child);
        }
    }

    private static void ApplyGrid(DataGridView grid)
    {
        var colors = Current;

        grid.EnableHeadersVisualStyles = !colors.IsDark;
        grid.BackgroundColor = colors.Surface;
        grid.GridColor = colors.Border;
        grid.BorderStyle = colors.IsDark ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;

        grid.DefaultCellStyle.BackColor = colors.Surface;
        grid.DefaultCellStyle.ForeColor = colors.Text;
        grid.DefaultCellStyle.SelectionBackColor = colors.Selection;
        grid.DefaultCellStyle.SelectionForeColor = colors.SelectionText;

        grid.ColumnHeadersDefaultCellStyle.BackColor = colors.Header;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = colors.Text;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = colors.Header;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = colors.Text;
        grid.ColumnHeadersBorderStyle = colors.IsDark ? DataGridViewHeaderBorderStyle.Single : DataGridViewHeaderBorderStyle.Raised;
    }

    private static void ApplyTitleBar(Form form)
    {
        if (form.IsHandleCreated)
        {
            SetDarkTitleBar(form.Handle, Current.IsDark);
        }
        else
        {
            form.HandleCreated += (_, _) => SetDarkTitleBar(form.Handle, Current.IsDark);
        }
    }

    private static void SetDarkTitleBar(IntPtr handle, bool dark)
    {
        try
        {
            int value = dark ? 1 : 0;
            if (DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkMode, ref value, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkModeBefore20H1, ref value, sizeof(int));
            }
        }
        catch (Exception)
        {
            // Not supported before Windows 10 1809
        }
    }

    private static void ApplyScrollBarTheme(Control control)
    {
        // Scroll bars (grids, multi-line text boxes, combo box lists) follow the window theme
        if (control is not (ScrollBar or TextBoxBase or ComboBox or DataGridView))
        {
            return;
        }

        void Apply()
        {
            try
            {
                SetWindowTheme(control.Handle, Current.IsDark ? "DarkMode_Explorer" : "Explorer", null);
            }
            catch (Exception)
            {
                // Not supported on older Windows versions
            }
        }

        if (control.IsHandleCreated)
        {
            Apply();
        }
        else
        {
            control.HandleCreated += (_, _) => Apply();
        }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hwnd, string? subAppName, string? subIdList);

    private sealed class DarkToolStripRenderer() : ToolStripProfessionalRenderer(new DarkColorTable())
    {
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Current.Text;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled ? Current.Text : Current.MutedText;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            // The default check mark image is black, draw a light one instead
            var bounds = e.ImageRectangle;
            using var background = new SolidBrush(Current.Selection);
            e.Graphics.FillRectangle(background, bounds);

            using var pen = new Pen(Current.Text, Math.Max(1.5f, bounds.Width / 8f));
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawLines(pen, new[]
            {
                new PointF(bounds.Left + bounds.Width * 0.25f, bounds.Top + bounds.Height * 0.5f),
                new PointF(bounds.Left + bounds.Width * 0.42f, bounds.Top + bounds.Height * 0.68f),
                new PointF(bounds.Left + bounds.Width * 0.75f, bounds.Top + bounds.Height * 0.32f),
            });
        }
    }

    private sealed class DarkColorTable : ProfessionalColorTable
    {
        private static Color Window => ThemeColors.Dark.Window;
        private static Color Surface => ThemeColors.Dark.Surface;
        private static Color Hover => Color.FromArgb(62, 62, 64);
        private static Color Border => ThemeColors.Dark.Border;

        public override Color MenuStripGradientBegin => Window;
        public override Color MenuStripGradientEnd => Window;
        public override Color StatusStripGradientBegin => Window;
        public override Color StatusStripGradientEnd => Window;
        public override Color ToolStripDropDownBackground => Surface;
        public override Color ImageMarginGradientBegin => Surface;
        public override Color ImageMarginGradientMiddle => Surface;
        public override Color ImageMarginGradientEnd => Surface;
        public override Color MenuBorder => Border;
        public override Color MenuItemBorder => Hover;
        public override Color MenuItemSelected => Hover;
        public override Color MenuItemSelectedGradientBegin => Hover;
        public override Color MenuItemSelectedGradientEnd => Hover;
        public override Color MenuItemPressedGradientBegin => Surface;
        public override Color MenuItemPressedGradientMiddle => Surface;
        public override Color MenuItemPressedGradientEnd => Surface;
        public override Color SeparatorDark => Border;
        public override Color SeparatorLight => Border;
        public override Color ToolStripBorder => Border;
        public override Color CheckBackground => ThemeColors.Dark.Selection;
        public override Color CheckSelectedBackground => ThemeColors.Dark.Selection;
        public override Color CheckPressedBackground => ThemeColors.Dark.Selection;
    }
}
