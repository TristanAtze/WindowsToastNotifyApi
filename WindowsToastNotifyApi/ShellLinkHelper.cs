using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using Windows.Foundation.Collections;

namespace WindowsToastNotifyApi;

[SupportedOSPlatform("windows")]
internal static class ShellLinkHelper
{
    public static void CreateShortcutWithAppId(
        string shortcutPath,
        string targetPath,
        string? arguments,
        string? workingDirectory,
        string? description,
        string? iconPath,
        string appUserModelId)
    {
        // COM: IShellLinkW
        var shellLink = (IShellLinkW)new CShellLink();

        // Pfad/Args/WD/Beschreibung setzen
        Marshal.ThrowExceptionForHR(shellLink.SetPath(targetPath));
        Marshal.ThrowExceptionForHR(shellLink.SetArguments(arguments ?? string.Empty));
        if (!string.IsNullOrWhiteSpace(workingDirectory))
            Marshal.ThrowExceptionForHR(shellLink.SetWorkingDirectory(workingDirectory));
        if (!string.IsNullOrWhiteSpace(description))
            Marshal.ThrowExceptionForHR(shellLink.SetDescription(description));
        if (!string.IsNullOrWhiteSpace(iconPath) && File.Exists(iconPath))
            Marshal.ThrowExceptionForHR(shellLink.SetIconLocation(iconPath, 0));

        // IPropertyStore abfragen und PKEY_AppUserModel_ID setzen
        var propertyStore = (IPropertyStore)shellLink;

        using var pv = PropVariant.FromString(appUserModelId);
        var pkeyAppId = PropertyKey.AppUserModel_ID;
        Marshal.ThrowExceptionForHR(propertyStore.SetValue(ref pkeyAppId, pv));
        Marshal.ThrowExceptionForHR(propertyStore.Commit());

        // Speichern
        var persistFile = (IPersistFile)shellLink;
        persistFile.Save(shortcutPath, true);

    }
}
