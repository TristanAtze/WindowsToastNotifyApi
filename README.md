# WindowsToastApi

Simple, Win32-friendly **Windows Toast Notification** API for .NET apps (Console, WPF, WinForms) on Windows 10/11.  
Built on top of [CommunityToolkit.WinUI.Notifications](https://www.nuget.org/packages/CommunityToolkit.WinUI.Notifications).

---

## ✨ Features

- ✅ Show toast notifications from **Console, WinForms, WPF** (Win32) apps  
- ✅ One-line `Toast.Info/Success/Warning/Error` presets  
- ✅ Custom buttons, payload, hero images, app logos  
- ✅ Activation callback with arguments and user input  
- ✅ Handles **AppUserModelID + Start Menu shortcut** automatically  
- ✅ Includes XML docs, SourceLink, symbols (`.snupkg`)

---

## 📦 Install

```powershell
dotnet add package WindowsToastApi
````

> Requires `net8.0-windows10.0.19041.0` or higher.

---

## 🚀 Quick Start

```csharp
using WindowsToastApi;

// 1. Initialize once at app startup
Toast.Initialize(
    appId: "YourCompany.YourApp",
    displayName: "Your App",
    iconPath: null);

// 2. Optional: subscribe to activation callback
Toast.Activated += args =>
{
    Console.WriteLine($"Activated: {args.Arguments}");
    foreach (var kv in args.Payload)
        Console.WriteLine($"{kv.Key}={kv.Value}");
};

// 3. Show presets
Toast.Info("Hello", "This is an info toast");
Toast.Success("Backup complete", "All files synced");
Toast.Warning("High CPU", "Process using 92%");
Toast.Error("Service crashed", "EventLogWatcher stopped");

// 4. Show toast with buttons & payload
Toast.Show("Deploy ready", "Promote to production?",
    new ToastOptions
    {
        PrimaryButton = ("Deploy now", "deploy"),
        SecondaryButton = ("Later", "later"),
        Payload = new Dictionary<string,string> { ["pipelineId"] = "a1b2c3" },
        Duration = ToastDuration.Long
    });
```

---

## 🔧 API Overview

### `Toast.Initialize(string appId, string displayName, string? iconPath = null)`

* Must be called **once** before showing toasts.
* Creates a Start Menu shortcut with the given **AppUserModelID**.

### `Toast.Show(string title, string message, ToastOptions? options = null)`

* Show a custom toast with text, optional images, buttons, payload.

### Presets

* `Toast.Info(string title, string message, ToastOptions? options = null)`
* `Toast.Success(...)`
* `Toast.Warning(...)`
* `Toast.Error(...)`

### Activation

```csharp
Toast.Activated += args =>
{
    Console.WriteLine(args.Arguments);
    foreach (var kv in args.Payload)
        Console.WriteLine($"{kv.Key}={kv.Value}");
};
```

### `ToastOptions`

* `HeroImagePath` → large hero image
* `AppLogoOverridePath` → circle app logo
* `Silent` → mute sound
* `Duration` → `Short` or `Long`
* `Scenario` → `Default`, `Alarm`, `Reminder`, `IncomingCall`
* `PrimaryButton` / `SecondaryButton` → `(content, arguments)`
* `Payload` → dictionary returned on activation

---

## ⚠️ Notes & Limitations

* Win32 apps require a **Start Menu shortcut** with **AppUserModelID** → handled by `Toast.Initialize`.
* Works only in **interactive user sessions** (not background Windows Services).
* Images/icons must be **absolute file paths** or `file:///` URIs.
* Windows may throttle notifications if spamming.

---

## 🛠 Development

Clone and build locally:

```bash
git clone https://github.com/BenSowieja/WindowsToastApi.git
cd WindowsToastApi
dotnet build -c Release
dotnet pack src/WindowsToastApi/WindowsToastApi.csproj -c Release -o out
```

---

## 📄 License

[MIT](LICENSE) © 2025 Ben Sowieja
