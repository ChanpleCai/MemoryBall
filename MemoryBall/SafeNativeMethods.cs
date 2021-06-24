using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MemoryBall
{
    [SuppressUnmanagedCodeSecurity]
    public static class SafeNativeMethods
    {
        #region GlobalMemoryStatusEx

        //https://msdn.microsoft.com/en-us/library/aa366589(VS.85).aspx

        [StructLayout(LayoutKind.Sequential)]
        public struct Memorystatusex
        {
            /// DWORD->unsigned int
            /// The size of the structure, in bytes. You must set this member before calling GlobalMemoryStatusEx.
            public uint dwLength;

            /// DWORD->unsigned int
            /// A number between 0 and 100 that specifies the approximate percentage of physical memory that is in use 
            /// (0 indicates no memory use and 100 indicates full memory use).
            public int dwMemoryLoad;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// The amount of actual physical memory, in bytes.
            public ulong ullTotalPhys;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// The amount of physical memory currently available, in bytes. 
            /// This is the amount of physical memory that can be immediately reused without having to write its contents to disk first. 
            /// It is the sum of the size of the standby, free, and zero lists.
            public ulong ullAvailPhys;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// The current committed memory limit for the system or the current process, whichever is smaller, in bytes. 
            /// To get the system-wide committed memory limit, call GetPerformanceInfo.
            public ulong ullTotalPageFile;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// The maximum amount of memory the current process can commit, in bytes. 
            /// This value is equal to or smaller than the system-wide available commit value. 
            /// To calculate the system-wide available commit value, 
            /// call GetPerformanceInfo and subtract the value of CommitTotal from the value of CommitLimit.
            public ulong ullAvailPageFile;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// The size of the user-mode portion of the virtual address space of the calling process, in bytes. 
            /// This value depends on the type of process, the type of processor, and the configuration of the operating system. 
            /// For example, this value is approximately 2 GB for most 32-bit processes on an x86 processor and 
            /// approximately 3 GB for 32-bit processes that are large address aware running on a system with 4-gigabyte tuning enabled.
            public ulong ullTotalVirtual;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// The amount of unreserved and uncommitted memory currently in the user-mode portion of the virtual address space of the calling process, in bytes.
            public ulong ullAvailVirtual;

            /// DWORDLONG->ULONGLONG->unsigned __int64
            /// Reserved. This value is always 0.
            public ulong ullAvailExtendedVirtual;
        }

        /// Return Type: BOOL->int
        /// lpBuffer: LPMEMORYSTATUSEX->_MEMORYSTATUSEX*
        [DllImport("kernel32.dll", EntryPoint = "GlobalMemoryStatusEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx([Out] out Memorystatusex lpBuffer);

        #endregion

        #region Window styles

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static void SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
                // use SetWindowLong
                IntSetWindowLong(hWnd, nIndex, dwNewLong);
            else
                // use SetWindowLongPtr
                IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr IntSetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        #endregion
    }
}
