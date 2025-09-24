# MauiToastNotifyApi

Cross-platform toast/local notification helper for .NET MAUI and MAUI Blazor Hybrid apps. Ships a single API that lights up native notifications on Windows, Android, iOS, MacCatalyst and falls back to `notify-send` or console output on desktop Linux.

---

## ✨ Features

- ✅ Single `Toast` API usable from .NET MAUI, MAUI Blazor Hybrid, and classic Windows apps
- ✅ Windows implementation keeps CommunityToolkit-powered hero images, buttons, payload + activation callbacks
- ✅ Android uses native notification channels & icons (auto creates channel on first init)
- ✅ iOS/MacCatalyst schedule `UNUserNotificationCenter` alerts and surface activation callbacks
- ✅ Linux/Generic .NET hosts fall back to `notify-send` (if present) or console log
- ✅ Helpers for `Toast.Info/Success/Warning/Error` presets and shared `ToastOptions`

---

## 📦 Install

```powershell
dotnet add package MauiToastNotifyApi
```

> Targets `net8.0-windows10.0.19041.0`, `net8.0-android`, `net8.0-ios`, `net8.0-maccatalyst`, and `net8.0` (fallback/Linux).

---

## 🚀 Quick Start (.NET MAUI)

Call `Toast.Initialize` once after the MAUI app is built (for Windows the AppUserModelID doubles as the notification channel id on Android):

```csharp
// MauiProgram.cs
using WindowsToastNotifyApi; // Namespace kept for backwards compatibility

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

    var app = builder.Build();

    Toast.Initialize(
        appId: "com.company.myapp",  // Windows AppUserModelID / Android channel id
        displayName: "My App",
        iconPath: null);              // Optional Windows .ico/.png absolute path

    return app;
}
```

Trigger notifications anywhere after initialization:

```csharp
// e.g. inside a Page/ViewModel command
Toast.Success("Backup complete", "All files synced to the cloud.");

Toast.Show("Deploy ready", "Promote to production?",
    new ToastOptions
    {
        PrimaryButton = ("Deploy now", "deploy"),  // Windows buttons only
        SecondaryButton = ("Later", "later"),
        Payload = new Dictionary<string, string> { ["pipelineId"] = "a1b2c3" }
    });
```

For Win32 console/WPF/WinForms the usage stays unchanged.

---

## 🔄 Activation Callback

```csharp
Toast.Activated += args =>
{
    Console.WriteLine($"Arguments: {args.Arguments}");
    foreach (var kv in args.Payload)
        Console.WriteLine($"{kv.Key}={kv.Value}");
};
```

| Platform          | Activation Support                             |
| ----------------- | ---------------------------------------------- |
| Windows           | ✅ via CommunityToolkit + Start Menu shortcut  |
| iOS / MacCatalyst | ✅ via `UNUserNotificationCenter` response     |
| Android           | ⚠️ Launches the app; explicit callbacks TBD    |
| Linux / net8.0    | ❌ (console/log only)                          |

---

## 🧰 ToastOptions Cheat Sheet

| Option                    | Windows | Android | iOS/Mac | Linux |
| ------------------------- | :-----: | :-----: | :-----: | :---: |
| `HeroImagePath`           | ✅      | ❌      | ❌      | ❌    |
| `AppLogoOverridePath`     | ✅      | ❌      | ❌      | ❌    |
| `Silent`                  | ⚠️ (ignored on Toolkit 7.1.2) | ✅ | ✅ | ❌ |
| `Duration`                | ✅      | ❌      | ❌      | ❌    |
| `Primary/SecondaryButton` | ✅      | ❌      | ❌      | ❌    |
| `Payload`                 | ✅      | ✅ (intent extras) | ✅ | ❌ |

> Options that are not supported on a platform are safely ignored.

---

## 📝 Platform Notes

- **Windows**: still creates an AppUserModelID shortcut under Start → Programs if missing. Buttons & activation payloads work as before.
- **Android**: uses the MAUI `Platform.AppContext`; make sure `MauiProgram.CreateMauiApp` runs before calling `Toast.Initialize`. Notifications reuse the application icon.
- **iOS/MacCatalyst**: requests notification permissions on first initialization and hooks `UNUserNotificationCenter.Current.Delegate` to raise `Toast.Activated`.
- **Linux / net8.0**: tries to execute `notify-send`. If the binary is not available it falls back to `Console.WriteLine`.

---

## 🛠 Development

```bash
# Windows only shortcut creation still requires an interactive user
# Build the Windows target (other targets require the matching workloads)
dotnet build WindowsToastNotifyApi/WindowsToastNotifyApi.csproj -f net8.0-windows10.0.19041.0
```

> Android/iOS/MacCatalyst builds require the corresponding .NET MAUI workloads to be installed on your machine.

---

## 📄 License

[MIT](LICENSE) © 2025 Ben Sowieja

