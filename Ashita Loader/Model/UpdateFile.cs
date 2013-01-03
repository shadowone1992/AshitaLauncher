
/**
 * UpdateFile.cs - Update file model.
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

    /// <summary>
    /// Update File Model Implementation
    /// 
    /// Holds information about a file that is out dated.
    /// </summary>
    public class UpdateFile : NotifiableModel
    {
        /// <summary>
        /// Gets or sets the files selected state.
        /// </summary>
        public Boolean IsSelected
        {
            get { return this.Get<Boolean>("IsSelected"); }
            set { this.Set("IsSelected", value); }
        }

        /// <summary>
        /// Gets or sets the file path of this file.
        /// </summary>
        public String FileName
        {
            get { return this.Get<String>("FileName"); }
            set { this.Set("FileName", value); }
        }

        /// <summary>
        /// Gets or sets the file path of this file.
        /// </summary>
        public String FilePath
        {
            get { return this.Get<String>("FilePath"); }
            set { this.Set("FilePath", value); }
        }

        /// <summary>
        /// Gets or sets the full path of this file.
        /// </summary>
        public String FullPath
        {
            get { return this.Get<String>("FullPath"); }
            set { this.Set("FullPath", value); }
        }

        /// <summary>
        /// Gets or sets the local checksum of this file.
        /// </summary>
        public String LocalChecksum
        {
            get { return this.Get<String>("LocalChecksum"); }
            set { this.Set("LocalChecksum", value); }
        }

        /// <summary>
        /// Gets or sets the remote checksum of this file.
        /// </summary>
        public String RemoteChecksum
        {
            get { return this.Get<String>("RemoteChecksum"); }
            set { this.Set("RemoteChecksum", value); }
        }
    }
}
