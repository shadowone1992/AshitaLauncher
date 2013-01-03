
/**
 * PlayOnlineLanguageToIndex.cs - Converter for PlayOnline language ids.
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
    /// Converter that takes a PlayOnline language ID and converts it to a proper
    /// list box index when editing and creating configuration files.
    /// </summary>
    public class PlayOnlineLanguageToIndex : IValueConverter
    {
        /// <summary>
        /// Converts the given string to a proper index value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Object Convert(Object value, Type targetType, Object param, CultureInfo culture)
        {
            if (value == null)
                return null;

            switch (((String)value).ToUpper())
            {
                case "JP":
                    return 0;
                case "US":
                    return 1;
                case "EU":
                    return 2;
            }

            return value;
        }

        /// <summary>
        /// Converts the index value back to its proper string value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="param"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public Object ConvertBack(Object value, Type targetType, Object param, CultureInfo culture)
        {
            if (value == null)
                return null;

            switch ((Int32)value)
            {
                case 0:
                    return "JP";
                case 1:
                    return "US";
                case 2:
                    return "EU";
            }

            return value;
        }
    }
}