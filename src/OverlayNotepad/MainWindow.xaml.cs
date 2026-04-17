using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OverlayNotepad.Helpers;
using OverlayNotepad.Models;
using OverlayNotepad.Services;

namespace OverlayNotepad
{
    public partial class MainWindow : Window
    {
        private AutoSaveManager _autoSaveManager;

        // 현재 설정은 SettingsManager.Instance.Current에서 참조
        private AppSettings.TextEffectConfig TextEffectCurrent =>
            SettingsManager.Instance.Current.TextEffect;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 설정 로드
            SettingsManager.Instance.Load();
            var settings = SettingsManager.Instance.Current;

            // 윈도우 위치/크기 복원
            RestoreWindowBounds(settings.Window);

            // 투명도 복원
            this.Opacity = settings.Transparency.Opacity;
            if (settings.Transparency.Mode == "background")
                this.Background = Brushes.Transparent;
            else
                this.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00));

            // Topmost 복원
            this.Topmost = settings.Topmost;

            // 텍스트 효과 초기화
            SyncOutlinedTextProperties();
            ApplyTextEffects();

            // 메모 복원
            MainTextBox.Text = SettingsManager.Instance.LoadMemo();

            // AutoSaveManager 시작
            _autoSaveManager = new AutoSaveManager(() =>
                SettingsManager.Instance.SaveMemo(MainTextBox.Text));
            _autoSaveManager.Start();

            MainTextBox.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 윈도우 위치/크기 저장
            var ws = SettingsManager.Instance.Current.Window;
            ws.Left = this.Left;
            ws.Top = this.Top;
            ws.Width = this.Width;
            ws.Height = this.Height;

            // 즉시 메모 저장
            _autoSaveManager?.SaveNow();
            _autoSaveManager?.Stop();
            SettingsManager.Instance.SaveMemo(MainTextBox.Text);
            SettingsManager.Instance.Save();
        }

        private void RestoreWindowBounds(AppSettings.WindowSettings ws)
        {
            if (ScreenHelper.IsWithinScreenBounds(ws.Left, ws.Top, ws.Width, ws.Height))
            {
                this.Left = ws.Left;
                this.Top = ws.Top;
            }
            else
            {
                var (left, top) = ScreenHelper.GetDefaultPosition(ws.Width, ws.Height);
                this.Left = left;
                this.Top = top;
            }
            this.Width = ws.Width;
            this.Height = ws.Height;
        }

        // --- 텍스트 동기화 ---

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutlinedText.Text = MainTextBox.Text;
            _autoSaveManager?.NotifyChanged();
        }

        private void MainTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            OutlinedText.VerticalOffset = e.VerticalOffset;
            OutlinedText.HorizontalOffset = e.HorizontalOffset;
        }

        private void SyncOutlinedTextProperties()
        {
            OutlinedText.FontFamily = MainTextBox.FontFamily;
            OutlinedText.FontSize = MainTextBox.FontSize;
            OutlinedText.FillBrush = Brushes.White;
        }

        // --- 효과 적용 ---

        private void ApplyTextEffects()
        {
            var fx = TextEffectCurrent;
            if (fx.OutlineEnabled)
            {
                OutlinedText.Visibility = Visibility.Visible;
                OutlinedText.OutlineThickness = fx.OutlineThickness;
                OutlinedText.OutlineBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(fx.OutlineColor));
                MainTextBox.Foreground = Brushes.Transparent;
            }
            else
            {
                OutlinedText.Visibility = Visibility.Collapsed;
                MainTextBox.Foreground = Brushes.White;
            }

            TextShadowEffect.Opacity = fx.ShadowEnabled ? fx.ShadowOpacity : 0;
        }

        // --- 컨텍스트 메뉴 핸들러 ---

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AlwaysOnTopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            this.Topmost = menuItem.IsChecked;
            SettingsManager.Instance.Current.Topmost = this.Topmost;
            SettingsManager.Instance.Save();
        }

        private void BackgroundTransparentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem.IsChecked)
            {
                this.Background = Brushes.Transparent;
                SettingsManager.Instance.Current.Transparency.Mode = "background";
            }
            else
            {
                this.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00));
                SettingsManager.Instance.Current.Transparency.Mode = "solid";
            }
            SettingsManager.Instance.Save();
        }

        private void OutlineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TextEffectCurrent.OutlineEnabled = menuItem.IsChecked;
            ApplyTextEffects();
            SettingsManager.Instance.Save();
        }

        private void ShadowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TextEffectCurrent.ShadowEnabled = menuItem.IsChecked;
            ApplyTextEffects();
            SettingsManager.Instance.Save();
        }

        private void OpacityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clicked = sender as MenuItem;
            double opacity = double.Parse(clicked.Tag.ToString());
            this.Opacity = opacity;
            SettingsManager.Instance.Current.Transparency.Opacity = opacity;
            SettingsManager.Instance.Save();
            UpdateOpacityMenuChecks(clicked);
        }

        private void UpdateOpacityMenuChecks(MenuItem selected)
        {
            foreach (object item in OpacityMenu.Items)
            {
                if (item is MenuItem mi)
                    mi.IsChecked = (mi == selected);
            }
        }

        private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // 비정상 종료 시 App.xaml.cs에서 호출
        public string GetMemoText() => MainTextBox?.Text ?? string.Empty;
    }
}
