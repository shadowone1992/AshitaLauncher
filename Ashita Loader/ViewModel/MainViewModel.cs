
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
