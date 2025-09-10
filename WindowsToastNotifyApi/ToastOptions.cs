namespace WindowsToastNotifyApi;

/// <summary>
/// Options for customizing toast appearance and behavior.
/// NOTE: On CommunityToolkit.WinUI.Notifications 7.1.2 some features like Silent/Scenario
/// are not available via builder helpers and are therefore ignored here.
/// </summary>
public sealed class ToastOptions
{
    /// <summary>Path to a hero image (absolute path or file URI).</summary>
    public string? HeroImagePath { get; set; }

    /// <summary>App logo override image (round-cropped). Use absolute path or file URI.</summary>
    public string? AppLogoOverridePath { get; set; }

    /// <summary>Play no sound (NOT supported directly on 7.1.2 builder; currently ignored).</summary>
    public bool Silent { get; set; }

    /// <summary>Short (default) or Long toast duration.</summary>
    public ToastDuration Duration { get; set; } = ToastDuration.Short;

    /// <summary>Scenario (NOT supported directly on 7.1.2 builder; currently ignored).</summary>
    public ToastScenario Scenario { get; set; } = ToastScenario.Default;

    /// <summary>Optional primary button content and its argument.</summary>
    public (string content, string? arguments)? PrimaryButton { get; set; }

    /// <summary>Optional secondary button content and its argument.</summary>
    public (string content, string? arguments)? SecondaryButton { get; set; }

    /// <summary>Additional key/value arguments returned on activation.</summary>
    public IDictionary<string, string>? Payload { get; set; }
}

public enum ToastDuration { Short, Long }
public enum ToastScenario { Default, Alarm, Reminder, IncomingCall }
