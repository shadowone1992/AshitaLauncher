
/**
 * NullToDefaultString.cs - String converter class.
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

namespace Ashita.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Display converter to show the word (Default) when a configuration file is
    /// set to load the default PlayOnline file. This occurs when the boot file
    /// path is empty in the configuration file.
    /// </summary>
    public class NullToDefaultString : IValueConverter
    {
        /// <summary>
        /// Converts the given value to a proper string. If no value is present
        /// a default string is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Object Convert(Object value, Type targetType, Object param, CultureInfo culture)
        {
            return (value != null && ((String) value).Length > 0) ? value : "(Default)";
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Object ConvertBack(Object value, Type targetType, Object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
