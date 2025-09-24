#if IOS || MACCATALYST
using System;
using System.Collections.Generic;
using Foundation;
using UserNotifications;

namespace WindowsToastNotifyApi;

public static partial class Toast
{
    private static ToastNotificationDelegate? _notificationDelegate;
    private static bool _authorizationRequested;

    static partial void PlatformInitialize(string appId, string displayName, string? iconPath)
    {
        if (_authorizationRequested)
            return;

        _authorizationRequested = true;

        UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (approved, error) =>
        {
            if (error is not null)
            {
                System.Diagnostics.Debug.WriteLine($"Toast notification authorization failed: {error.LocalizedDescription}");
            }
        });

        if (_notificationDelegate is null)
        {
            _notificationDelegate = new ToastNotificationDelegate();
            UNUserNotificationCenter.Current.Delegate = _notificationDelegate;
        }
    }

    static partial void PlatformShow(string title, string message, ToastOptions? options)
    {
        var content = new UNMutableNotificationContent
        {
            Title = title,
            Body = message,
            Sound = options?.Silent == true ? null : UNNotificationSound.Default
        };

        if (options?.Payload is { Count: > 0 })
        {
            var userInfo = new NSMutableDictionary();
            foreach (var kv in options.Payload)
                userInfo.SetValueForKey(new NSString(kv.Value), new NSString(kv.Key));
            content.UserInfo = userInfo;
        }

        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);
        var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);

        UNUserNotificationCenter.Current.AddNotificationRequest(request, error =>
        {
            if (error is not null)
                System.Diagnostics.Debug.WriteLine($"Toast notification failed: {error.LocalizedDescription}");
        });
    }

    private sealed class ToastNotificationDelegate : UNUserNotificationCenterDelegate
    {
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var payload = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in response.Notification.Request.Content.UserInfo.Keys)
            {
                if (key is NSString nsKey)
                {
                    var value = response.Notification.Request.Content.UserInfo.ObjectForKey(nsKey);
                    payload[nsKey] = value switch
                    {
                        NSString s => s.ToString(),
                        _ => value?.ToString() ?? string.Empty
                    };
                }
            }

            Toast.RaiseActivated(new ToastActivationArgs
            {
                Arguments = response.ActionIdentifier ?? string.Empty,
                Payload = payload
            });

            completionHandler();
        }
    }
}
#endif
