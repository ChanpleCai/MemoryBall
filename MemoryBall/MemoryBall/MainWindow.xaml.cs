using System;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static MemoryBall.SafeNativeMethods;

namespace MemoryBall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private readonly System.Timers.Timer _infoUpdatetimer;
        private Memorystatusex _mEmorystatusex;
        private readonly MemoryInfo _memoryInfo;

        public MainWindow()
        {
            InitializeComponent();
            _mEmorystatusex = new Memorystatusex();
            _mEmorystatusex.dwLength = (uint)Marshal.SizeOf(_mEmorystatusex);
            _memoryInfo = new MemoryInfo();
            MainGrid.DataContext = _memoryInfo;
            _infoUpdatetimer = new System.Timers.Timer(1000);
            _infoUpdatetimer.Elapsed += InfoUpdatetimer_Elapsed;
            _infoUpdatetimer.Start();
        }

        private void InfoUpdatetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GlobalMemoryStatusEx(out _mEmorystatusex);
            _memoryInfo.MemLoad = _mEmorystatusex.dwMemoryLoad;
            _memoryInfo.FillColor = "LawnGreen";
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
            MainBorder.Opacity = 0.7;
            MainGrid.Opacity = 1;
            _memoryInfo.ToolTipMessage =
                $"占用：{(((_mEmorystatusex.ullTotalPhys - _mEmorystatusex.ullAvailPhys) / 1073741824.0)):F1}/{((_mEmorystatusex.ullTotalPhys / 1073741824.0)):F1} G\r\n" +
                $"提交：{(((_mEmorystatusex.ullTotalPageFile - _mEmorystatusex.ullAvailPageFile) / 1073741824.0)):F1}/{((_mEmorystatusex.ullTotalPageFile / 1073741824.0)):F1} G";
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            MainBorder.Opacity = 0.3;
            MainGrid.Opacity = 0.8;
        }

        private async void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _infoUpdatetimer.Stop();
                    await Task.Run(() =>
                    {
                        var temp = _memoryInfo.MemLoad;
                        for (int i = temp; i >= 0; i -= 2)
                        {
                            _memoryInfo.MemLoad = i;
                            Thread.Sleep(15);
                        }

                        Process.Start("ie4uinit", "-show");

                        GC.Collect();
                        GC.WaitForFullGCComplete();

                        for (int i = 0; i <= temp; i += 2)
                        {
                            Thread.Sleep(15);
                            _memoryInfo.MemLoad = i;
                        }

                        _memoryInfo.MemLoad = temp;
                    });
                    _infoUpdatetimer.Start();
                    break;
                case MouseButton.Right:
                    Process.Start("taskmgr");
                    break;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var temp = e.Data.GetData(DataFormats.FileDrop);
            if (temp == null)
            {
                return;
            }

            FileSystem.DeleteFile(
                ((Array)temp).GetValue(0).ToString(),
                UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isLeftCtrlDown)
            {
                var handle = new WindowInteropHelper(this).Handle;
                if (e.Delta > 0)
                {
                    SendMessageW(handle, 0x319, handle, (IntPtr)0xA0000);
                    return;
                }

                SendMessageW(handle, 0x319, handle, (IntPtr)0x90000);
                return;
            }

            int currentBrightness = 50;
            //https://blogs.technet.microsoft.com/heyscriptingguy/2013/07/25/use-powershell-to-report-and-set-monitor-brightness/
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope("root\\WMI"), new SelectQuery("WmiMonitorBrightness")))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (var o in objectCollection)
                    {
                        currentBrightness = Convert.ToInt32(((ManagementObject)o).Properties["CurrentBrightness"].Value);
                        break;
                    }
                }
            }

            if (e.Delta > 0)
            {
                if (currentBrightness >= 100)
                {
                    _memoryInfo.MemLoad = currentBrightness;
                    _memoryInfo.FillColor = "DeepPink";
                    _infoUpdatetimer.Start();
                    return;
                }

                currentBrightness += 5;
                if (currentBrightness > 100)
                {
                    currentBrightness = 100;
                }
            }
            else
            {
                if (currentBrightness <= 0)
                {
                    _memoryInfo.MemLoad = currentBrightness;
                    _memoryInfo.FillColor = "DeepPink";
                    _infoUpdatetimer.Start();
                    return;
                }

                currentBrightness -= 5;
                if (currentBrightness < 0)
                {
                    currentBrightness = 0;
                }
            }

            _infoUpdatetimer.Stop();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope("root\\WMI"), new SelectQuery("WmiMonitorBrightnessMethods")))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (var o in objectCollection)
                    {
                        ((ManagementObject)o).InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, currentBrightness });
                        _memoryInfo.MemLoad = currentBrightness;
                        _memoryInfo.FillColor = "DeepPink";
                        break;
                    }
                }
            }
            _infoUpdatetimer.Start();
        }

        private void MainWindow_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Guid.NewGuid().ToString());
        }


        private static bool _isLeftCtrlDown;
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                _isLeftCtrlDown = true;
            }
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                _isLeftCtrlDown = false;
            }
        }
    }
}
