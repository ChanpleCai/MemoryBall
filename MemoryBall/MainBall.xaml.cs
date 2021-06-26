using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Windows.UI.ViewManagement;
using static MemoryBall.SafeNativeMethods;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;
using Timer = System.Timers.Timer;

namespace MemoryBall
{
    /// <summary>
    ///     MainBall.xaml 的交互逻辑
    /// </summary>
    public partial class MainBall
    {
        private const int Constant = 6;
        private const int Interval = 250;

        private static bool _isMouseEnter;
        private readonly Timer _infoUpdatetimer;
        private readonly SysInfo _sysInfo;

        public MainBall()
        {
            InitializeComponent();
            _sysInfo = new SysInfo();
            MainGrid.DataContext = _sysInfo;
            _infoUpdatetimer = new Timer(Interval);
            _infoUpdatetimer.Elapsed += InfoUpdatetimer_Elapsed;
            _infoUpdatetimer.Start();

            ThemeHelper.Settings.ColorValuesChanged += Settings_ColorValuesChanged;
            Settings_ColorValuesChanged(default, default);
        }

        private void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            _ = Dispatcher.BeginInvoke(new Action(() =>
            {
                _sysInfo.BgColor = ThemeHelper.Background;
                _sysInfo.NetColor = ThemeHelper.NetColor;
                _sysInfo.CpuColor = ThemeHelper.CpuColor;
                _sysInfo.MemColor = ThemeHelper.MemColor;
            }));
        }

        private void InfoUpdatetimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _sysInfo.MemLoad = PerformanceHelper.SetMemLoad();
            _sysInfo.CpuLoad = PerformanceHelper.SetCpuLoad();
            _sysInfo.NetLoad = PerformanceHelper.SetNetLoad();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = 0;

            //https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher
            var handle = new WindowInteropHelper(this).Handle;
            SetWindowLong(handle, -20, (IntPtr) ((int) GetWindowLong(handle, -20) | 0x00000080));
        }

        private async void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseEnter = true;
            BgBorder.Opacity = 0.125;
            await Task.Delay(Interval);

            if (!_isMouseEnter) return;

            if (Height + Top <= Constant)
            {
                Top += Height - Constant;
                _infoUpdatetimer.Start();
                InfoUpdatetimer_Elapsed(default, default);
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _infoUpdatetimer.Stop();

            if (e.ChangedButton == MouseButton.Right)
                Environment.Exit(0);

            _infoUpdatetimer.Start();
        }

        private async void MainBall_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseEnter = false;
            BgBorder.Opacity = 0.025;
            await Task.Delay(Interval * 2);

            if (_isMouseEnter) return;

            var currentScreen = Screen.FromPoint(new Point((int) Left, (int) Top));


            if (!(Top - currentScreen.WorkingArea.Top < Constant)) return;

            Top = Constant - Height;

            _infoUpdatetimer.Stop();
        }
    }
}
