using System.Windows.Media;

namespace OverlayNotepad.Models
{
    public class ThemeDefinition
    {
        public string Name { get; set; }
        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public double BackgroundOpacity { get; set; }
        public Color OutlineColor { get; set; }
        public Color ShadowColor { get; set; }
        public Color DragBarColor { get; set; }
        public double DragBarOpacity { get; set; }

        public static ThemeDefinition CreateDark()
        {
            return new ThemeDefinition
            {
                Name = "dark",
                TextColor = Colors.White,
                BackgroundColor = Color.FromRgb(0x1E, 0x1E, 0x1E),
                BackgroundOpacity = 0.8,
                OutlineColor = Colors.Black,
                ShadowColor = Colors.Black,
                DragBarColor = Color.FromRgb(0x33, 0x33, 0x33),
                DragBarOpacity = 0.9
            };
        }

        public static ThemeDefinition CreateLight()
        {
            return new ThemeDefinition
            {
                Name = "light",
                TextColor = Colors.Black,
                BackgroundColor = Colors.White,
                BackgroundOpacity = 0.8,
                OutlineColor = Colors.White,
                ShadowColor = Color.FromRgb(0x80, 0x80, 0x80),
                DragBarColor = Color.FromRgb(0xE0, 0xE0, 0xE0),
                DragBarOpacity = 0.9
            };
        }
    }
}
