using System;
using System.Windows.Threading;

namespace OverlayNotepad.Services
{
    public class AutoSaveManager
    {
        private const int DebounceMs = 2000;
        private const int TimerIntervalMs = 100;

        private readonly DispatcherTimer _timer;
        private readonly Action _saveAction;
        private DateTime _lastChangeTime;
        private bool _isDirty;

        public AutoSaveManager(Action saveAction)
        {
            _saveAction = saveAction;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(TimerIntervalMs) };
            _timer.Tick += Timer_Tick;
        }

        public bool HasUnsavedChanges => _isDirty;

        // 타이머는 NotifyChanged() 호출 시 자동 시작됩니다. 직접 Start() 불필요.
        public void Start()
        {
            if (_isDirty && !_timer.IsEnabled)
                _timer.Start();
        }

        public void Stop() => _timer.Stop();

        public void NotifyChanged()
        {
            _lastChangeTime = DateTime.Now;
            _isDirty = true;
            if (!_timer.IsEnabled)
                _timer.Start();
        }

        public void SaveNow()
        {
            if (_isDirty)
            {
                _saveAction?.Invoke();
                _isDirty = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isDirty && (DateTime.Now - _lastChangeTime).TotalMilliseconds >= DebounceMs)
            {
                _saveAction?.Invoke();
                _isDirty = false;
                _timer.Stop();
            }
        }
    }
}
