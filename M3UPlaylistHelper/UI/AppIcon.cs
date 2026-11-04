namespace M3UPlaylistHelper.UI;

using System.Reflection;

/// <summary>
/// The application icon, embedded in the assembly, for the title bars of the windows.
/// (The .exe icon itself comes from ApplicationIcon in the project file.)
/// </summary>
public static class AppIcon
{
    private static readonly Lazy<Icon?> icon = new(Load);

    public static Icon? Icon => icon.Value;

    private static Icon? Load()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AppIcon.ico");
        return stream == null ? null : new Icon(stream);
    }

    public static void ApplyTo(Form form)
    {
        if (Icon != null)
        {
            form.Icon = Icon;
        }
    }
}
