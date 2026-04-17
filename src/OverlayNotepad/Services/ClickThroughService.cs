using System;
using OverlayNotepad.Interop;

namespace OverlayNotepad.Services
{
    public class ClickThroughChangedEventArgs : EventArgs
    {
        public bool IsEnabled { get; }
        public bool IsFirstActivation { get; }

        public ClickThroughChangedEventArgs(bool isEnabled, bool isFirstActivation)
        {
            IsEnabled = isEnabled;
            IsFirstActivation = isFirstActivation;
        }
    }

    public class ClickThroughService
    {
        private const double OpacityFloor = 0.2;

        private IntPtr _hwnd;
        private bool _isEnabled;
        private bool _isFirstActivation = true;
        private bool _isAvailable = true;

        public event EventHandler<ClickThroughChangedEventArgs> StateChanged;

        public bool IsEnabled  => _isEnabled;
        public bool IsAvailable => _isAvailable;

        public void Initialize(IntPtr hwnd, bool clickThroughHotkeyAvailable)
        {
            if (_hwnd != IntPtr.Zero) return;
            _hwnd = hwnd;
            _isAvailable = clickThroughHotkeyAvailable;
            _isEnabled = false;
        }

        public bool Toggle()
        {
            if (!_isAvailable) return false;

            _isEnabled = !_isEnabled;
            if (_isEnabled)
                Enable();
            else
                Disable();

            bool isFirst = _isEnabled && _isFirstActivation;
            if (isFirst) _isFirstActivation = false;

            StateChanged?.Invoke(this, new ClickThroughChangedEventArgs(_isEnabled, isFirst));
            return true;
        }

        private void Enable()
        {
            int exStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE,
                exStyle | NativeMethods.WS_EX_TRANSPARENT);
        }

        private void Disable()
        {
            int exStyle = NativeMethods.GetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(_hwnd, NativeMethods.GWL_EXSTYLE,
                exStyle & ~NativeMethods.WS_EX_TRANSPARENT);
        }

        public static double EnforceOpacityFloor(double currentOpacity, bool isClickThroughEnabled)
        {
            if (isClickThroughEnabled && currentOpacity < OpacityFloor)
                return OpacityFloor;
            return currentOpacity;
        }
    }
}
