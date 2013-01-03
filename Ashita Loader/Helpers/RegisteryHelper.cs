
/**
 * RegistryHelper.cs - Slim registry wrappers.
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

namespace Ashita.Helpers
{
    using Microsoft.Win32;
    using System;

    /// <summary>
    /// Templated registery wrappers used to easily obtain and write
    /// data in the system registry.
    /// 
    /// </summary>
    public static class RegisteryHelper
    {
        /// <summary>
        /// Reads a registry value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKeyName"></param>
        /// <param name="strValueName"></param>
        /// <returns></returns>
        public static T GetValue<T>(String strKeyName, String strValueName)
        {
            try
            {
                return (T)Registry.GetValue(strKeyName, strValueName, default(T));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Writes a registry value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKeyName"></param>
        /// <param name="strValueName"></param>
        /// <param name="value"></param>
        public static void SetValue<T>(String strKeyName, String strValueName, T value)
        {
            try
            {
                Registry.SetValue(strKeyName, strValueName, value);
            }
            catch
            {
            }
        }
    }
}
