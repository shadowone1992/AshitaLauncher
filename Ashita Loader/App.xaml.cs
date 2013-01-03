
/**
 * App.xaml.cs - Main application implementaton.
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

namespace Ashita
{
    using Ashita.Classes;
    using Ashita.Model;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        /// Startup override to handle command line arguments.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            if (e.Args.Any())
            {
                var configs = (from s in e.Args
                              let c = new Configuration()
                              where c.LoadFromFile(s)
                              select c).ToList();
                configs.ForEach(c => AshitaInject.DoInjection(c));
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// Assembly resolver to allow embedded objects to be loaded without extracting to disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Obtain the resolving assemblyy name..
            var dllName = args.Name.Contains(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");

            // Ignore resources requests..
            if (dllName.EndsWith(".resources"))
                return null;

            // Load this resource..
            var fullName = this.GetType().Namespace + ".Embedded." + new AssemblyName(args.Name).Name + ".dll";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName);
            if (stream != null)
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return Assembly.Load(data);
            }

            return null;
        }
    }
}
