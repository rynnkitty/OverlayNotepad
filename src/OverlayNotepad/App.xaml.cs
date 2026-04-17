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
                SettingsManager.Instance.Save();

                if (MainWindow is MainWindow mainWin)
                {
                    SettingsManager.Instance.SaveMemo(mainWin.GetMemoText());
                }
            }
            catch
            {
                // 긴급 저장 실패 시 추가 예외 방지
            }
        }
    }
}
