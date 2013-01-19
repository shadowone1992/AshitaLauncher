
/**
 * AshitaInject.cs - Ashita injection implementation.
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
    using Ashita.Helpers;
    using Ashita.Model;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// Ashita Injection Implementation
    /// 
    /// Static class that handles the overall injection method that is used
    /// with Ashita. This class makes use of the ManagedInjector class.
    /// </summary>
    public static class AshitaInject
    {
        /// <summary>
        /// Simple message box wrapper to display error messages.
        /// </summary>
        /// <param name="strError"></param>
        private static void Error(String strError)
        {
            MessageBox.Show(strError, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Ashita injection method.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Boolean DoInjection(Configuration config)
        {
            // Ensure the configuration file exists..
            if (config == null || !File.Exists(config.FilePath))
            {
                Error("Invalid configuration file; could not launch.");
                return false;
            }

            // Obtain path to PlayOnline..
            var polPath = RegisteryHelper.GetValue<String>(String.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\PlayOnline{0}\\InstallFolder", (config.PolVersion == "JP") ? "" : config.PolVersion), "1000");
            if (string.IsNullOrEmpty(polPath))
            {
                Error("Failed to read PlayOnline path from registry.");
                return false;
            }

            // Build path to Final Fantasy registry settings..
            var ffxiPath = String.Format(
                "HKEY_LOCAL_MACHINE\\SOFTWARE\\PlayOnline{0}\\{1}\\FinalFantasyXI{2}",
                (config.PolVersion == "JP") ? "" : config.PolVersion,
                (config.PolVersion == "JP") ? "SQUARE" : "SquareEnix",
                config.TestServer ? "TestClient" : "");

            // Write FFXI window resolution..
            RegisteryHelper.SetValue(ffxiPath, "0001", config.ResolutionX);
            RegisteryHelper.SetValue(ffxiPath, "0002", config.ResolutionY);

            // Write FFXI background resolution..
            if (config.BackgroundX != -1 && config.BackgroundY != -1)
            {
                RegisteryHelper.SetValue(ffxiPath, "0003", config.BackgroundX);
                RegisteryHelper.SetValue(ffxiPath, "0004", config.BackgroundY);
            }

            // Write FFXI menu resolution..
            if (config.MenuX != -1 && config.MenuY != -1)
            {
                RegisteryHelper.SetValue(ffxiPath, "0037", config.MenuX);
                RegisteryHelper.SetValue(ffxiPath, "0038", config.MenuY);
            }

            // Write FFXI misc settings..
            RegisteryHelper.SetValue(ffxiPath, "0021", 1); // Hardware Mouse
            RegisteryHelper.SetValue(ffxiPath, "0034", (config.ShowBorder) ? 1 : 0);

            // Attempt to launch the game suspended..
            var bootFile = (config.BootFile.Length > 0) ? config.BootFile : polPath + "\\pol.exe";
            var procId = ManagedInjector.CreateSuspended(bootFile, String.Format("{0} {1}", bootFile, config.BootCommand));
            if (procId == 0)
            {
                Error("Ashita failed to load the boot file.");
                return false;
            }

            // Create configuration mapping..
            var fileHandle = NativeMethods.CreateFileMapping(IntPtr.Zero, IntPtr.Zero, (int)NativeMethods.MemoryProtection.ReadWrite, 0, Marshal.SizeOf(typeof(AshitaSettings)), String.Format("AshitaMMFSettings_{0}", procId));
            if (fileHandle == IntPtr.Zero)
            {
                Error("Ashita failed to load configuration mapping.");
                return false;
            }

            // Map view of file into local space..
            var fileMapping = NativeMethods.MapViewOfFile(fileHandle, 0x001F, 0, 0, 0);
            if (fileMapping == IntPtr.Zero)
            {
                Error("Ashita failed to map view of configuration file.");
                return false;
            }

            // Write settings to MMF..
            var settings = new AshitaSettings
                {
                    InstallPath = AppDomain.CurrentDomain.BaseDirectory,
                    ConfigPath = config.FilePath,
                    Language = config.Language,
                    IsLoaded = false
                };
            Marshal.StructureToPtr(settings, fileMapping, true);

            // Inject Ashita into remote target..
            if (!ManagedInjector.InjectModule(procId, AppDomain.CurrentDomain.BaseDirectory + "\\Ashita Core.dll", false))
            {
                ManagedInjector.KillProcess(procId);
                Error("Ashita failed to inject into PlayOnline.");
                return false;
            }

            // Resume the game and play!
            ManagedInjector.ResumeProcess(procId);

            // Wait for Ashita to load our configurations..
            var loadedAttempts = 0;
            while (settings.IsLoaded == false)
            {
                if (loadedAttempts == 5)
                {
                    // Cleanup..
                    NativeMethods.UnmapViewOfFile(fileMapping);
                    NativeMethods.CloseHandle(fileHandle);
                    ManagedInjector.KillProcess(procId);
                    Error("Ashita failed to process the selected configuration.");
                    return false;
                }

                settings = (AshitaSettings)Marshal.PtrToStructure(fileMapping, typeof(AshitaSettings));
                Thread.Sleep(1000);
                loadedAttempts++;
            }

            // Cleanup..
            NativeMethods.UnmapViewOfFile(fileMapping);
            NativeMethods.CloseHandle(fileHandle);

            return true;
        }
    }
}
