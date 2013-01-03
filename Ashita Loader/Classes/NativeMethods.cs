
/**
 * NativeAPI.cs - Win32 Native API Definitions
 * -----------------------------------------------------------------------
 * 
 *		This file is part of Ashita.
 *
 *		Ashita is free software: you can redistribute it and/or modify
 *		it under the terms of the GNU Lesser General Public License as published by
 *		the Free Software Foundation, either version 3 of the License, or
 *		(at your option) any later version.
 *
 *		Ashita is distributed in the hope that it will be useful,
 *		but WITHOUT ANY WARRANTY; without even the implied warranty of
 *		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *		GNU Lesser General Public License for more details.
 *
 *		You should have received a copy of the GNU Lesser General Public License
 *		along with Ashita.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

namespace Ashita.Classes
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native Win32 API Definitions
    /// 
    /// API needed in order to do the required tasks of
    /// loading, injection, and remote export calling.
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// kernel32.CreateFileMapping
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpAttributes"></param>
        /// <param name="flProtect"></param>
        /// <param name="dwMaximumSizeLow"></param>
        /// <param name="dwMaximumSizeHigh"></param>
        /// <param name="lpName"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpAttributes,
            int flProtect,
            int dwMaximumSizeLow,
            int dwMaximumSizeHigh,
            String lpName
            );

        /// <summary>
        /// kernel32.FlushViewOfFile
        /// </summary>
        /// <param name="lpBaseAddress"></param>
        /// <param name="dwNumBytesToFlush"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FlushViewOfFile(
            IntPtr lpBaseAddress,
            uint dwNumBytesToFlush
            );

        /// <summary>
        /// kernel32.MapViewOfFile
        /// </summary>
        /// <param name="hFileMappingObject"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwFileOffsetHigh"></param>
        /// <param name="dwFileOffsetLow"></param>
        /// <param name="dwNumBytesToMap"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            int dwDesiredAccess,
            int dwFileOffsetHigh,
            int dwFileOffsetLow,
            int dwNumBytesToMap
            );

        /// <summary>
        /// kernel32.OpenFileMapping
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="lpName"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenFileMapping(
            int dwDesiredAccess,
            bool bInheritHandle,
            String lpName
            );

        /// <summary>
        /// kernel32.UnmapViewOfFile
        /// </summary>
        /// <param name="lpBaseAddress"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool UnmapViewOfFile(
            IntPtr lpBaseAddress
            );

        /// <summary>
        /// kernel32.CloseHandle
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
            IntPtr hObject
            );

        /// <summary>
        /// kernel32.CreateProcess
        /// </summary>
        /// <param name="lpApplicationName"></param>
        /// <param name="lpCommandLine"></param>
        /// <param name="lpProcessAttributes"></param>
        /// <param name="lpThreadAttributes"></param>
        /// <param name="bInheritHandles"></param>
        /// <param name="dwCreationFlags"></param>
        /// <param name="lpEnvironment"></param>
        /// <param name="lpCurrentDirectory"></param>
        /// <param name="lpStartupInfo"></param>
        /// <param name="lpProcessInformation"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateProcess(
            String lpApplicationName,
            String lpCommandLine,
            [In] IntPtr lpProcessAttributes,
            [In] IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            String lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation
            );

        /// <summary>
        /// kernel32.CreateRemoteThread
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpThreadAttributes"></param>
        /// <param name="dwStackSize"></param>
        /// <param name="lpStartAddress"></param>
        /// <param name="lpParameter"></param>
        /// <param name="dwCreationFlags"></param>
        /// <param name="lpThreadId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId
            );

        /// <summary>
        /// kernel32.GetExitCodeThread
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="lpExitCode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool GetExitCodeThread(
            IntPtr hThread,
            out uint lpExitCode
            );

        /// <summary>
        /// kernel32.GetModuleHandle
        /// </summary>
        /// <param name="lpModuleName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName
            );

        /// <summary>
        /// kernel32.GetProcAddress
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName
            );

        /// <summary>
        /// kernel32.LoadLibraryA
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
        public static extern IntPtr LoadLibrary(
            string lpFileName
            );

        /// <summary>
        /// kernel32.OpenThread
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwThreadId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(
            ThreadAccess dwDesiredAccess,
            bool bInheritHandle,
            uint dwThreadId
            );


        /// <summary>
        /// kernel32.ResumeThread
        /// </summary>
        /// <param name="hThread"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(
            IntPtr hThread
            );

        /// <summary>
        /// kernel32.SuspendThread
        /// </summary>
        /// <param name="hThread"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(
            IntPtr hThread
            );

        /// <summary>
        /// kernel32.TerminateThread
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="dwExitCode"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool TerminateThread(
            IntPtr hThread,
            uint dwExitCode
            );

        /// <summary>
        /// kernel32.VirtualAllocEx
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpAddress"></param>
        /// <param name="dwSize"></param>
        /// <param name="flAllocationType"></param>
        /// <param name="flProtect"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect
            );

        /// <summary>
        /// kernel32.VirtualFreeEx
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpAddress"></param>
        /// <param name="dwSize"></param>
        /// <param name="flFreeType"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            AllocationType flFreeType
            );

        /// <summary>
        /// kernel32.WaitForSingleObject
        /// </summary>
        /// <param name="hHandle"></param>
        /// <param name="dwMilliseconds"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(
            IntPtr hHandle,
            UInt32 dwMilliseconds
            );

        /// <summary>
        /// kernel32.WriteProcessMemory
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpBaseAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nSize"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            uint nSize,
            out int lpNumberOfBytesWritten
            );


        /// <summary>
        /// Virtual memory allocation types.
        /// </summary>
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        /// <summary>
        /// Virtual memory protection types.
        /// </summary>
        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        /// <summary>
        /// Thread access flags.
        /// </summary>
        [Flags]
        public enum ThreadAccess
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        /// <summary>
        /// Process creation startup information structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        /// <summary>
        /// Process creation information structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        /// <summary>
        /// Process creation security attributes structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        /// <summary>
        /// CreateProcess creation flags.
        /// </summary>
        public const UInt32 DEBUG_PROCESS = 0x00000001;
        public const UInt32 DEBUG_ONLY_THIS_PROCESS = 0x00000002;
        public const UInt32 CREATE_SUSPENDED = 0x00000004;
        public const UInt32 DETACHED_PROCESS = 0x00000008;
        public const UInt32 CREATE_NEW_CONSOLE = 0x00000010;

        /// <summary>
        /// CreateProcess priority constants.
        /// </summary>
        public const UInt32 NORMAL_PRIORITY_CLASS = 0x00000020;
        public const UInt32 IDLE_PRIORITY_CLASS = 0x00000040;
        public const UInt32 HIGH_PRIORITY_CLASS = 0x00000080;
        public const UInt32 REALTIME_PRIORITY_CLASS = 0x00000100;

        /// <summary>
        /// WaitForSingleObject constants.
        /// </summary>
        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        /// <summary>
        /// GetExitCodeThread constants.
        /// </summary>
        public const UInt32 STILL_ACTIVE = 0x00000103;
    }
}