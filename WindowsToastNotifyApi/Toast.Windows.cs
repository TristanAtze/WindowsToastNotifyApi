#if WINDOWS
using CommunityToolkit.WinUI.Notifications; // ToastContentBuilder, ToastButton, ToastNotificationManagerCompat
using System.Diagnostics;
using Windows.UI.Notifications;             // ToastNotification

namespace WindowsToastNotifyApi;

public static partial class Toast
{
    private static bool _activationRegistered;

    static partial void PlatformInitialize(string appId, string displayName, string? iconPath)
    {
        EnsureStartMenuShortcut(appId, displayName, iconPath);

        if (_activationRegistered)
            return;

        ToastNotificationManagerCompat.OnActivated += e =>
        {
            var payload = e.UserInput?.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? string.Empty)
                          ?? new Dictionary<string, string>();

            var args = new ToastActivationArgs
            {
                Arguments = e.Argument ?? string.Empty,
                Payload = payload
            };

            RaiseActivated(args);
        };

        _activationRegistered = true;
    }

    static partial void PlatformShow(string title, string message, ToastOptions? options)
    {
        var builder = new ToastContentBuilder()
            .AddText(title)
            .AddText(message);

        if (!string.IsNullOrWhiteSpace(options?.HeroImagePath))
            builder.AddHeroImage(new Uri(options!.HeroImagePath!));

        if (!string.IsNullOrWhiteSpace(options?.AppLogoOverridePath))
            builder.AddAppLogoOverride(new Uri(options!.AppLogoOverridePath!), ToastGenericAppLogoCrop.Circle);

        if (options?.PrimaryButton is { } p)
        {
            builder.AddButton(new ToastButton()
                .SetContent(p.content)
                .AddArgument("action", p.arguments ?? "primary"));
        }

        if (options?.SecondaryButton is { } s)
        {
            builder.AddButton(new ToastButton()
                .SetContent(s.content)
                .AddArgument("action", s.arguments ?? "secondary"));
        }

        if (options?.Payload is { Count: > 0 })
        {
            foreach (var kv in options.Payload)
                builder.AddArgument(kv.Key, kv.Value);
        }

        var duration = options?.Duration == ToastDuration.Long
            ? CommunityToolkit.WinUI.Notifications.ToastDuration.Long
            : CommunityToolkit.WinUI.Notifications.ToastDuration.Short;

        builder.SetToastDuration(duration);

        var content = builder.GetToastContent();
        var toast = new ToastNotification(content.GetXml());

        ToastNotificationManagerCompat.CreateToastNotifier().Show(toast);
    }

    private static void EnsureStartMenuShortcut(string appId, string displayName, string? iconPath)
    {
        var programs = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        var shortcutPath = Path.Combine(programs, "Programs", $"{displayName}.lnk");

        if (File.Exists(shortcutPath))
            return;

        var exePath = Process.GetCurrentProcess().MainModule!.FileName!;
        var workDir = AppContext.BaseDirectory;

        ShellLinkHelper.CreateShortcutWithAppId(
            shortcutPath: shortcutPath,
            targetPath: exePath,
            arguments: string.Empty,
            workingDirectory: workDir,
            description: displayName,
            iconPath: iconPath,
            appUserModelId: appId
        );
    }
}
#endif
