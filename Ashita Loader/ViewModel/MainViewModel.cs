
/**
 * MainViewModel.cs - Main view model implementation.
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

namespace Ashita.ViewModel
{
    using Ashita.Model;
    using GalaSoft.MvvmLight.Command;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// Main View Model
    /// 
    /// View model backend for the 'Main' view.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainViewModel()
        {
            this.OpenBugReportsCommand = new RelayCommand(() => Process.Start("http://bugs.ffevo.net/ashita"));
            this.OpenForumsCommand = new RelayCommand(() => Process.Start("http://www.ffevo.net/index"));
        }

        /// <summary>
        /// Command definition used to open the reports url.
        /// </summary>
        public ICommand OpenBugReportsCommand { get; set; }

        /// <summary>
        /// Command definition used to open the forums url.
        /// </summary>
        public ICommand OpenForumsCommand { get; set; }
    }
}



/**
 * 
 * TODO :: Cleanup this junk..
 * 
using System.Security.Cryptography;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Ashita.ViewModel
{
    using Ashita.Classes;
    using Ashita.Helpers;
    using Ashita.Model;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    public class MainViewModel : ViewModelBase
    {
        private readonly String m_RemoteUpdatePath = "http://svn.ffevo.net/autoupdate/";
        private readonly String m_RemoteSvnPath = "http://svn.ffevo.net/ashita/release/";

        /// <summary>
        /// Internal configuration collection.
        /// </summary>
        private ObservableCollection<Configuration> _configCollection;

        /// <summary>
        /// Internal update file collection.
        /// </summary>
        private ObservableCollection<UpdateFile> _updateCollection;

        /// <summary>
        /// Internal selected configuration item.
        /// </summary>
        private Configuration _selectedConfig;

        /// <summary>
        /// Internal temp configuration item.
        /// </summary>
        private Configuration _tempConfig;

        /// <summary>
        /// Internal configuration delete panel visibility.
        /// </summary>
        private Visibility _deleteConfigVisibility;

        /// <summary>
        /// Intenal configuration new/edit panel visibility.
        /// </summary>
        private Visibility _newEditConfigVisibility;

        /// <summary>
        /// Internal updates not found visibility.
        /// </summary>
        private Visibility _noUpdatesFoundVisibility;

        /// <summary>
        /// Internal flag if updates are being processed.
        /// </summary>
        private Boolean _updatesProcessingFiles;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainViewModel()
        {
            // Set UI defaults..
            this.DeleteConfigVisibility = Visibility.Hidden;
            this.NewEditConfigVisibility = Visibility.Visible;
            this.UpdatesProcessingFiles = false;
            
            // Connect Titlebar Commands
            this.OpenBugReportsCommand = new RelayCommand(() => Process.Start("http://bugs.ffevo.net/ashita"));
            this.OpenForumsCommand = new RelayCommand(() => Process.Start("http://www.ffevo.net/index"));

            // Connect UI commands..
            this.NewConfigCommand = new RelayCommand(OnNewConfigClicked);
            this.EditConfigCommand = new RelayCommand(OnEditConfigClicked);
            this.DeleteConfigCommand = new RelayCommand(OnDeleteConfigClicked);
            this.ConfirmDeleteConfigCommand = new RelayCommand(OnConfirmDeleteConfigClicked);
            this.CancelDeleteConfigCommand = new RelayCommand(OnCancelDeleteConfigClicked);
            this.ConfigEntryClicked = this.LaunchCommand = new RelayCommand(OnLaunchClicked);

            this.SaveConfigCommand = new RelayCommand(OnSaveNewEditConfigClicked);
            this.CancelEditConfigCommand = new RelayCommand(OnCancelNewEditConfigClicked);

            this.RefreshUpdatesCommand = new RelayCommand(OnRefreshUpdatesClicked);
            

            // Load configuration files..
            try
            {
                this._configCollection = new ObservableCollection<Configuration>();

                var files = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Boot\\");
                var configs = (from f in files let c = new Configuration()
                               where c.LoadFromFile(f) select c).ToList();

                configs.ForEach(this._configCollection.Add);
            }
            catch { }

            // Load the update files..
            OnRefreshUpdatesClicked();
        }
        
        /// <summary>
        /// Command handler when a new configuration is being created.
        /// </summary>
        private void OnNewConfigClicked()
        {
            this.SelectedConfig = this.TempConfig = new Configuration();
            this.NewEditConfigVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command handler when a configuration is being edited.
        /// </summary>
        private void OnEditConfigClicked()
        {
            if (this.SelectedConfig == null)
                return;
            this.NewEditConfigVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command handler when a configuration is being launched.
        /// </summary>
        private void OnLaunchClicked()
        {
            try
            {
                // Validate our selected configuration..
                var config = this.SelectedConfig;
                if (config == null || !File.Exists(config.FilePath))
                {
                    MessageBox.Show("Invalid configuration file; could not launch.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Obtain path to PlayOnline..
                var polPath = RegisteryHelper.GetValue<String>(String.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\PlayOnline{0}\\InstallFolder", (config.PolVersion == "JP") ? "" : config.PolVersion), "1000");
                if (string.IsNullOrEmpty(polPath))
                {
                    MessageBox.Show("Failed to read PlayOnline path from registry.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
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
                var procId = ManagedInjector.CreateSuspended(bootFile, config.BootCommand);
                if (procId == 0)
                {
                    MessageBox.Show("Ashita failed to load the boot file.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create configuration mapping..
                var fileHandle = NativeAPI.CreateFileMapping(IntPtr.Zero, IntPtr.Zero, (int) NativeAPI.MemoryProtection.ReadWrite, 0, Marshal.SizeOf(typeof(AshitaSettings)), String.Format("AshitaMMFSettings_{0}", procId));
                if (fileHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Ashita failed to load configuration mapping.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Map view of file into local space..
                var fileMapping = NativeAPI.MapViewOfFile(fileHandle, 0x001F, 0, 0, 0);
                if (fileMapping == IntPtr.Zero)
                {
                    MessageBox.Show("Ashita failed to map view of configuration file.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Write settings to MMF..
                var settings = new AshitaSettings
                {
                    InstallPath = AppDomain.CurrentDomain.BaseDirectory,
                    ConfigPath = config.FilePath,
                    Language = (int) config.Language,
                    IsLoaded = false
                };
                Marshal.StructureToPtr(settings, fileMapping, true);

                // Inject Ashita into remote target..
                if (!ManagedInjector.InjectModule(procId, AppDomain.CurrentDomain.BaseDirectory + "\\Ashita Core.dll", false))
                {
                    ManagedInjector.KillProcess(procId);
                    MessageBox.Show("Ashita failed to inject into PlayOnline.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
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
                        NativeAPI.UnmapViewOfFile(fileMapping);
                        NativeAPI.CloseHandle(fileHandle);
                        ManagedInjector.KillProcess(procId);
                        MessageBox.Show("Ashita failed to process the selected configuration.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    settings = (AshitaSettings)Marshal.PtrToStructure(fileMapping, typeof(AshitaSettings));
                    Thread.Sleep(1000);
                    loadedAttempts++;
                }

                // Cleanup..
                NativeAPI.UnmapViewOfFile(fileMapping);
                NativeAPI.CloseHandle(fileHandle);

                if (config.AutoClose)
                {
                    Application.Current.Shutdown();
                }
            }
            catch { }
        }

        /// <summary>
        /// Command handler when a configuration is being deleted.
        /// </summary>
        private void OnDeleteConfigClicked()
        {
            if (this.SelectedConfig != null)
                this.DeleteConfigVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command handler when a config is being confirmed for deletion.
        /// </summary>
        private void OnConfirmDeleteConfigClicked()
        {
            this.DeleteConfigVisibility = Visibility.Hidden;
            try
            {
                if (this.SelectedConfig != null)
                {
                    File.Delete(this.SelectedConfig.FilePath);
                    this.Configurations.Remove(this.SelectedConfig);
                }
            }
            catch { }
        }

        /// <summary>
        /// Command handler when a config is being canceled from deletion.
        /// </summary>
        private void OnCancelDeleteConfigClicked()
        {
            this.DeleteConfigVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Command handler when a new/edit config is being saved.
        /// </summary>
        private void OnSaveNewEditConfigClicked()
        {
            
        }

        /// <summary>
        /// Command handler when a new/edit config is being canceled.
        /// </summary>
        private void OnCancelNewEditConfigClicked()
        {
            this.SelectedConfig = null;
            this.TempConfig = null;
            this.NewEditConfigVisibility = Visibility.Hidden;
        }






        private void DoUpdateFiles()
        {
            this.UpdatesProcessingFiles = true;
            var tempFiles = new ObservableCollection<UpdateFile>();

            try
            {
                // Attempt to read the remote data..
                var updateList = XDocument.Load(this.m_RemoteUpdatePath);
                foreach (var f in updateList.Root.Elements("file"))
                {
                    // Read remote file data..
                    var file = new UpdateFile
                                   {
                                       FileName = f.Attribute("file_name").Value,
                                       FilePath = f.Attribute("file_path").Value,
                                       FullPath = AppDomain.CurrentDomain.BaseDirectory + "//" + f.Attribute("file_path").Value,
                                       RemoteChecksum = f.Attribute("checksum").Value,
                                       LocalChecksum = String.Empty
                                   };

                    // Obtain local checksum if we exist..
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
            catch { }
            this.UpdateFiles = tempFiles;
            this.UpdatesProcessingFiles = false;
        }


        /// <summary>
        /// Command handler when refresh updates is invoked.
        /// </summary>
        private void OnRefreshUpdatesClicked()
        {
            ThreadStart threadStart = () => Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background, new Action(DoUpdateFiles));
            new Thread(threadStart).Start();
        }




















        /// <summary>
        /// Gets or sets the configuration collection.
        /// </summary>
        public ObservableCollection<Configuration> Configurations
        {
            get { return this._configCollection; }
            set
            {
                this._configCollection = value;
                this.RaisePropertyChanged("Configurations");
            }
        }

        /// <summary>
        /// Gets or sets the update files collection.
        /// </summary>
        public ObservableCollection<UpdateFile> UpdateFiles
        {
            get { return this._updateCollection; }
            set
            {
                this._updateCollection = value;
                this.RaisePropertyChanged("UpdateFiles");
            }
        }

        /// <summary>
        /// Gets or sets the selected configuration item.
        /// </summary>
        public Configuration SelectedConfig
        {
            get { return this._selectedConfig; }
            set
            {
                this._selectedConfig = value;
                this.RaisePropertyChanged("SelectedConfig");
            }
        }

        /// <summary>
        /// Gets or sets the temp configuration item.
        /// </summary>
        public Configuration TempConfig
        {
            get { return this._tempConfig; }
            set
            {
                this._tempConfig = value;
                this.RaisePropertyChanged("TempConfig");
            }
        }

        /// <summary>
        /// Gets or sets the delete config panel visibility.
        /// </summary>
        public Visibility DeleteConfigVisibility
        {
            get { return this._deleteConfigVisibility; }
            set
            {
                this._deleteConfigVisibility = value;
                this.RaisePropertyChanged("DeleteConfigVisibility");
            }
        }

        /// <summary>
        /// Gets or sets the new/edit config panel visibility.
        /// </summary>
        public Visibility NewEditConfigVisibility
        {
            get { return this._newEditConfigVisibility; }
            set
            {
                this._newEditConfigVisibility = value;
                this.RaisePropertyChanged("NewEditConfigVisibility");
            }
        }

        /// <summary>
        /// Gets or sets the no updates found label visibility.
        /// </summary>
        public Visibility NoUpdatesFoundVisibility
        {
            get
            {
                this._noUpdatesFoundVisibility = (this._updateCollection == null || !this._updateCollection.Any()) ? Visibility.Visible : Visibility.Hidden;
                return this._noUpdatesFoundVisibility;
            }
            set
            {
                this._noUpdatesFoundVisibility = value;
                this.RaisePropertyChanged("NoUpdatesFoundVisibility");
            }
        }

        /// <summary>
        /// Gets or set the update processing indication visibility.
        /// </summary>
        public Boolean UpdatesProcessingFiles
        {
            get { return this._updatesProcessingFiles; }
            set
            {
                this._updatesProcessingFiles = value;
                this.RaisePropertyChanged("UpdatesProcessingFiles");
            }
        }

        /// <summary>
        /// Gets the application version string.
        /// </summary>
        public String LoaderVersionString
        {
            get
            {
                var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format("Version: {0}.{1}.{2}.{3}", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
            }
        }

        /// <summary>
        /// TODO: These need to be notifiable since they are bound to the UI.
        /// </summary>
        public ICommand OpenBugReportsCommand { get;set; }
        public ICommand OpenForumsCommand { get;set; }

        public ICommand NewConfigCommand { get; set; }
        public ICommand EditConfigCommand { get;set; }
        public ICommand DeleteConfigCommand { get;set; }
        public ICommand LaunchCommand { get;set; }
        public ICommand ConfigEntryClicked { get;set; }
        public ICommand ConfirmDeleteConfigCommand { get;set; }
        public ICommand CancelDeleteConfigCommand { get;set; }

        public ICommand SaveConfigCommand { get;set; }
        public ICommand CancelEditConfigCommand { get;set; }

        public ICommand RefreshUpdatesCommand { get;set; }
    }
}*/