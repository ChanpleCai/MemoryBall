﻿using System;
using System.Management;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.PrimaryScreenWidth - 2 * Width;
            Top = Height;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            MainBorder.Opacity = 0.7;
            MainGrid.Opacity = 1;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            MainBorder.Opacity = 0.3;
            MainGrid.Opacity = 0.8;
        }

        private async void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _infoUpdatetimer.Stop();
            await Task.Run(() =>
            {
                var temp = _memoryInfo.MemLoad;
                for (int i = temp; i >= 0; i-=2)
                {
                    _memoryInfo.MemLoad = i;
                    Thread.Sleep(15);
                }
                for (int i = 0; i <= temp; i+=2)
                {
                    Thread.Sleep(15);
                    _memoryInfo.MemLoad = i;
                }

                _memoryInfo.MemLoad = temp;
            });
            _infoUpdatetimer.Start();
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
            int currentBrightness = 50;
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
                    return;
                }
                currentBrightness -= 5;
                if (currentBrightness < 0)
                {
                    currentBrightness = 0;
                }
            }



            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope("root\\WMI"), new SelectQuery("WmiMonitorBrightnessMethods")))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (var o in objectCollection)
                    {
                        ((ManagementObject)o).InvokeMethod("WmiSetBrightness",
                            new object[] { uint.MaxValue, currentBrightness});
                        break;
                    }
                }
            }
        }
    }
}
