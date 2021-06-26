using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace MemoryBall
{
    public static class ThemeHelper
    {
        public static readonly UISettings Settings = new();

        private static readonly SolidColorBrush WhiteColor =
            new(Color.FromArgb(255, 255, 255, 255));

        // https://stackoverflow.com/questions/51334674/how-to-detect-windows-10-light-dark-mode-in-win32-application

        public static SolidColorBrush Background
            => Settings.GetColorValue(UIColorType.Background).ToSolidColorBrush();

        private static bool IsWhiteBackgroundColor
            => Background == WhiteColor;

        public static SolidColorBrush MemColor
            => Settings.GetColorValue(UIColorType.Accent).ToSolidColorBrush();

        public static SolidColorBrush CpuColor
            => IsWhiteBackgroundColor
                ? Settings.GetColorValue(UIColorType.AccentLight1).ToSolidColorBrush()
                : Settings.GetColorValue(UIColorType.Accent).ToSolidColorBrush();

        public static SolidColorBrush NetColor
            => IsWhiteBackgroundColor
                ? Settings.GetColorValue(UIColorType.Accent).ToSolidColorBrush()
                : Settings.GetColorValue(UIColorType.AccentDark1).ToSolidColorBrush();

        private static SolidColorBrush ToSolidColorBrush(this Windows.UI.Color color)
            => new(Color.FromArgb(color.A, color.R, color.G, color.B));
    }
}
