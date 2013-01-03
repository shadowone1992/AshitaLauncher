
/**
 * IDataServie.cs - DataService interface implementation outline.
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

    /// <summary>
    /// DataService Interface Implementation.
    /// 
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Data service method to obtain configuration files.
        /// </summary>
        /// <param name="callback"></param>
        void GetConfigurationFiles(Action<List<Configuration>, Exception> callback);

        /// <summary>
        /// Data service method to obtain update files.
        /// </summary>
        /// <param name="callback"></param>
        void GetUpdateFiles(Action<List<UpdateFile>, Exception> callback);

        /// <summary>
        /// Data service method to update selected files.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="selectedFiles"></param>
        void UpdateSelectedFiles(Action<List<UpdateFile>, Exception> callback, List<UpdateFile> selectedFiles);
    }
}
