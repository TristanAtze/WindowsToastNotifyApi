#if !WINDOWS && !ANDROID && !IOS && !MACCATALYST
using System;
using System.Diagnostics;

namespace WindowsToastNotifyApi;

public static partial class Toast
{
    static partial void PlatformInitialize(string appId, string displayName, string? iconPath)
    {
        // No setup required for console/Linux fallback.
    }

    static partial void PlatformShow(string title, string message, ToastOptions? options)
    {
        if (TryNotifySend(title, message))
            return;

        Console.WriteLine($"[Toast] {title}: {message}");
    }

    private static bool TryNotifySend(string title, string message)
    {
        try
        {
            var psi = new ProcessStartInfo("notify-send")
            {
                UseShellExecute = false
            };
            psi.ArgumentList.Add(title);
            psi.ArgumentList.Add(message);
            using var process = Process.Start(psi);
            return process is not null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"notify-send failed: {ex.Message}");
            return false;
        }
    }
}
#endif
