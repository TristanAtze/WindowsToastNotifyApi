using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WindowsToastNotifyApi;
using Xunit;

namespace WindowsToastNotifyApi.Tests;

public sealed class ToastTests : IDisposable
{
    public ToastTests()
    {
        ResetToast();
    }

    public void Dispose()
    {
        ResetToast();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Show_BeforeInitialize_Throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Toast.Show("Title", "Message"));
        Assert.Contains("Call Toast.Initialize", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Initialize_SetsMetadataAndIsInitialized()
    {
        Toast.Initialize("com.acme.app", "Acme", iconPath: "/tmp/icon");
        Assert.True(Toast.IsInitialized);
        Assert.Equal("com.acme.app", GetStaticProperty<string?>("AppId"));
        Assert.Equal("Acme", GetStaticProperty<string?>("DisplayName"));
        Assert.Equal("/tmp/icon", GetStaticProperty<string?>("IconPath"));
    }

    [Fact]
    public void RaiseActivated_NotifiesSubscribers()
    {
        Toast.Initialize("id", "name");

        ToastActivationArgs? received = null;
        void Handler(ToastActivationArgs args) => received = args;

        Toast.Activated += Handler;
        try
        {
            InvokeInternalMethod("RaiseActivated", new ToastActivationArgs
            {
                Arguments = "action",
                Payload = new Dictionary<string, string> { ["key"] = "value" }
            });
        }
        finally
        {
            Toast.Activated -= Handler;
        }

        Assert.NotNull(received);
        Assert.Equal("action", received!.Arguments);
        Assert.Equal("value", received.Payload["key"]);
    }

    [Fact]
    public void Show_AfterInitialize_WritesFallbackOutput()
    {
        Toast.Initialize("app", "display");

        using var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);
        try
        {
            Toast.Show("Title", "Message");
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = writer.ToString();
        Assert.Contains("[Toast] Title: Message", output);
    }

    private static void ResetToast()
    {
        var toastType = typeof(Toast);

        SetStaticField(toastType, "_initialized", false);
        SetStaticField(toastType, "_appId", null);
        SetStaticField(toastType, "_displayName", null);
        SetStaticField(toastType, "_iconPath", null);
        SetStaticField(toastType, "Activated", null);
        SetStaticField(toastType, "_activationRegistered", false);
    }

    private static void SetStaticField(Type type, string fieldName, object? value)
    {
        var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (field != null)
            field.SetValue(null, value);
    }

    private static T GetStaticProperty<T>(string propertyName)
    {
        var prop = typeof(Toast).GetProperty(propertyName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        return prop is null
            ? throw new InvalidOperationException($"Could not find property '{propertyName}'.")
            : (T?)prop.GetValue(null)!;
    }

    private static void InvokeInternalMethod(string methodName, params object?[] args)
    {
        var method = typeof(Toast).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        method?.Invoke(null, args);
    }
}
