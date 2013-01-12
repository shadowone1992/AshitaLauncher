
/**
 * ResourceLanguageToString.cs - Converter for resource language ids.
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
    /// Converter that takes a resource language ID and converts it to a proper
    /// string value when displaying configuration files.
    /// </summary>
    public class ResourceLanguageToString : IValueConverter
    {
        /// <summary>
        /// Convers the given language id to the proper string value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Object Convert(Object value, Type targetType, Object param, CultureInfo culture)
        {
            var langStrings = new[] { "Invalid", "JP", "US", "FR", "DE" };
            return langStrings[(int)value];
        }

        /// <summary>
        /// Converts the given language string to its proper language id.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Object ConvertBack(Object value, Type targetType, Object param, CultureInfo culture)
        {
            switch (((string)value).ToLower())
            {
                case "JP":
                    return 1;
                case "US":
                    return 2;
                case "FR":
                    return 3;
                case "DE":
                    return 4;
                default:
                    return 2;
            }
        }
    }
}
