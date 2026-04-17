using OverlayNotepad.Models;

namespace OverlayNotepad.Services
{
    public class ThemeManager
    {
        public static readonly ThemeManager Instance = new ThemeManager();

        public ThemeDefinition DarkTheme { get; }
        public ThemeDefinition LightTheme { get; }
        public ThemeDefinition CurrentTheme { get; private set; }
        public string CurrentThemeName => CurrentTheme.Name;

        private ThemeManager()
        {
            DarkTheme = ThemeDefinition.CreateDark();
            LightTheme = ThemeDefinition.CreateLight();
            CurrentTheme = DarkTheme;
        }

        public void SetTheme(string themeName)
        {
            CurrentTheme = (themeName == "light") ? LightTheme : DarkTheme;
        }

        public void ToggleTheme()
        {
            SetTheme(CurrentThemeName == "dark" ? "light" : "dark");
        }

        public ThemeDefinition GetTheme(string name)
        {
            return (name == "light") ? LightTheme : DarkTheme;
        }
    }
}
