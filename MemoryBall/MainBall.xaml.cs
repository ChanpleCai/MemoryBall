using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.VisualBasic.FileIO;
using static MemoryBall.SafeNativeMethods;

namespace MemoryBall
{
    /// <summary>
    ///     MainBall.xaml 的交互逻辑
    /// </summary>
    public partial class MainBall
    {
        private readonly Timer _infoUpdatetimer;
        private Memorystatusex _mEmorystatusex;
        private readonly SysInfo _memoryInfo;

        public MainBall()
        {
            InitializeComponent();
            _mEmorystatusex = new Memorystatusex();
            _mEmorystatusex.dwLength = (uint)Marshal.SizeOf(_mEmorystatusex);
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
            //_memoryInfo.FillColor = SystemParameters.WindowGlassBrush;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth - 2 * Width;
            Top = Height;

            //https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher
            var handle = new WindowInteropHelper(this).Handle;
            SetWindowLong(handle, -20, (IntPtr)((int)GetWindowLong(handle, -20) | 0x00000080));
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            const double d = 1073741824.0;
            _memoryInfo.ToolTipMessage =
                $"占用：{(_mEmorystatusex.ullTotalPhys - _mEmorystatusex.ullAvailPhys) / d:F1}/{_mEmorystatusex.ullTotalPhys / d:F1} G\r\n" +
                $"提交：{(_mEmorystatusex.ullTotalPageFile - _mEmorystatusex.ullAvailPageFile) / d:F1}/{_mEmorystatusex.ullTotalPageFile / d:F1} G";
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var temp = e.Data.GetData(DataFormats.FileDrop);
            if (temp == null) return;

            FileSystem.DeleteFile(
                ((Array)temp).GetValue(0)?.ToString(),
                UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }

        private async void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _infoUpdatetimer.Stop();

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    await Task.Run(async () =>
                    {
                        var temp = _memoryInfo.MemLoad;
                        for (int i = temp; i >= 0; i -= 2)
                        {
                            _memoryInfo.MemLoad = i;
                            await Task.Delay(15);
                        }

                        Process.Start("ie4uinit", "-show");

                        for (int i = 0; i <= temp; i += 2)
                        {
                            await Task.Delay(15);
                            _memoryInfo.MemLoad = i;
                        }

                        _memoryInfo.MemLoad = temp;
                    });
                    break;
                case MouseButton.Right:
                    Environment.Exit(0);
                    break;
            }


            _infoUpdatetimer.Start();
        }
    }
}