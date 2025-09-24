namespace WindowsToastNotifyApi;

/// <summary>
/// Cross-platform toast/notification entry point. Platform-specific implementations
/// live in partial class files (e.g. Toast.Windows.cs, Toast.Android.cs, ...).
/// </summary>
public static partial class Toast
{
    private static bool _initialized;
    private static string? _appId;
    private static string? _displayName;
    private static string? _iconPath;

    /// <summary>
    /// Raised when the user activates a toast (only supported on platforms that expose activation payloads).
    /// </summary>
    public static event Action<ToastActivationArgs>? Activated;

    internal static string? AppId => _appId;
    internal static string? DisplayName => _displayName;
    internal static string? IconPath => _iconPath;

    /// <summary>
    /// Indicates whether <see cref="Initialize"/> has been called for the current process.
    /// </summary>
    public static bool IsInitialized => _initialized;

    /// <summary>
    /// Call once during app start to set up platform specific notification infrastructure.
    /// </summary>
    /// <param name="appId">Unique application identifier. On some platforms this maps to a notification channel id.</param>
    /// <param name="displayName">Human readable application name.</param>
    /// <param name="iconPath">Optional app icon resource/asset path. Usage depends on the platform.</param>
    public static void Initialize(string appId, string displayName, string? iconPath = null)
    {
        _appId = appId ?? throw new ArgumentNullException(nameof(appId));
        _displayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        _iconPath = iconPath;

        PlatformInitialize(appId, displayName, iconPath);

        _initialized = true;
    }

    /// <summary>
    /// Shows a toast/notification with the given content.
    /// </summary>
    public static void Show(string title, string message, ToastOptions? options = null)
    {
        EnsureInitialized();
        PlatformShow(title, message, options);
    }

    public static void Info(string title, string message, ToastOptions? options = null)
        => Show($"ℹ️ {title}", message, options);

    public static void Success(string title, string message, ToastOptions? options = null)
        => Show($"✅ {title}", message, options);

    public static void Warning(string title, string message, ToastOptions? options = null)
        => Show($"⚠️ {title}", message, options);

    public static void Error(string title, string message, ToastOptions? options = null)
        => Show($"🛑 {title}", message, options);

    internal static void RaiseActivated(ToastActivationArgs args)
        => Activated?.Invoke(args);

    private static void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException("Call Toast.Initialize(appId, displayName) before showing notifications.");
    }

    static partial void PlatformInitialize(string appId, string displayName, string? iconPath);
    static partial void PlatformShow(string title, string message, ToastOptions? options);
}
