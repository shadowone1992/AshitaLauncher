
/**
 * DesignDataService.cs - Designer mode data service.
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

namespace Ashita.Design
{
    using Ashita.Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// DesignMode DataService Implementation
    /// 
    /// Data service that allows the designer to show fake information while the
    /// project is being altered. 
    /// </summary>
    public class DesignDataService : IDataService
    {
        /// <summary>
        /// Gets a list of fake files for design mode purposes.
        /// </summary>
        /// <param name="callback"></param>
        public void GetConfigurationFiles(Action<List<Configuration>, Exception> callback)
        {
            var configs = new List<Configuration>()
                {
                    new Configuration()
                        {
                            Language = (int)ResourceLanguage.US,
                            Name = "Small",
                            ResolutionX = 800,
                            ResolutionY = 600,
                            BootFile = "C:\\Users\\atom0s\\Desktop\\pol.exe"
                        },
                    new Configuration()
                        {
                            Language = (int)ResourceLanguage.US,
                            Name = "Local",
                            ResolutionX = 800,
                            ResolutionY = 600,
                            BootFile = "C:\\Users\\atom0s\\Desktop\\pol.exe"
                        },
                    new Configuration()
                        {
                            Language = (int)ResourceLanguage.US,
                            Name = "Private Server",
                            ResolutionX = 640,
                            ResolutionY = 480,
                            BootFile = "C:\\Program Files (x86)\\Steam\\steamApps\\common\\ffxi\\SquareEnix\\FINAL FANTASY XI\\pol.exe"
                        }
                };

            callback(configs, null);
        }

        /// <summary>
        /// Gets a list of fake files for design mode purposes.
        /// </summary>
        /// <param name="callback"></param>
        public void GetUpdateFiles(Action<List<UpdateFile>, Exception> callback)
        {
            var updates = new List<UpdateFile>()
                {
                    new UpdateFile()
                        {
                            FileName = "Ashita Core.dll",
                            FilePath = "Ashita Core.dll",
                            FullPath = "C:\\Derp\\Ashita Core.dll",
                            LocalChecksum = "",
                            RemoteChecksum = "8207351f05789139b2c930a6f1800f20",
                        }
                };

            callback(updates, null);
        }

        /// <summary>
        /// Stub implementation to prevent compile errors. (Does nothing.)
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="selectedFiles"></param>
        public void UpdateSelectedFiles(Action<List<UpdateFile>, Exception> callback, List<UpdateFile> selectedFiles)
        {
            callback(null, null);
        }
    }
}
