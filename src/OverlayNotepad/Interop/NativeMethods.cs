using System;
using System.Runtime.InteropServices;

namespace OverlayNotepad.Interop
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        internal const uint MOD_CONTROL = 0x0002;
        internal const uint MOD_SHIFT   = 0x0004;
        internal const uint VK_N        = 0x4E;
        internal const uint VK_T        = 0x54;
        internal const int  WM_HOTKEY   = 0x0312;
        internal const int  GWL_EXSTYLE = -20;
        internal const int  WS_EX_TRANSPARENT = 0x00000020;
        internal const int  WS_EX_LAYERED     = 0x00080000;
    }
}
