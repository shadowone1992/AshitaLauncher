
/**
 * ViewModelBase.cs - View model base implementation.
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
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// View Model Base Implementation
    /// 
    /// Base view model object that is inheritable by all view models.
    /// Implements the NotifiableModel class to allow for clean properties.
    /// </summary>
    public abstract class ViewModelBase : NotifiableModel
    {
        /// <summary>
        /// Internal static design mode flag.
        /// </summary>
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets if this ViewModelBase is in design mode.
        /// </summary>
        public bool IsInDesignMode
        {
            get { return ViewModelBase.IsInDesignModeStatic; }
        }

        /// <summary>
        /// Gets the static ViewModelBase design mode flag.
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!ViewModelBase._isInDesignMode.HasValue)
                {
                    var isInDesignModeProperty = DesignerProperties.IsInDesignModeProperty;
                    ViewModelBase._isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(isInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;
                }
                return ViewModelBase._isInDesignMode.Value;
            }
        }
    }
}
