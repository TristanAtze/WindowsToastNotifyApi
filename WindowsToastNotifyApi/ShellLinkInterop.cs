#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace WindowsToastNotifyApi;

[ComImport]
[Guid("00021401-0000-0000-C000-000000000046")]
internal class CShellLink { }

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214F9-0000-0000-C000-000000000046")]
internal interface IShellLinkW
{
    int GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, uint fFlags);
    int GetIDList(out IntPtr ppidl);
    int SetIDList(IntPtr pidl);
    int GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszName, int cchMaxName);
    int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
    int GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszDir, int cchMaxPath);
    int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
    int GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszArgs, int cchMaxPath);
    int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
    int GetHotkey(out short pwHotkey);
    int SetHotkey(short wHotkey);
    int GetShowCmd(out int piShowCmd);
    int SetShowCmd(int iShowCmd);
    int GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszIconPath, int cchIconPath, out int piIcon);
    int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
    int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
    int Resolve(IntPtr hwnd, uint fFlags);
    int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
internal interface IPropertyStore
{
    int GetCount(out uint cProps);
    int GetAt(uint iProp, out PropertyKey pkey);
    int GetValue(ref PropertyKey key, [Out] PropVariant pv);
    int SetValue(ref PropertyKey key, [In] PropVariant pv);
    int Commit();
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct PropertyKey
{
    public Guid fmtid;
    public uint pid;

    public PropertyKey(Guid fmtid, uint pid)
    {
        this.fmtid = fmtid;
        this.pid = pid;
    }

    public static PropertyKey AppUserModel_ID => new PropertyKey(
        new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
}

[StructLayout(LayoutKind.Sequential)]
internal sealed class PropVariant : IDisposable
{
    private ushort vt;
    private ushort wReserved1, wReserved2, wReserved3;
    private IntPtr ptr;
    private int int32;

    public static PropVariant FromString(string value)
    {
        var pv = new PropVariant();
        pv.vt = (ushort)VarEnum.VT_LPWSTR;
        pv.ptr = Marshal.StringToCoTaskMemUni(value);
        return pv;
    }

    public void Dispose()
    {
        PropVariantClear(this);
        GC.SuppressFinalize(this);
    }

    ~PropVariant()
    {
        PropVariantClear(this);
    }

    [DllImport("ole32.dll")]
    private static extern int PropVariantClear([In, Out] PropVariant pvar);
}

internal enum VarEnum : ushort
{
    VT_EMPTY = 0,
    VT_NULL = 1,
    VT_I2 = 2,
    VT_I4 = 3,
    VT_R4 = 4,
    VT_R8 = 5,
    VT_CY = 6,
    VT_DATE = 7,
    VT_BSTR = 8,
    VT_DISPATCH = 9,
    VT_ERROR = 10,
    VT_BOOL = 11,
    VT_VARIANT = 12,
    VT_UNKNOWN = 13,
    VT_DECIMAL = 14,
    VT_I1 = 16,
    VT_UI1 = 17,
    VT_UI2 = 18,
    VT_UI4 = 19,
    VT_I8 = 20,
    VT_UI8 = 21,
    VT_INT = 22,
    VT_UINT = 23,
    VT_LPWSTR = 31,
}
#endif
