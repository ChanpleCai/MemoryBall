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
        private static int Index;
        private static string LastCpuLoad;
        private static string LastNetLoad;

        private static long LastNetSend;
        private static long LastNetReceived;

        private static Memorystatusex MemoryStatus;
        private const double ThreeTimes = 1073741824;

        private static readonly float[] CpuUsages = new float[Capacity];

        private static readonly PerformanceCounter CpuCounter = new("Processor", "% Processor Time", "_Total");

        private static NetworkInterface[] NetInterfaces;

        /// <summary>
        ///     ctor
        /// </summary>
        static PerformanceHelper()
        {
            MemoryStatus = new Memorystatusex();
            MemoryStatus.dwLength = (uint) Marshal.SizeOf(MemoryStatus);


            if (NetworkInterface.GetIsNetworkAvailable())
                NetInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        }

        public static string SetCpuLoad()
        {
            CpuUsages[Index] = CpuCounter.NextValue();

            if (++Index < Capacity)
                return LastCpuLoad;

            Index = 0;
            return LastCpuLoad = $"{CpuUsages.Max():F0}%";
        }

        public static int SetMemLoad()
        {
            if (Index == 0)
                _ = GlobalMemoryStatusEx(out MemoryStatus);

            return MemoryStatus.dwMemoryLoad;
        }

        public static string GetToolTipMessage()
            => $"占用：{(MemoryStatus.ullTotalPhys - MemoryStatus.ullAvailPhys) / ThreeTimes:F1}/{MemoryStatus.ullTotalPhys / ThreeTimes:F1} G\r\n"
               + $"提交：{(MemoryStatus.ullTotalPageFile - MemoryStatus.ullAvailPageFile) / ThreeTimes:F1}/{MemoryStatus.ullTotalPageFile / ThreeTimes:F1} G";

        public static string SetNetLoad()
        {
            if (Index != 0) return LastNetLoad;

            long sentValue = 0, receivedValue = 0;

            foreach (var networkInterface in NetInterfaces)
            {
                var instance = networkInterface.GetIPv4Statistics();

                sentValue += instance.BytesSent;

                receivedValue += instance.BytesReceived;
            }

            LastNetLoad = $"↑{AddUnit(sentValue - LastNetSend)}\r\n↓{AddUnit(receivedValue - LastNetReceived)}";

            LastNetSend = sentValue;
            LastNetReceived = receivedValue;

            return LastNetLoad;
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
