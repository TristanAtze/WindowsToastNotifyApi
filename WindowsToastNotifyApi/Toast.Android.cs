#if ANDROID
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;

namespace WindowsToastNotifyApi;

public static partial class Toast
{
    private static int _notificationId;

    static partial void PlatformInitialize(string appId, string displayName, string? iconPath)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var context = Application.Context;
        var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
        var channelId = GetChannelId(appId);
        var channel = new NotificationChannel(channelId, displayName, NotificationImportance.Default)
        {
            Description = displayName
        };
        manager?.CreateNotificationChannel(channel);
    }

    static partial void PlatformShow(string title, string message, ToastOptions? options)
    {
        var context = Application.Context;
        var channelId = GetChannelId(AppId ?? context.PackageName);

        Notification notification;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var builder = new Notification.Builder(context, channelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetStyle(new Notification.BigTextStyle().BigText(message))
                .SetSmallIcon(ResolveSmallIcon(context))
                .SetAutoCancel(true);

            var pendingIntent = CreateLaunchIntent(context, options);
            if (pendingIntent is not null)
                builder.SetContentIntent(pendingIntent);

            notification = builder.Build();
        }
        else
        {
            var builder = new Notification.Builder(context)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(ResolveSmallIcon(context))
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.Default)
                .SetStyle(new Notification.BigTextStyle().BigText(message));

            var pendingIntent = CreateLaunchIntent(context, options);
            if (pendingIntent is not null)
                builder.SetContentIntent(pendingIntent);

            notification = builder.Build();
        }

        var manager = NotificationManager.FromContext(context);
        var id = Interlocked.Increment(ref _notificationId);
        manager.Notify(id, notification);
    }

    private static PendingIntent? CreateLaunchIntent(Context context, ToastOptions? options)
    {
        var launchIntent = context.PackageManager?.GetLaunchIntentForPackage(context.PackageName);
        if (launchIntent is null)
            return null;

        launchIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        if (options?.Payload is { Count: > 0 })
        {
            foreach (var kv in options.Payload)
                launchIntent.PutExtra(kv.Key, kv.Value);
        }

        var flags = PendingIntentFlags.UpdateCurrent;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            flags |= PendingIntentFlags.Immutable;

        return PendingIntent.GetActivity(context, 0, launchIntent, flags);
    }

    private static string GetChannelId(string appId)
        => string.IsNullOrWhiteSpace(appId) ? "maui.toast.default" : appId;

    private static int ResolveSmallIcon(Context context)
        => context.ApplicationInfo?.Icon is int icon && icon != 0
            ? icon
            : Android.Resource.Drawable.IcDialogInfo;
}
#endif

