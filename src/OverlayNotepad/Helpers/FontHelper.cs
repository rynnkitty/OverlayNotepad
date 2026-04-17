using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;

namespace OverlayNotepad.Helpers
{
    public static class FontHelper
    {
        private static readonly HashSet<string> _installedNames = LoadInstalledNames();

        private static HashSet<string> LoadInstalledNames()
        {
            using (var fc = new InstalledFontCollection())
            {
                return new HashSet<string>(fc.Families.Select(f => f.Name));
            }
        }

        public static bool IsInstalled(string fontName)
        {
            return _installedNames.Contains(fontName);
        }

        public static List<string> GetPresetFonts()
        {
            var presets = new[]
            {
                "맑은 고딕", "나눔고딕", "나눔바른고딕", "D2Coding", "Consolas", "Arial", "굴림"
            };
            return presets.Where(IsInstalled).ToList();
        }

        public static List<double> GetPresetSizes()
        {
            return new List<double> { 10, 12, 14, 16, 20, 24, 32 };
        }

        public static List<(string Name, string Hex)> GetPresetColors()
        {
            return new List<(string, string)>
            {
                ("흰색",  "#FFFFFF"),
                ("검정",  "#000000"),
                ("빨강",  "#FF0000"),
                ("파랑",  "#0000FF"),
                ("초록",  "#00FF00"),
                ("노랑",  "#FFFF00"),
                ("회색",  "#808080"),
                ("주황",  "#FFA500"),
                ("보라",  "#800080"),
                ("하늘",  "#87CEEB"),
            };
        }
    }
}
