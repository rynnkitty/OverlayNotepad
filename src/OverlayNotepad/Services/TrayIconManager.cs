using System;
using System.Windows.Forms;

namespace OverlayNotepad.Services
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _trayMenu;
        private ToolStripMenuItem _alwaysOnTopItem;

        public event EventHandler ToggleVisibilityRequested;
        public event EventHandler AlwaysOnTopToggleRequested;
        public event EventHandler ExitRequested;

        public void Initialize(System.Drawing.Icon appIcon)
        {
            _trayMenu = new ContextMenuStrip();

            var toggleItem = new ToolStripMenuItem("표시/숨김");
            toggleItem.Click += (s, e) => ToggleVisibilityRequested?.Invoke(this, EventArgs.Empty);

            _alwaysOnTopItem = new ToolStripMenuItem("항상 위에");
            _alwaysOnTopItem.CheckOnClick = true;
            _alwaysOnTopItem.Checked = true;
            _alwaysOnTopItem.CheckedChanged += (s, e) =>
                AlwaysOnTopToggleRequested?.Invoke(this, EventArgs.Empty);

            var separator = new ToolStripSeparator();

            var exitItem = new ToolStripMenuItem("종료");
            exitItem.Click += (s, e) => ExitRequested?.Invoke(this, EventArgs.Empty);

            _trayMenu.Items.Add(toggleItem);
            _trayMenu.Items.Add(_alwaysOnTopItem);
            _trayMenu.Items.Add(separator);
            _trayMenu.Items.Add(exitItem);

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = appIcon ?? System.Drawing.SystemIcons.Application;
            _notifyIcon.Text = "OverlayNotepad";
            _notifyIcon.Visible = true;
            _notifyIcon.DoubleClick += (s, e) =>
                ToggleVisibilityRequested?.Invoke(this, EventArgs.Empty);
            _notifyIcon.ContextMenuStrip = _trayMenu;
        }

        public void UpdateAlwaysOnTopState(bool isTopmost)
        {
            if (_alwaysOnTopItem != null)
                _alwaysOnTopItem.Checked = isTopmost;
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
            _trayMenu?.Dispose();
            _trayMenu = null;
        }
    }
}
