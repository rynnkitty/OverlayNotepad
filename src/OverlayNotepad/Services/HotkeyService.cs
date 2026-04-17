using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using OverlayNotepad.Interop;

namespace OverlayNotepad.Services
{
    public class HotkeyService : IDisposable
    {
        private const int IdToggleVisibility   = 1;
        private const int IdToggleClickThrough = 2;

        private IntPtr _hwnd;
        private HwndSource _hwndSource;
        private bool _visibilityHotkeyRegistered;
        private bool _clickThroughHotkeyRegistered;
        private bool _disposed;

        public event EventHandler ToggleVisibilityRequested;
        public event EventHandler ToggleClickThroughRequested;

        public bool IsClickThroughHotkeyRegistered => _clickThroughHotkeyRegistered;

        public void Initialize(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");
            if (_hwndSource != null) return;

            var helper = new WindowInteropHelper(window);
            _hwnd = helper.Handle;
            _hwndSource = HwndSource.FromHwnd(_hwnd);
            _hwndSource.AddHook(WndProc);

            _visibilityHotkeyRegistered = NativeMethods.RegisterHotKey(
                _hwnd, IdToggleVisibility,
                NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT, NativeMethods.VK_N);

            _clickThroughHotkeyRegistered = NativeMethods.RegisterHotKey(
                _hwnd, IdToggleClickThrough,
                NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT, NativeMethods.VK_T);
        }

        public IReadOnlyList<string> GetFailedHotkeys()
        {
            var failed = new List<string>();
            if (!_visibilityHotkeyRegistered)
                failed.Add("Ctrl+Shift+N (표시/숨김)");
            if (!_clickThroughHotkeyRegistered)
                failed.Add("Ctrl+Shift+T (Click-Through)");
            return failed;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                if (hotkeyId == IdToggleVisibility)
                    ToggleVisibilityRequested?.Invoke(this, EventArgs.Empty);
                else if (hotkeyId == IdToggleClickThrough)
                    ToggleClickThroughRequested?.Invoke(this, EventArgs.Empty);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_visibilityHotkeyRegistered)
                NativeMethods.UnregisterHotKey(_hwnd, IdToggleVisibility);
            if (_clickThroughHotkeyRegistered)
                NativeMethods.UnregisterHotKey(_hwnd, IdToggleClickThrough);
            _hwndSource?.RemoveHook(WndProc);
            _hwndSource?.Dispose();
        }
    }
}
