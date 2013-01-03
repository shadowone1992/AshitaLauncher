
/**
 * LauncherViewModel.cs - Launcher view model implementation.
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
    using Ashita.Classes;
    using Ashita.Model;
    using GalaSoft.MvvmLight.Command;
    using Microsoft.Win32;
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Launcher View Model
    /// 
    /// View model backend for the 'Launcher' view.
    /// </summary>
    public class LauncherViewModel : ViewModelBase
    {
        /// <summary>
        /// Internal DataService object.
        /// </summary>
        private readonly IDataService _dataService;

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="dataService"></param>
        public LauncherViewModel(IDataService dataService)
        {
            // Store the data service object..
            this._dataService = dataService;

            // Set default properties..
            this.DeleteConfigVisibility = Visibility.Hidden;
            this.ConfigEditVisibility = Visibility.Hidden;
            this.TempConfig = null;

            // Connect command handlers..
            this.NewConfigCommand = new RelayCommand(NewConfigClicked);
            this.EditConfigCommand = new RelayCommand(EditConfigClicked);
            this.SaveEditConfigCommand = new RelayCommand(SaveEditConfigClicked);
            this.CancelEditConfigCommand = new RelayCommand(CancelEditConfigClicked);
            this.BrowseLaunchFileCommand = new RelayCommand(BrowseLaunchFileClicked);
            this.DeleteConfigCommand = new RelayCommand(DeleteConfigClicked);
            this.ConfirmDeleteConfigCommand = new RelayCommand(ConfirmDeleteConfigClicked);
            this.CancelDeleteConfigCommand = new RelayCommand(CancelDeleteConfigClicked);
            this.LaunchCommand = new RelayCommand(LaunchClicked);

            // Populate the configuration collection..
            this._dataService.GetConfigurationFiles(
                (configs, error) =>
                    {
                        this.Configurations = (error == null) ?
                                                  new ObservableCollection<Configuration>(configs) :
                                                  new ObservableCollection<Configuration>();
                    });
        }

        /// <summary>
        /// Command handler when a new config is being created.
        /// </summary>
        private void NewConfigClicked()
        {
            this.SelectedConfig = null;
            this.TempConfig = new Configuration();
            this.ConfigEditVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command handler when an existing config is being edited.
        /// </summary>
        private void EditConfigClicked()
        {
            if (this.SelectedConfig == null)
                return;

            this.TempConfig = (Configuration)this.SelectedConfig.Clone();
            this.ConfigEditVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command  handler when a config is being saved.
        /// </summary>
        private void SaveEditConfigClicked()
        {
            if ((this.SelectedConfig != null && this.Configurations != null))
            {
                // Edited a current configuration..
                this.TempConfig.Save();
            }
            else
            {
                // Saving a new configuration..
                var sfd = new SaveFileDialog
                    {
                        AddExtension = true,
                        CheckPathExists = true,
                        Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                        FilterIndex = 0,
                        InitialDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Boot"),
                        OverwritePrompt = true,
                        RestoreDirectory = true,
                        Title = "Save configuration file..."
                    };

                if ((bool)sfd.ShowDialog(Application.Current.MainWindow))
                {
                    this.TempConfig.FilePath = sfd.FileName;
                    this.TempConfig.Save();
                }
            }

            // Cleanup..
            this.SelectedConfig = this.TempConfig = null;
            this.ConfigEditVisibility = Visibility.Hidden;

            // Reload configuration list..
            this._dataService.GetConfigurationFiles(
                (configs, error) =>
                    {
                        this.Configurations = (error == null) ?
                                                  new ObservableCollection<Configuration>(configs) :
                                                  new ObservableCollection<Configuration>();
                    });
        }

        /// <summary>
        /// Command handler when an edited config is being canceled.
        /// </summary>
        private void CancelEditConfigClicked()
        {
            this.TempConfig = null;
            this.ConfigEditVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Command handler when browsing for a boot file.
        /// </summary>
        private void BrowseLaunchFileClicked()
        {
            var ofd = new OpenFileDialog
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = "*.exe",
                    Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                    FilterIndex = 0,
                    InitialDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory),
                    Multiselect = false,
                    RestoreDirectory = true
                };

            if ((bool)ofd.ShowDialog())
                this.TempConfig.BootFile = ofd.FileName;
        }

        /// <summary>
        /// Command handler when a configuration is being deleted.
        /// </summary>
        private void DeleteConfigClicked()
        {
            if (this.SelectedConfig != null)
                this.DeleteConfigVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command handler when a config is being confirmed for deletion.
        /// </summary>
        private void ConfirmDeleteConfigClicked()
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
            catch
            {
            }
        }

        /// <summary>
        /// Command handler when a config is being canceled from deletion.
        /// </summary>
        private void CancelDeleteConfigClicked()
        {
            this.DeleteConfigVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Command handler when a config is being launched.
        /// </summary>
        private void LaunchClicked()
        {
            if (this.SelectedConfig == null)
                return;

            if (AshitaInject.DoInjection(this.SelectedConfig))
            {
                if (this.SelectedConfig.AutoClose)
                    Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Gets or sets the configuration collection.
        /// </summary>
        public ObservableCollection<Configuration> Configurations
        {
            get { return this.Get<ObservableCollection<Configuration>>("Configurations"); }
            set { this.Set("Configurations", value); }
        }

        /// <summary>
        /// Gets or sets the selected configuration.
        /// </summary>
        public Configuration SelectedConfig
        {
            get { return this.Get<Configuration>("SelectedConfig"); }
            set { this.Set("SelectedConfig", value); }
        }

        /// <summary>
        /// Gets or sets the temp configuration.
        /// </summary>
        public Configuration TempConfig
        {
            get { return this.Get<Configuration>("TempConfig"); }
            set { this.Set("TempConfig", value); }
        }

        /// <summary>
        /// Gets or sets the delete config panel visibility.
        /// </summary>
        public Visibility DeleteConfigVisibility
        {
            get { return this.Get<Visibility>("DeleteConfigVisibility"); }
            set { this.Set("DeleteConfigVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the edit config panel visibility.
        /// </summary>
        public Visibility ConfigEditVisibility
        {
            get { return this.Get<Visibility>("ConfigEditVisibility"); }
            set { this.Set("ConfigEditVisibility", value); }
        }

        /// <summary>
        /// Command definition used to create a new configuraton.
        /// </summary>
        public ICommand NewConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to edit an existing configuration.
        /// </summary>
        public ICommand EditConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to save a new/editing configuration.
        /// </summary>
        public ICommand SaveEditConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to cancel a new/editing configuration.
        /// </summary>
        public ICommand CancelEditConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to browse for a boot file.
        /// </summary>
        public ICommand BrowseLaunchFileCommand { get; set; }

        /// <summary>
        /// Command definition used to delete an existing configuration.
        /// </summary>
        public ICommand DeleteConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to confirm a delete configuration command.
        /// </summary>
        public ICommand ConfirmDeleteConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to cancel a delete configuration command.
        /// </summary>
        public ICommand CancelDeleteConfigCommand { get; set; }

        /// <summary>
        /// Command definition used to launch a configuration.
        /// </summary>
        public ICommand LaunchCommand { get; set; }
    }
}
