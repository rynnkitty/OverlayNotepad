using System.Windows;
using System.Windows.Input;

namespace OverlayNotepad
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // FocusManager 실패 시 폴백: 직접 포커스 설정
            MainTextBox.Focus();
        }

        private void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
