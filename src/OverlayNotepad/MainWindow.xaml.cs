using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OverlayNotepad.Models;

namespace OverlayNotepad
{
    public partial class MainWindow : Window
    {
        private TextEffectSettings _textEffectSettings = new TextEffectSettings();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SyncOutlinedTextProperties();
            ApplyTextEffects();
            MainTextBox.Focus();
        }

        // --- 텍스트 동기화 ---

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutlinedText.Text = MainTextBox.Text;
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
            if (_textEffectSettings.OutlineEnabled)
            {
                OutlinedText.Visibility = Visibility.Visible;
                OutlinedText.OutlineThickness = _textEffectSettings.OutlineThickness;
                OutlinedText.OutlineBrush = new SolidColorBrush(_textEffectSettings.OutlineColor);
                MainTextBox.Foreground = Brushes.Transparent;
            }
            else
            {
                OutlinedText.Visibility = Visibility.Collapsed;
                MainTextBox.Foreground = Brushes.White;
            }

            TextShadowEffect.Opacity = _textEffectSettings.ShadowEnabled ? _textEffectSettings.ShadowOpacity : 0;
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
        }

        private void BackgroundTransparentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem.IsChecked)
                this.Background = Brushes.Transparent;
            else
                this.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00));
        }

        private void OutlineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            _textEffectSettings.OutlineEnabled = menuItem.IsChecked;
            ApplyTextEffects();
        }

        private void ShadowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            _textEffectSettings.ShadowEnabled = menuItem.IsChecked;
            ApplyTextEffects();
        }

        private void OpacityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clicked = sender as MenuItem;
            double opacity = double.Parse(clicked.Tag.ToString());
            this.Opacity = opacity;
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
    }
}
