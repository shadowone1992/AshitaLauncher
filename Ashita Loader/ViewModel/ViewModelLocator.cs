
/**
 * ViewModelLocator.cs - Base view model locator.
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
    using Ashita.Design;
    using Ashita.Model;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// View Model Locator Implementation
    /// 
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Static Constructor
        /// </summary>
        static ViewModelLocator()
        {
            // Prepare locator service..
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            else
                SimpleIoc.Default.Register<IDataService, DataService>();

            // Register view models to service..
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LauncherViewModel>();
            SimpleIoc.Default.Register<UpdatesViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
        }

        /// <summary>
        /// Gets the MainViewModel object.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        /// Gets the LauncherViewModel object.
        /// </summary>
        public LauncherViewModel Launcher
        {
            get { return ServiceLocator.Current.GetInstance<LauncherViewModel>(); }
        }

        /// <summary>
        /// Gets the UpdatesViewModel object.
        /// </summary>
        public UpdatesViewModel Updates
        {
            get { return ServiceLocator.Current.GetInstance<UpdatesViewModel>(); }
        }

        /// <summary>
        /// Gets the AboutViewModel object.
        /// </summary>
        public AboutViewModel About
        {
            get { return ServiceLocator.Current.GetInstance<AboutViewModel>(); }
        }
    }
}
