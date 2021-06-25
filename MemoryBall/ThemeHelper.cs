using System.Windows.Media;
using Windows.UI.ViewManagement;
using Color = Windows.UI.Color;

namespace MemoryBall
{
    public static class ThemeHelper
    {
        public static readonly UISettings Settings = new();

        // https://stackoverflow.com/questions/51334674/how-to-detect-windows-10-light-dark-mode-in-win32-application

        public static SolidColorBrush Background
            => Settings.GetColorValue(UIColorType.Background).ToSolidColorBrush();

        public static SolidColorBrush MemColor
            => Settings.GetColorValue(UIColorType.AccentDark1).ToSolidColorBrush();

        public static SolidColorBrush CpuColor
            => Settings.GetColorValue(UIColorType.AccentLight1).ToSolidColorBrush();

        public static SolidColorBrush NetColor
            => Settings.GetColorValue(UIColorType.Accent).ToSolidColorBrush();

        private static SolidColorBrush ToSolidColorBrush(this Color color)
            => new(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
    }
}
