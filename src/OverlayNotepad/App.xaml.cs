using System;
using System.Windows;
using System.Windows.Threading;
using OverlayNotepad.Services;

namespace OverlayNotepad
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[Startup] 초기화 완료: {sw.ElapsedMilliseconds}ms");
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
#endif
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EmergencySave();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            EmergencySave();
            e.Handled = true;
        }

        private void EmergencySave()
        {
            try
            {
                if (MainWindow is MainWindow mainWin)
                {
                    mainWin.EmergencyDisposeTray();
                    SettingsManager.Instance.SaveMemo(mainWin.GetMemoText());
                }
                SettingsManager.Instance.Save();
            }
            catch
            {
                // 긴급 저장 실패 시 추가 예외 방지
            }
        }
    }
}
