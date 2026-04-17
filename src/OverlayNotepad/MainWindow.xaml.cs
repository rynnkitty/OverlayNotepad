using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OverlayNotepad.Helpers;
using OverlayNotepad.Models;
using OverlayNotepad.Services;

namespace OverlayNotepad
{
    public partial class MainWindow : Window
    {
        private AutoSaveManager _autoSaveManager;
        private TrayIconManager _trayIconManager;
        private bool _fontMenuInitialized = false;

        // 현재 설정은 SettingsManager.Instance.Current에서 참조
        private AppSettings.TextEffectConfig TextEffectCurrent =>
            SettingsManager.Instance.Current.TextEffect;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            this.StateChanged += Window_StateChanged;
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

            // 서식 설정 복원
            var font = settings.Font;
            if (!FontHelper.IsInstalled(font.Family))
                font.Family = "맑은 고딕";
            ApplyFontFamily(font.Family, persist: false);
            ApplyFontSize(font.Size, persist: false);
            ApplyFontColor(font.Color, persist: false);

            // 테마 초기화
            ThemeManager.Instance.SetTheme(settings.Theme ?? "dark");
            ApplyTheme(ThemeManager.Instance.CurrentTheme);

            // 메뉴 체크 상태 동기화
            SyncMenuCheckedStates(settings);

            // 메모 복원
            MainTextBox.Text = SettingsManager.Instance.LoadMemo();

            // AutoSaveManager 시작
            _autoSaveManager = new AutoSaveManager(() =>
                SettingsManager.Instance.SaveMemo(MainTextBox.Text));
            _autoSaveManager.Start();

            // 색상 서브메뉴 동적 생성
            InitializeFontColorMenu();

            // TrayIconManager 초기화
            _trayIconManager = new TrayIconManager();
            System.Drawing.Icon appIcon = null;
            try
            {
                var iconUri = new System.Uri("pack://application:,,,/Resources/app.ico");
                var stream = Application.GetResourceStream(iconUri);
                if (stream != null)
                    appIcon = new System.Drawing.Icon(stream.Stream);
            }
            catch { }
            _trayIconManager.Initialize(appIcon ?? System.Drawing.SystemIcons.Application);
            _trayIconManager.UpdateAlwaysOnTopState(this.Topmost);
            _trayIconManager.ToggleVisibilityRequested += OnTrayToggleVisibility;
            _trayIconManager.AlwaysOnTopToggleRequested += OnTrayAlwaysOnTopToggle;
            _trayIconManager.ExitRequested += OnTrayExit;

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

            // 트레이 아이콘 정리
            _trayIconManager?.Dispose();
            _trayIconManager = null;
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

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void AlwaysOnTopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
                SetTopmost(menuItem.IsChecked);
        }

        private void OnTrayToggleVisibility(object sender, System.EventArgs e)
        {
            if (!this.IsVisible || this.WindowState == WindowState.Minimized)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
                this.Activate();
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void OnTrayAlwaysOnTopToggle(object sender, System.EventArgs e)
        {
            SetTopmost(!this.Topmost);
        }

        private void OnTrayExit(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SetTopmost(bool value)
        {
            this.Topmost = value;
            AlwaysOnTopMenuItem.IsChecked = value;
            _trayIconManager?.UpdateAlwaysOnTopState(value);
            SettingsManager.Instance.Current.Topmost = value;
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

        private void ApplyTheme(ThemeDefinition theme)
        {
            var textBrush = new SolidColorBrush(theme.TextColor);
            textBrush.Freeze();
            OutlinedText.FillBrush = textBrush;
            MainTextBox.CaretBrush = textBrush;
            if (!TextEffectCurrent.OutlineEnabled)
                MainTextBox.Foreground = textBrush;

            if (SettingsManager.Instance.Current.Transparency.Mode == "background")
                MainBackground.Background = Brushes.Transparent;
            else
            {
                var bgColor = theme.BackgroundColor;
                bgColor.A = (byte)(theme.BackgroundOpacity * 255);
                var bgBrush = new SolidColorBrush(bgColor);
                bgBrush.Freeze();
                MainBackground.Background = bgBrush;
            }

            var outlineBrush = new SolidColorBrush(theme.OutlineColor);
            outlineBrush.Freeze();
            OutlinedText.OutlineBrush = outlineBrush;

            TextShadowEffect.Color = theme.ShadowColor;

            var dragColor = theme.DragBarColor;
            dragColor.A = (byte)(theme.DragBarOpacity * 255);
            var dragBrush = new SolidColorBrush(dragColor);
            dragBrush.Freeze();
            DragBar.Background = dragBrush;
        }

        private void ToggleTheme()
        {
            ThemeManager.Instance.ToggleTheme();
            var theme = ThemeManager.Instance.CurrentTheme;
            ApplyTheme(theme);
            var current = SettingsManager.Instance.Current;
            current.Theme = theme.Name;
            current.TextEffect.OutlineColor = ColorToHex(theme.OutlineColor);
            current.Font.Color = ColorToHex(theme.TextColor);
            SettingsManager.Instance.Save();
        }

        private void SyncMenuCheckedStates(AppSettings settings)
        {
            BackgroundTransparentMenuItem.IsChecked = (settings.Transparency.Mode == "background");
            AlwaysOnTopMenuItem.IsChecked = settings.Topmost;
            OutlineMenuItem.IsChecked = settings.TextEffect.OutlineEnabled;
            ShadowMenuItem.IsChecked = settings.TextEffect.ShadowEnabled;
            foreach (object item in OpacityMenu.Items)
            {
                if (item is MenuItem mi && mi.Tag != null &&
                    double.TryParse(mi.Tag.ToString(), out double tagVal))
                    mi.IsChecked = (tagVal == settings.Transparency.Opacity);
            }
        }

        private static string ColorToHex(Color c) =>
            string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);

        private void ApplyFontFamily(string fontName, bool persist = true)
        {
            var family = new FontFamily(fontName);
            MainTextBox.FontFamily = family;
            OutlinedText.FontFamily = family;
            SettingsManager.Instance.Current.Font.Family = fontName;
            if (persist) SettingsManager.Instance.Save();
        }

        private void ApplyFontSize(double size, bool persist = true)
        {
            MainTextBox.FontSize = size;
            OutlinedText.FontSize = size;
            SettingsManager.Instance.Current.Font.Size = size;
            if (persist) SettingsManager.Instance.Save();
        }

        private void ApplyFontColor(string hex, bool persist = true)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            var brush = new SolidColorBrush(color);
            OutlinedText.FillBrush = brush;
            MainTextBox.CaretBrush = brush;
            if (!TextEffectCurrent.OutlineEnabled)
                MainTextBox.Foreground = brush;
            SettingsManager.Instance.Current.Font.Color = hex;
            if (persist) SettingsManager.Instance.Save();
        }

        private void InitializeFontColorMenu()
        {
            FontColorMenu.Items.Clear();
            foreach (var (name, hex) in FontHelper.GetPresetColors())
            {
                var rect = new Rectangle { Width = 12, Height = 12, Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)) };
                var item = new MenuItem { Header = name, Tag = hex, Icon = rect };
                item.Click += ColorPreset_Click;
                FontColorMenu.Items.Add(item);
            }
            var sep = new Separator();
            FontColorMenu.Items.Add(sep);
            var customItem = new MenuItem { Header = "사용자 지정..." };
            customItem.Click += ColorDialog_Click;
            FontColorMenu.Items.Add(customItem);
        }

        private void FontFamilyMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (_fontMenuInitialized) return;
            _fontMenuInitialized = true;

            FontFamilyMenu.Items.Clear();
            string currentFamily = MainTextBox.FontFamily.Source;
            foreach (string fontName in FontHelper.GetPresetFonts())
            {
                var item = new MenuItem
                {
                    Header = fontName,
                    Tag = fontName,
                    IsCheckable = true,
                    IsChecked = fontName == currentFamily
                };
                item.Click += FontPreset_Click;
                FontFamilyMenu.Items.Add(item);
            }
            FontFamilyMenu.Items.Add(new Separator());
            var moreItem = new MenuItem { Header = "더 보기..." };
            moreItem.Click += FontDialog_Click;
            FontFamilyMenu.Items.Add(moreItem);
        }

        private void FontPreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Tag is string fontName)
            {
                ApplyFontFamily(fontName);
                foreach (object item in FontFamilyMenu.Items)
                {
                    if (item is MenuItem m)
                        m.IsChecked = (m.Tag as string) == fontName;
                }
            }
        }

        private void FontDialog_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FontDialog())
            {
                dialog.Font = new System.Drawing.Font(
                    MainTextBox.FontFamily.Source,
                    (float)MainTextBox.FontSize);
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ApplyFontFamily(dialog.Font.FontFamily.Name);
                    ApplyFontSize(dialog.Font.Size);
                    _fontMenuInitialized = false;
                }
            }
        }

        private void FontSizePreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Tag is string tag && double.TryParse(tag, out double size))
            {
                ApplyFontSize(size);
                foreach (object item in FontSizeMenu.Items)
                {
                    if (item is MenuItem m)
                        m.IsChecked = (m.Tag as string) == tag;
                }
            }
        }

        private void FontSizeCustom_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "글자 크기를 입력하세요 (6 ~ 72):",
                "글자 크기 직접 입력",
                MainTextBox.FontSize.ToString());
            if (string.IsNullOrWhiteSpace(input)) return;
            if (double.TryParse(input, out double size) && size >= 6 && size <= 72)
                ApplyFontSize(size);
        }

        private void ColorPreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Tag is string hex)
                ApplyFontColor(hex);
        }

        private void ColorDialog_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.ColorDialog { FullOpen = true })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var c = dialog.Color;
                    ApplyFontColor(string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B));
                }
            }
        }

        // 비정상 종료 시 App.xaml.cs에서 호출
        public string GetMemoText() => MainTextBox?.Text ?? string.Empty;

        public void EmergencyDisposeTray()
        {
            try { _trayIconManager?.Dispose(); _trayIconManager = null; }
            catch { }
        }
    }
}
