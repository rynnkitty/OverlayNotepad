using System.Windows.Media;

namespace OverlayNotepad.Models
{
    public class TextEffectSettings
    {
        public bool OutlineEnabled { get; set; } = true;
        public double OutlineThickness { get; set; } = 1.0;
        public Color OutlineColor { get; set; } = Colors.Black;

        public bool ShadowEnabled { get; set; } = true;
        public double ShadowBlurRadius { get; set; } = 5.0;
        public double ShadowOffsetX { get; set; } = 1.5;
        public double ShadowOffsetY { get; set; } = 1.5;
        public double ShadowOpacity { get; set; } = 0.8;
    }
}
