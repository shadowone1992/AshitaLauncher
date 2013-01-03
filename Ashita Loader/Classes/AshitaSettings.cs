
/**
 * AshitaSettings.cs - Ashita MMF Settings Structure
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

namespace Ashita.Classes
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// AshitaSettings Memory Mapped File Structure
    /// 
    /// Data structure that is passed to Ashita to locate and load the
    /// selected configuration file and process plugins / scripts.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AshitaSettings
    {
        /// <summary>
        /// Ashita's base install path.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public String InstallPath;

        /// <summary>
        /// Configuration file being loaded.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public String ConfigPath;

        /// <summary>
        /// Language id being used with this launch of Ashita.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)] public Int32 Language;

        /// <summary>
        /// Flag to determine that Ashita has read this data.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public Boolean IsLoaded;
    }
}
