using CommunityToolkit.WinUI.Notifications; // ToastContentBuilder, ToastButton, ToastNotificationManagerCompat
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.UI.Notifications;             // ToastNotification

namespace WindowsToastNotifyApi;

/// <summary>
/// Entry point for showing Windows Toast notifications from Win32/.NET apps.
/// Works with CommunityToolkit.WinUI.Notifications 7.1.2.
/// </summary>
public static class Toast
{
    private static string? _appId;
    private static string? _displayName;
    private static string? _iconPath;
    private static bool _initialized;

    public static event Action<ToastActivationArgs>? Activated;

    /// <summary>
    /// Initialize once. Creates Start Menu shortcut with the AppUserModelID so toasts show for Win32 apps.
    /// </summary>
    public static void Initialize(string appId, string displayName, string? iconPath = null)
    {
        _appId = appId ?? throw new ArgumentNullException(nameof(appId));
        _displayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        _iconPath = iconPath;

        EnsureStartMenuShortcut(appId, displayName, iconPath);

        // Wire activation
        ToastNotificationManagerCompat.OnActivated += e =>
        {
            var payload = e.UserInput?.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? "")
                          ?? new Dictionary<string, string>();

            var args = new ToastActivationArgs
            {
                Arguments = e.Argument ?? string.Empty,
                Payload = payload
            };

            Activated?.Invoke(args);
        };

        _initialized = true;
    }

    public static void Show(string title, string message, ToastOptions? options = null)
    {
        EnsureInitialized();

        var builder = new ToastContentBuilder()
            .AddText(title)
            .AddText(message);

        if (!string.IsNullOrWhiteSpace(options?.HeroImagePath))
            builder.AddHeroImage(new Uri(options!.HeroImagePath!));

        if (!string.IsNullOrWhiteSpace(options?.AppLogoOverridePath))
            builder.AddAppLogoOverride(new Uri(options!.AppLogoOverridePath!), ToastGenericAppLogoCrop.Circle);

        if (options?.PrimaryButton is { } p)
            builder.AddButton(new ToastButton()
                .SetContent(p.content)
                .AddArgument("action", p.arguments ?? "primary"));

        if (options?.SecondaryButton is { } s)
            builder.AddButton(new ToastButton()
                .SetContent(s.content)
                .AddArgument("action", s.arguments ?? "secondary"));

        if (options?.Payload is { Count: > 0 })
        {
            foreach (var kv in options.Payload)
                builder.AddArgument(kv.Key, kv.Value);
        }

        // Duration is supported in 7.1.2
        var duration = options?.Duration == ToastDuration.Long
            ? CommunityToolkit.WinUI.Notifications.ToastDuration.Long
            : CommunityToolkit.WinUI.Notifications.ToastDuration.Short;

        builder.SetToastDuration(duration);

        // NOTE:
        // - Scenario is not set because builder API for scenario isn't available in 7.1.2.
        // - Silent is not set because AddSilent() does not exist in 7.1.2.

        var content = builder.GetToastContent();
        var toast = new ToastNotification(content.GetXml());

        // In 7.1.2 the compat notifier has no overload with appId. Use CreateToastNotifier().
        // The AppUserModelID is taken from the Start Menu shortcut created in Initialize().
        ToastNotificationManagerCompat.CreateToastNotifier().Show(toast);
    }

    public static void Info(string title, string message, ToastOptions? options = null)
        => Show($"ℹ️ {title}", message, options);

    public static void Success(string title, string message, ToastOptions? options = null)
        => Show($"✅ {title}", message, options);

    public static void Warning(string title, string message, ToastOptions? options = null)
        => Show($"⚠️ {title}", message, options);

    public static void Error(string title, string message, ToastOptions? options = null)
        => Show($"🛑 {title}", message, options);

    private static void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException("Call Toast.Initialize(appId, displayName) once before showing notifications.");
    }

    /// <summary>
    /// Create a Start Menu shortcut with AppUserModelID so Win32 toasts are attributed to your app.
    /// </summary>
    private static void EnsureStartMenuShortcut(string appId, string displayName, string? iconPath)
    {
        var programs = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        var shortcutPath = Path.Combine(programs, "Programs", $"{displayName}.lnk");

        if (File.Exists(shortcutPath))
            return;

        // Target = aktueller Prozess
        var exePath = Process.GetCurrentProcess().MainModule!.FileName!;
        var workDir = AppContext.BaseDirectory;

        ShellLinkHelper.CreateShortcutWithAppId(
            shortcutPath: shortcutPath,
            targetPath: exePath,
            arguments: "",
            workingDirectory: workDir,
            description: displayName,
            iconPath: iconPath,
            appUserModelId: appId
        );
    }
}
