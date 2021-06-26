using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using static MemoryBall.SafeNativeMethods;

namespace MemoryBall
{
    public static class PerformanceHelper
    {
        private const int Capacity = 4;
        private const double ThreeTimes = 1073741824;
        private static int _index;
        private static string _lastCpuLoad;
        private static string _lastNetLoad;

        private static long _lastNetSend;
        private static long _lastNetReceived;

        private static Memorystatusex _memoryStatus;

        private static readonly float[] CpuUsages = new float[Capacity];

        private static readonly PerformanceCounter CpuCounter = new("Processor", "% Processor Time", "_Total");

        private static readonly NetworkInterface[] NetInterfaces;

        /// <summary>
        ///     ctor
        /// </summary>
        static PerformanceHelper()
        {
            _memoryStatus = new Memorystatusex();
            _memoryStatus.dwLength = (uint) Marshal.SizeOf(_memoryStatus);


            if (NetworkInterface.GetIsNetworkAvailable())
                NetInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        }

        public static string SetCpuLoad()
        {
            CpuUsages[_index] = CpuCounter.NextValue();

            if (++_index < Capacity)
                return _lastCpuLoad;

            _index = 0;
            return _lastCpuLoad = $"{CpuUsages.Max():F0}%";
        }

        public static int SetMemLoad()
        {
            if (_index == 0)
                _ = GlobalMemoryStatusEx(out _memoryStatus);

            return _memoryStatus.dwMemoryLoad;
        }

        public static string SetNetLoad()
        {
            if (_index != 0) return _lastNetLoad;

            long sentValue = 0, receivedValue = 0;

            foreach (var networkInterface in NetInterfaces)
            {
                var instance = networkInterface.GetIPv4Statistics();

                sentValue += instance.BytesSent;

                receivedValue += instance.BytesReceived;
            }

            _lastNetLoad = $"↑{AddUnit(sentValue - _lastNetSend)}\r\n↓{AddUnit(receivedValue - _lastNetReceived)}";

            _lastNetSend = sentValue;
            _lastNetReceived = receivedValue;

            return _lastNetLoad;
        }

        private static string AddUnit(long value)
            => value switch
            {
                < 1024       => $"{value} B/s",
                < 1048576    => $"{value / 1024.0:F1} KB/s",
                < 1073741824 => $"{value / 1048576:F1} MB/s",
                _            => $"{value / 1073741824:F1} GB/s"
            };
    }
}
