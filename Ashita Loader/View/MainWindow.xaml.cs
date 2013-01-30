
/**
 * MainWindow.xaml.cs - MainWindow initialization.
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

namespace Ashita.View
{
    using MahApps.Metro;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var theme = System.Configuration.ConfigurationManager.AppSettings["theme"] ?? "light";
            var accentConfig = System.Configuration.ConfigurationManager.AppSettings["accent"] ?? "blue";
            var accent = ThemeManager.DefaultAccents.FirstOrDefault(a => a.Name.ToLower() == accentConfig) ?? ThemeManager.DefaultAccents.First(a => a.Name == "Blue");
            MahApps.Metro.ThemeManager.ChangeTheme(this, accent, (theme.ToLower() == "dark") ? Theme.Dark : Theme.Light);

            // Determine if we should warn about updating..
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Ashita Core.dll"))
            {
                MessageBox.Show(
                    "It appears this is your first time running this launcher.\r\n" +
                    "There are updates available. Please click the Updates tab at the top of the launcher.",
                    "Updates Available!",
                    MessageBoxButton.OK, MessageBoxImage.Information
                    );
            }
        }
    }
}
