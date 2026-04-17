using System.Windows;

namespace OverlayNotepad.Helpers
{
    public static class ScreenHelper
    {
        public static bool IsWithinScreenBounds(double left, double top, double width, double height)
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenRight = screenLeft + SystemParameters.VirtualScreenWidth;
            double screenBottom = screenTop + SystemParameters.VirtualScreenHeight;

            // 윈도우의 최소 50%가 화면 내에 있는지 확인
            double halfWidth = width / 2;
            double halfHeight = height / 2;

            return (left + halfWidth) >= screenLeft &&
                   (left + halfWidth) <= screenRight &&
                   (top + halfHeight) >= screenTop &&
                   (top + halfHeight) <= screenBottom;
        }

        public static (double left, double top) GetDefaultPosition(double width, double height)
        {
            double left = (SystemParameters.VirtualScreenWidth - width) / 2 + SystemParameters.VirtualScreenLeft;
            double top = (SystemParameters.VirtualScreenHeight - height) / 2 + SystemParameters.VirtualScreenTop;
            return (left, top);
        }
    }
}
