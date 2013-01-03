
/**
 * DataServie.cs - DataService implementation used for collection data.
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

namespace Ashita.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Xml.Linq;

    /// <summary>
    /// Main data service implementation used to handle various data such as:
    ///     - Configuration Files
    ///     - Update Files
    /// 
    /// </summary>
    public class DataService : IDataService
    {
        /// <summary>
        /// Obtains the current configuration files on disk.
        /// </summary>
        /// <param name="callback"></param>
        public void GetConfigurationFiles(Action<List<Configuration>, Exception> callback)
        {
            try
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                    {
                        var configs = new List<Configuration>();

                        try
                        {
                            // Attempt to load all configuration files..
                            configs = (from f in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Boot\\")
                                       let c = new Configuration()
                                       where c.LoadFromFile(f)
                                       select c).ToList();
                        }
                        catch
                        {
                        }

                        // Return the resulting list..
                        e.Result = configs;
                    };

                // Assign and invoke worker..
                worker.RunWorkerCompleted += (sender, e) => callback((List<Configuration>)e.Result, null);
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                callback(null, ex);
            }
        }

        /// <summary>
        /// Obtains the remote update file list and determines if updates are needed.
        /// </summary>
        /// <param name="callback"></param>
        public void GetUpdateFiles(Action<List<UpdateFile>, Exception> callback)
        {
            try
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                    {
                        var tempFiles = new List<UpdateFile>();
                        try
                        {
                            // Parse and validate the update list..
                            var updateList = XDocument.Load(Ashita.Properties.Resources.RemoteUpdateList);
                            if (updateList.Root == null || !updateList.Root.Elements("file").Any())
                            {
                                e.Result = tempFiles;
                                return;
                            }

                            // Process each found file in the update list..
                            foreach (var f in updateList.Root.Elements("file"))
                            {
                                // Read the remote file data..
                                var file = new UpdateFile
                                    {
                                        FileName = f.Attribute("file_name").Value,
                                        FilePath = f.Attribute("file_path").Value,
                                        FullPath = AppDomain.CurrentDomain.BaseDirectory + "//" + f.Attribute("file_path").Value,
                                        RemoteChecksum = f.Attribute("checksum").Value,
                                        LocalChecksum = String.Empty
                                    };

                                // Obtain local checksum if file exists..
                                if (File.Exists(file.FullPath))
                                {
                                    using (var stream = new BufferedStream(File.OpenRead(file.FullPath), 12000000))
                                    {
                                        var md5 = MD5.Create().ComputeHash(stream);
                                        file.LocalChecksum = String.Join("", md5.Select(x => x.ToString("x2")));
                                    }
                                }

                                if (String.Equals(file.LocalChecksum, file.RemoteChecksum))
                                    continue;

                                tempFiles.Add(file);
                            }
                        }
                        catch
                        {
                        }

                        e.Result = tempFiles.ToList();
                    };

                // Assign and invoke the worker..
                worker.RunWorkerCompleted += (sender, e) => callback((List<UpdateFile>)e.Result, null);
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                callback(null, ex);
            }
        }

        /// <summary>
        /// Updates the given list of files.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="selectedFiles"></param>
        public void UpdateSelectedFiles(Action<List<UpdateFile>, Exception> callback, List<UpdateFile> selectedFiles)
        {
            try
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                    {
                        var failedFiles = new List<UpdateFile>();
                        foreach (var f in selectedFiles)
                        {
                            try
                            {
                                // Attempt to delete the current file..
                                if (File.Exists(f.FullPath))
                                    File.Delete(f.FullPath);

                                // Create the directories leading to the file..
                                var filePath = Path.GetDirectoryName(f.FullPath);
                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                // Download the remote file..
                                var client = new WebClient();
                                client.DownloadFile(Ashita.Properties.Resources.RemoteDownloadUrl + f.FilePath, f.FullPath);
                            }
                            catch
                            {
                                failedFiles.Add(f);
                            }
                        }

                        e.Result = failedFiles;
                    };

                // Assign and invoke the worker..
                worker.RunWorkerCompleted += (sender, e) => callback((List<UpdateFile>)e.Result, null);
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                callback(null, ex);
            }
        }
    }
}
