
/**
 * AboutViewModel.cs - About view model implementation.
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
    using System;
    using System.Reflection;

    /// <summary>
    /// About View Model
    /// 
    /// View model backend for the 'About' view.
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the application version string.
        /// </summary>
        public String LoaderVersion
        {
            get
            {
                var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format("Version: {0}.{1}.{2}.{3}", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
            }
        }
    }
}