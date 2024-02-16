using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS.Surveillance.Monitor
{
    //-----------------------------------------------------------------------
    // <copyright file="ProcessExtensions.cs" company="DockOfTheBay">
    //     http://www.dotbay.be
    // </copyright>
    // <summary>Defines the ProcessExtensions class.</summary>
    //-----------------------------------------------------------------------

    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Extension Methods for the System.Diagnostics.Process Class.
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Returns the Parent Process of a Process
        /// </summary>
        /// <param name="process">The Windows Process.</param>
        /// <returns>The Parent Process of the Process.</returns>
        public static Process ParentProcess(this Process process)
        {
            int parentPid = 0;
            int processPid = process.Id;
            uint TH32CS_SNAPPROCESS = 2;

            // Take snapshot of processes
            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (hSnapshot == IntPtr.Zero)
            {
                return null;
            }

            PROCESSENTRY32 procInfo = new PROCESSENTRY32();

            procInfo.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

            // Read first
            if (Process32First(hSnapshot, ref procInfo) == false)
            {
                return null;
            }

            // Loop through the snapshot
            do
            {
                // If it's me, then ask for my parent.
                if (processPid == procInfo.th32ProcessID)
                {
                    parentPid = (int)procInfo.th32ParentProcessID;
                }
            }
            while (parentPid == 0 && Process32Next(hSnapshot, ref procInfo)); // Read next

            if (parentPid > 0)
            {
                return Process.GetProcessById(parentPid);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Takes a snapshot of the specified processes, as well as the heaps, 
        /// modules, and threads used by these processes.
        /// </summary>
        /// <param name="dwFlags">
        /// The portions of the system to be included in the snapshot.
        /// </param>
        /// <param name="th32ProcessID">
        /// The process identifier of the process to be included in the snapshot.
        /// </param>
        /// <returns>
        /// If the function succeeds, it returns an open handle to the specified snapshot.
        /// If the function fails, it returns INVALID_HANDLE_VALUE.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        /// <summary>
        /// Retrieves information about the first process encountered in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>
        /// Returns TRUE if the first entry of the process list has been copied to the buffer.
        /// Returns FALSE otherwise.
        /// </returns>
        [DllImport("kernel32.dll")]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// Retrieves information about the next process recorded in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>
        /// Returns TRUE if the next entry of the process list has been copied to the buffer.
        /// Returns FALSE otherwise.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        /// <summary>
        /// Describes an entry from a list of the processes residing 
        /// in the system address space when a snapshot was taken.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
    }
}

