using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static MemoryBall.SafeNativeMethods;

namespace MemoryBall
{
    /// <summary>
    ///     MainBall.xaml 的交互逻辑
    /// </summary>
    public partial class MainBall
    {
        private readonly Timer _infoUpdatetimer;
        private readonly SysInfo _memoryInfo;
        private Memorystatusex _mEmorystatusex;

        public MainBall()
        {
            InitializeComponent();
            _mEmorystatusex = new Memorystatusex();
            _mEmorystatusex.dwLength = (uint) Marshal.SizeOf(_mEmorystatusex);
            _memoryInfo = new SysInfo();
            MainGrid.DataContext = _memoryInfo;
            _infoUpdatetimer = new Timer(1000);
            _infoUpdatetimer.Elapsed += InfoUpdatetimer_Elapsed;
            _infoUpdatetimer.Start();
        }

        private void InfoUpdatetimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GlobalMemoryStatusEx(out _mEmorystatusex);
            _memoryInfo.MemLoad = _mEmorystatusex.dwMemoryLoad;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => DragMove();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth - 2 * Width;
            Top = Height;

            //https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher
            var handle = new WindowInteropHelper(this).Handle;
            SetWindowLong(handle, -20, (IntPtr) ((int) GetWindowLong(handle, -20) | 0x00000080));
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            const double d = 1073741824.0;
            _memoryInfo.ToolTipMessage =
                $"占用：{(_mEmorystatusex.ullTotalPhys - _mEmorystatusex.ullAvailPhys) / d:F1}/{_mEmorystatusex.ullTotalPhys / d:F1} G\r\n"
                + $"提交：{(_mEmorystatusex.ullTotalPageFile - _mEmorystatusex.ullAvailPageFile) / d:F1}/{_mEmorystatusex.ullTotalPageFile / d:F1} G";
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _infoUpdatetimer.Stop();

            if (e.ChangedButton == MouseButton.Right)
                Environment.Exit(0);

            _infoUpdatetimer.Start();
        }
    }
}
