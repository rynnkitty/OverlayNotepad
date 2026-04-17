using System.Runtime.Serialization;

namespace OverlayNotepad.Models
{
    [DataContract]
    public class AppSettings
    {
        [DataMember(Name = "window")]
        public WindowSettings Window { get; set; }

        [DataMember(Name = "transparency")]
        public TransparencySettings Transparency { get; set; }

        [DataMember(Name = "textEffect")]
        public TextEffectConfig TextEffect { get; set; }

        [DataMember(Name = "font")]
        public FontSettings Font { get; set; }

        [DataMember(Name = "topmost")]
        public bool Topmost { get; set; }

        [DataMember(Name = "theme")]
        public string Theme { get; set; }

        public static AppSettings CreateDefault()
        {
            return new AppSettings
            {
                Window = new WindowSettings { Left = 0, Top = 0, Width = 400, Height = 300 },
                Transparency = new TransparencySettings { Opacity = 0.8, Mode = "background" },
                TextEffect = new TextEffectConfig
                {
                    OutlineEnabled = true,
                    OutlineThickness = 1.0,
                    OutlineColor = "#000000",
                    ShadowEnabled = true,
                    ShadowBlur = 5.0,
                    ShadowOffset = 1.5,
                    ShadowOpacity = 0.8
                },
                Font = new FontSettings { Family = "맑은 고딕", Size = 14, Color = "#FFFFFF" },
                Topmost = true,
                Theme = "dark"
            };
        }

        [DataContract]
        public class WindowSettings
        {
            [DataMember(Name = "left")]
            public double Left { get; set; }

            [DataMember(Name = "top")]
            public double Top { get; set; }

            [DataMember(Name = "width")]
            public double Width { get; set; } = 400;

            [DataMember(Name = "height")]
            public double Height { get; set; } = 300;
        }

        [DataContract]
        public class TransparencySettings
        {
            [DataMember(Name = "opacity")]
            public double Opacity { get; set; } = 0.8;

            [DataMember(Name = "mode")]
            public string Mode { get; set; } = "background";
        }

        [DataContract]
        public class TextEffectConfig
        {
            [DataMember(Name = "outlineEnabled")]
            public bool OutlineEnabled { get; set; } = true;

            [DataMember(Name = "outlineThickness")]
            public double OutlineThickness { get; set; } = 1.0;

            [DataMember(Name = "outlineColor")]
            public string OutlineColor { get; set; } = "#000000";

            [DataMember(Name = "shadowEnabled")]
            public bool ShadowEnabled { get; set; } = true;

            [DataMember(Name = "shadowBlur")]
            public double ShadowBlur { get; set; } = 5.0;

            [DataMember(Name = "shadowOffset")]
            public double ShadowOffset { get; set; } = 1.5;

            [DataMember(Name = "shadowOpacity")]
            public double ShadowOpacity { get; set; } = 0.8;
        }

        [DataContract]
        public class FontSettings
        {
            [DataMember(Name = "family")]
            public string Family { get; set; } = "맑은 고딕";

            [DataMember(Name = "size")]
            public double Size { get; set; } = 14;

            [DataMember(Name = "color")]
            public string Color { get; set; } = "#FFFFFF";
        }
    }
}
