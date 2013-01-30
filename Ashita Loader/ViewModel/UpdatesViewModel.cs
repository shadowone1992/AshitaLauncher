
/**
 * UpdatesViewModel.cs - Update view model implementation.
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
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Ashita.Model;
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Updates View Model
    /// 
    /// View model backend for the 'Updates' view.
    /// </summary>
    public class UpdatesViewModel : ViewModelBase
    {
        /// <summary>
        /// Internal DataService object.
        /// </summary>
        private readonly IDataService _dataService;

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="dataService"></param>
        public UpdatesViewModel(IDataService dataService)
        {
            // Store the data service object..
            this._dataService = dataService;

            // Connect command handlers..
            this.SelectAllCommand = new RelayCommand(OnSelecctAllChecked);
            this.RefreshUpdatesCommand = new RelayCommand(OnRefreshUpdatesClicked);
            this.UpdateSelectedCommand = new RelayCommand(OnUpdateSelectedClicked);
            this.UpdateAllFilesCommand = new RelayCommand(OnUpdateAllFilesClicked);

            // Refresh updates list..
            OnRefreshUpdatesClicked();
        }

        /// <summary>
        /// Command handler when select all is invoked.
        /// </summary>
        private void OnSelecctAllChecked()
        {
            foreach (var f in this.UpdateFiles)
            {
                f.IsSelected = this.SelectAllChecked;
            }
        }

        /// <summary>
        /// Command handler when refresh updates is invoked.
        /// </summary>
        private void OnRefreshUpdatesClicked()
        {
            // Reset selection checkbox..
            this.SelectAllChecked = false;

            // Prepare for the new update list..
            this.UpdateFiles = null;
            this.NoUpdatesFoundVisibility = Visibility.Hidden;
            this.UpdatingFilesVisibility = Visibility.Hidden;
            this.UpdatesProcessingFiles = true;

            // Populate the update collection..
            this._dataService.GetUpdateFiles(
                (updates, error) =>
                    {
                        this.UpdateFiles = (error == null) ?
                                               ProcessIgnoredFiles(new ObservableCollection<UpdateFile>(updates)) :
                                               new ObservableCollection<UpdateFile>();
                        this.UpdatesProcessingFiles = false;
                    });
        }

        /// <summary>
        /// Command handler when update selected is invoked.
        /// </summary>
        private void OnUpdateSelectedClicked()
        {
            if (this.UpdateFiles == null)
                return;

            var selectedFiles = this.UpdateFiles.Where(x => x.IsSelected).ToList();
            if (!selectedFiles.Any())
                return;

            this.UpdateFiles = null;
            this.NoUpdatesFoundVisibility = Visibility.Hidden;
            this.UpdatingFilesVisibility = Visibility.Visible;

            this._dataService.UpdateSelectedFiles(
                (failed, error) =>
                    {
                        // Print out what files failed to update..
                        if (failed != null && failed.Any())
                        {
                            var output = "Updater failed to update the following files:" + Environment.NewLine;
                            failed.ForEach(x => output += x.FilePath + Environment.NewLine);
                            MessageBox.Show(output, "Failed to update files..");
                        }

                        // Hide the updater..
                        this.UpdatingFilesVisibility = Visibility.Hidden;

                        // Update the file list..
                        OnRefreshUpdatesClicked();
                    }, selectedFiles);
        }

        /// <summary>
        /// Command handler when update all is invoked.
        /// </summary>
        private void OnUpdateAllFilesClicked()
        {
            if (this.UpdateFiles == null)
                return;

            var selectedFiles = this.UpdateFiles.ToList();
            if (!selectedFiles.Any())
                return;

            this.UpdateFiles = null;
            this.NoUpdatesFoundVisibility = Visibility.Hidden;
            this.UpdatingFilesVisibility = Visibility.Visible;

            this._dataService.UpdateSelectedFiles(
                (failed, error) =>
                    {
                        // Print out what files failed to update..
                        if (failed != null && failed.Any())
                        {
                            var output = "Updater failed to update the following files:" + Environment.NewLine;
                            failed.ForEach(x => output += x.FilePath + Environment.NewLine);
                            MessageBox.Show(output, "Failed to update files..");
                        }

                        // Hide the updater..
                        this.UpdatingFilesVisibility = Visibility.Hidden;

                        // Update the file list..
                        OnRefreshUpdatesClicked();
                    }, selectedFiles);
        }

        /// <summary>
        /// Parses the incoming collection to remove ignored files.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<UpdateFile> ProcessIgnoredFiles(ObservableCollection<UpdateFile> files)
        {
            try
            {
                // Obtain the file list to ignore..
                var ignore = System.Configuration.ConfigurationManager.AppSettings["ignoreFiles"];
                if (String.IsNullOrEmpty(ignore))
                    return files;

                // Split ignoreFiles into patterns..
                var ignorePatterns = ignore.Split(';');

                // Build list of files to ignore..
                var ignoreFiles = new List<UpdateFile>();
                files.ToList().ForEach(f => ignoreFiles.AddRange(from pattern in ignorePatterns
                                                                 where !String.IsNullOrEmpty(pattern)
                                                                 where Regex.Match(f.FilePath, pattern).Captures.Count > 0
                                                                 select f));

                // Return filtered list of files..
                var filteredFiles = files.ToList();
                filteredFiles.RemoveAll(ignoreFiles.Contains);
                return new ObservableCollection<UpdateFile>(filteredFiles);
            }
            catch
            {
                // On error; return full original list..
                return files;
            }
        }

        /// <summary>
        /// Gets or sets the update file collection.
        /// </summary>
        public ObservableCollection<UpdateFile> UpdateFiles
        {
            get { return this.Get<ObservableCollection<UpdateFile>>("UpdateFiles"); }
            set
            {
                this.NoUpdatesFoundVisibility = (value == null || !value.Any()) ? Visibility.Visible : Visibility.Hidden;
                this.Set("UpdateFiles", value);
            }
        }

        /// <summary>
        /// Gets or sets the select all checkbox value.
        /// </summary>
        public Boolean SelectAllChecked
        {
            get { return this.Get<Boolean>("SelectAllChecked"); }
            set { this.Set("SelectAllChecked", value); }
        }

        /// <summary>
        /// Gets or sets the update processing indication visibility.
        /// </summary>
        public Boolean UpdatesProcessingFiles
        {
            get { return this.Get<Boolean>("UpdatesProcessingFiles"); }
            set { this.Set("UpdatesProcessingFiles", value); }
        }

        /// <summary>
        /// Gets or sets the no updates found label visibility.
        /// </summary>
        public Visibility NoUpdatesFoundVisibility
        {
            get { return this.Get<Visibility>("NoUpdatesFoundVisibility"); }
            set { this.Set("NoUpdatesFoundVisibility", value); }
        }

        /// <summary>
        /// Gets or sets the no updates found label visibility.
        /// </summary>
        public Visibility UpdatingFilesVisibility
        {
            get { return this.Get<Visibility>("UpdatingFilesVisibility"); }
            set { this.Set("UpdatingFilesVisibility", value); }
        }

        /// <summary>
        /// Command definition used to select or deselect all files.
        /// </summary>
        public ICommand SelectAllCommand { get; set; }

        /// <summary>
        /// Command definition used to refresh the update list.
        /// </summary>
        public ICommand RefreshUpdatesCommand { get; set; }

        /// <summary>
        /// Command definition used to update selected files.
        /// </summary>
        public ICommand UpdateSelectedCommand { get; set; }

        /// <summary>
        /// Command definition used to update all files.
        /// </summary>
        public ICommand UpdateAllFilesCommand { get; set; }
    }
}
