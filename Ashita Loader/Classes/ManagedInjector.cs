
/**
 * ManagedInjector.cs - Managed DLL Injection Functions
 * -----------------------------------------------------------------------
 * (c) 2012 atom0s [atom0s@live.com]
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
 * -----------------------------------------------------------------------
 * 
 * This is a slimed down version of a managed injection wrapper I wrote for
 * previous projects of mine. This has been made to work with Ashita and
 * has had all exception handling and logging removed.
 * 
 * ~ atom0s
 * 
 */

namespace Ashita.Classes
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// ManagedInjector
    /// 
    /// Static class that exposes helper functions used to make 
    /// injection in C# easy.
    /// 
    /// (c) 2012 atom0s [atom0s@live.com]
    /// </summary>
    public static class ManagedInjector
    {
        /// <summary>
        /// Creates a process in a suspended state preparing it for injection.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Int32 CreateSuspended(String filePath, String arguments = "")
        {
            var pi = new NativeMethods.PROCESS_INFORMATION();
            var si = new NativeMethods.STARTUPINFO();

            try
            {
                // Attempt to launch the file in a suspended state..
                return NativeMethods.CreateProcess(filePath, arguments, IntPtr.Zero, IntPtr.Zero, false, NativeMethods.NORMAL_PRIORITY_CLASS | NativeMethods.CREATE_SUSPENDED, IntPtr.Zero, Path.GetDirectoryName(filePath), ref si, out pi) ? pi.dwProcessId : 0;
            }
            catch
            {
                return 0;
            }
            finally
            {
                // Cleanup handles if created..
                if (pi.hProcess != IntPtr.Zero)
                    NativeMethods.CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero)
                    NativeMethods.CloseHandle(pi.hThread);
            }
        }

        /// <summary>
        /// Injects a module into the given process.
        /// </summary>
        /// <param name="procId"></param>
        /// <param name="moduleName"></param>
        /// <param name="suspendProcess"></param>
        /// <returns></returns>
        public static Boolean InjectModule(Int32 procId, String moduleName, Boolean suspendProcess = true)
        {
            // Validate the process is running.
            if (!IsProcessRunning(procId))
                return false;

            // Validate the module exists..
            if (moduleName.Length == 0 || !File.Exists(moduleName))
                return false;

            // Prepare variables for injection..
            var lpAllocMemory = IntPtr.Zero;
            var lpRemoteThread = IntPtr.Zero;
            Process proc = null;
            try
            {
                // Obtain the target process..
                proc = Process.GetProcessById(procId);

                // Suspend the process if needed..
                if (suspendProcess && !SuspendProcess(procId))
                    return false;

                // Allocate memory in the remote process..
                lpAllocMemory = NativeMethods.VirtualAllocEx(proc.Handle, IntPtr.Zero, (uint)moduleName.Length, NativeMethods.AllocationType.Commit, NativeMethods.MemoryProtection.ExecuteReadWrite);
                if (lpAllocMemory == IntPtr.Zero) throw new Exception("Injection: Failed to allocate memory in the remote process.");

                // Attempt to write module path to remote process..
                int dataWritten;
                var dataToWrite = Encoding.Unicode.GetBytes(moduleName);
                if (!NativeMethods.WriteProcessMemory(proc.Handle, lpAllocMemory, dataToWrite, (uint)dataToWrite.Length, out dataWritten) || dataWritten != dataToWrite.Length)
                    throw new Exception("Injection: Failed to write module path in remote process.");

                // Create remote thread to load injected module..
                lpRemoteThread = NativeMethods.CreateRemoteThread(proc.Handle, IntPtr.Zero, 0, NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32"), "LoadLibraryW"), lpAllocMemory, 0, IntPtr.Zero);
                if (lpRemoteThread == IntPtr.Zero)
                    throw new Exception("Injection: Failed to create remote thread in target process.");

                // Obtain thread exit code to check if injection worked..
                uint uiExitThreadCode;
                NativeMethods.WaitForSingleObject(lpRemoteThread, 10000);
                NativeMethods.GetExitCodeThread(lpRemoteThread, out uiExitThreadCode);
                if (uiExitThreadCode == 0 || uiExitThreadCode == NativeMethods.STILL_ACTIVE)
                    throw new Exception("Injection: Thread did not return valid address for loaded module.");

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                // Cleanup objects..
                if (lpRemoteThread != IntPtr.Zero)
                    NativeMethods.CloseHandle(lpRemoteThread);
                if (lpAllocMemory != IntPtr.Zero && proc != null)
                    NativeMethods.VirtualFreeEx(proc.Handle, lpAllocMemory, (uint)moduleName.Length, NativeMethods.AllocationType.Decommit);

                // Resume process if we wanted it suspended..
                if (suspendProcess)
                    ResumeProcess(procId);
            }
        }

        /// <summary>
        /// Determines if a procecss with the given id is running.
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        private static Boolean IsProcessRunning(Int32 procId)
        {
            try
            {
                var p = Process.GetProcessById(procId);
                return (p.HasExited == false);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Resumes a suspended process.
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        public static Boolean ResumeProcess(Int32 procId)
        {
            try
            {
                // Open the thread for modifications..
                var p = Process.GetProcessById(procId);
                var t = NativeMethods.OpenThread(NativeMethods.ThreadAccess.SUSPEND_RESUME, false, (uint)p.Threads[0].Id);
                if (t == IntPtr.Zero) return false;

                // Resume the process and cleanup..
                NativeMethods.ResumeThread(t);
                NativeMethods.CloseHandle(t);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suspends a process.
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        public static Boolean SuspendProcess(Int32 procId)
        {
            try
            {
                // Open the thread for modifications..
                var p = Process.GetProcessById(procId);
                var t = NativeMethods.OpenThread(NativeMethods.ThreadAccess.SUSPEND_RESUME, false, (uint)p.Threads[0].Id);
                if (t == IntPtr.Zero) return false;

                // Resume the process and cleanup..
                NativeMethods.SuspendThread(t);
                NativeMethods.CloseHandle(t);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kills a process by its process id.
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        public static Boolean KillProcess(Int32 procId)
        {
            try
            {
                Process.GetProcessById(procId).Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
