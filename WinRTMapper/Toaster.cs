using System;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using Windows.UI.Notifications;

public static class Toaster {
    private static string AppId;

    public static void PrepareToaster(string appid) {
        IsAppLinkExists(appid);
        AppId = appid;
    }

    public static void Toast(string header, string text) {
        var xdoc = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText03);
        var children = xdoc.GetElementsByTagName("text");
        children.Item(0).AppendChild(xdoc.CreateTextNode(header));
        children.Item(1).AppendChild(xdoc.CreateTextNode(text));
        var toast = new ToastNotification(xdoc);
        ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
    }

    private static bool CreateApplicationShortcut(string defaultPath, string appid) {
        var exePath = Process.GetCurrentProcess().MainModule.FileName;
        var newShortcut = (IShellLinkW)new CShellLink();

        // Create a shortcut to the exe
        ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
        ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

        // Open the shortcut property store, set the AppUserModelId property
        var newShortcutProperties = (IPropertyStore)newShortcut;

        using (PropVariant appId = new PropVariant(appid)) {
            ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
            ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
        }

        // Commit the shortcut to disk
        var newShortcutSave = (IPersistFile)newShortcut;

        ErrorHelper.VerifySucceeded(newShortcutSave.Save(defaultPath, true));
        return true;
    }

    private static bool IsAppLinkExists(string appid) {
        var defaultPath = string.Format(@"{0}\Microsoft\Windows\Start Menu\Programs\{1}.lnk", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appid);

        return File.Exists(defaultPath) == false ? CreateApplicationShortcut(defaultPath, appid) : true;
    }
}