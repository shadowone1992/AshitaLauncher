
/**
 * Configuration.cs - Configuration file model.
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
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Resource language enumeration.
    /// </summary>
    public enum ResourceLanguage
    {
        JP = 1,
        US = 2,
        FR = 3,
        DE = 4,
    }

    public class Configuration : NotifiableModel, ICloneable
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Configuration()
        {
            this.Language = (int)ResourceLanguage.US;
            this.LogLevel = 4;
            this.ResolutionX = 800;
            this.ResolutionY = 600;
            this.BackgroundX = this.BackgroundY = this.MenuX = this.MenuY = -1;
            this.PolVersion = "US";
            this.ShowBorder = true;
            this.UnhookMouse = true;
            this.AutoClose = true;
            this.BootCommand = "/game eAZcFcB";
            this.StartupScript = "default.txt";
            this.D3DPresentParamsBufferCount = 1;
            this.D3DPresentParamsSwapEffect = 1;
        }


        /// <summary>
        /// Loads a configuration file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Boolean LoadFromFile(String fileName)
        {
            // Store the file path..
            this.FilePath = fileName;

            try
            {
                // Attempt to load the configuration file..
                using (var s = new StreamReader(fileName))
                {
                    var document = XDocument.Parse(s.ReadToEnd());
                    var xElement = document.Element("settings");
                    if (xElement != null)
                    {
                        var settings = xElement.Descendants().ToDictionary(setting => setting.Attribute("name").Value.ToLower(), setting => setting.Value);

                        this.Name = (settings.ContainsKey("config_name")) ? settings["config_name"] : Path.GetFileName(fileName);
                        this.BootFile = settings["boot_file"];
                        this.PolVersion = settings["pol_version"];
                        this.Language = Int32.Parse(settings["language"]);
                        this.ResolutionX = Int32.Parse(settings["resolution_x"]);
                        this.ResolutionY = Int32.Parse(settings["resolution_y"]);
                        this.BackgroundX = (settings.ContainsKey("background_resolution_x")) ? Int32.Parse(settings["background_resolution_x"]) : -1;
                        this.BackgroundY = (settings.ContainsKey("background_resolution_y")) ? Int32.Parse(settings["background_resolution_y"]) : -1;
                        this.MenuX = (settings.ContainsKey("menu_resolution_x")) ? Int32.Parse(settings["menu_resolution_x"]) : -1;
                        this.MenuY = (settings.ContainsKey("menu_resolution_y")) ? Int32.Parse(settings["menu_resolution_y"]) : -1;
                        this.BootCommand = settings["boot_command"];
                        this.AutoClose = settings["auto_close"] == "1";
                        this.StartupScript = settings["startup_script"];
                        this.ShowBorder = settings["show_border"] == "1";
                        this.LogLevel = Int32.Parse(settings["log_level"]);
                        this.UnhookMouse = settings["unhook_mouse"] == "1";
                        this.D3DPresentParamsBufferCount = Int32.Parse(settings["d3d_presentparams_buffercount"]);
                        this.D3DPresentParamsSwapEffect = Int32.Parse(settings["d3d_presentparams_swapeffect"]);
                        this.TestServer = (settings.ContainsKey("test_server")) && (settings["test_server"] == "1");
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to save a configuration file to disk.
        /// </summary>
        /// <returns></returns>
        public Boolean Save()
        {
            try
            {
                // Prepare new config document..
                var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                var root = new XElement("settings");

                // Obtain list of properties..
                var props = (from p in typeof(Configuration).GetProperties()
                             where p.GetCustomAttributes(typeof(XmlElementAttribute), false).Any()
                             select p);

                foreach (var p in props)
                {
                    string newValue;
                    var curValue = p.GetValue(this, null);

                    switch (p.PropertyType.ToString())
                    {
                        case "System.Boolean":
                            newValue = (curValue != null) ? Convert.ToInt32(curValue).ToString() : "0";
                            break;
                        case "Ashita.Model.ResourceLanguage":
                            newValue = (curValue != null) ? Convert.ToInt32(curValue).ToString() : "1";
                            break;
                        default:
                            newValue = (curValue != null) ? curValue.ToString() : String.Empty;
                            break;
                    }

                    var propName = ((XmlElementAttribute)p.GetCustomAttributes(typeof(XmlElementAttribute), false).First()).ElementName;
                    root.Add(new XElement("setting", new XAttribute("name", propName), newValue));
                }

                // Save the new file..
                document.Add(root);
                document.Save(this.FilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ICloneable implementation.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var xml = new XmlSerializer(typeof(Configuration));
            using (var mStream = new MemoryStream())
            {
                xml.Serialize(mStream, this);
                mStream.Seek(0, SeekOrigin.Begin);
                return xml.Deserialize(mStream);
            }
        }

        /// <summary>
        /// Gets or sets the file path of this config.
        /// </summary>
        public String FilePath
        {
            get { return this.Get<String>("FilePath"); }
            set { this.Set("FilePath", value); }
        }

        /// <summary>
        /// Gets or sets the name of this config.
        /// </summary>
        [XmlElement("config_name"), DefaultValue("")]
        public String Name
        {
            get { return this.Get<String>("Name"); }
            set { this.Set("Name", value); }
        }

        /// <summary>
        /// Gets or sets the boot file of this config.
        /// </summary>
        [XmlElement("boot_file")]
        public String BootFile
        {
            get { return this.Get<String>("BootFile"); }
            set { this.Set("BootFile", value); }
        }

        /// <summary>
        /// Gets or sets the PlayOnline language of this config.
        /// </summary>
        [XmlElement("pol_version")]
        public String PolVersion
        {
            get { return this.Get<String>("PolVersion"); }
            set { this.Set("PolVersion", value); }
        }

        /// <summary>
        /// Gets or sets the resource language of this config.
        /// </summary>
        [XmlElement("language")]
        public Int32 Language
        {
            get { return this.Get<Int32>("Language"); }
            set { this.Set("Language", value); }
        }

        /// <summary>
        /// Gets or sets the width property of this config.
        /// </summary>
        [XmlElement("resolution_x")]
        public Int32 ResolutionX
        {
            get { return this.Get<Int32>("ResolutionX"); }
            set { this.Set("ResolutionX", value); }
        }

        /// <summary>
        /// Gets or sets the height property of this config.
        /// </summary>
        [XmlElement("resolution_y")]
        public Int32 ResolutionY
        {
            get { return this.Get<Int32>("ResolutionY"); }
            set { this.Set("ResolutionY", value); }
        }

        /// <summary>
        /// Gets or sets the background width property of this config.
        /// </summary>
        [XmlElement("background_resolution_x")]
        public Int32 BackgroundX
        {
            get { return this.Get<Int32>("BackgroundX"); }
            set { this.Set("BackgroundX", value); }
        }

        /// <summary>
        /// Gets or sets the background height property of this config.
        /// </summary>
        [XmlElement("background_resolution_y")]
        public Int32 BackgroundY
        {
            get { return this.Get<Int32>("BackgroundY"); }
            set { this.Set("BackgroundY", value); }
        }

        /// <summary>
        /// Gets or sets the menu width property of this config.
        /// </summary>
        [XmlElement("menu_resolution_x")]
        public Int32 MenuX
        {
            get { return this.Get<Int32>("MenuX"); }
            set { this.Set("MenuX", value); }
        }

        /// <summary>
        /// Gets or sets the menu height property of this config.
        /// </summary>
        [XmlElement("menu_resolution_y")]
        public Int32 MenuY
        {
            get { return this.Get<Int32>("MenuY"); }
            set { this.Set("MenuY", value); }
        }

        /// <summary>
        /// Gets or sets the boot command of this config.
        /// </summary>
        [XmlElement("boot_command")]
        public String BootCommand
        {
            get { return this.Get<String>("BootCommand"); }
            set { this.Set("BootCommand", value); }
        }

        /// <summary>
        /// Gets or sets the auto close of this config.
        /// </summary>
        [XmlElement("auto_close")]
        public Boolean AutoClose
        {
            get { return this.Get<Boolean>("AutoClose"); }
            set { this.Set("AutoClose", value); }
        }

        /// <summary>
        /// Gets or sets the startup script of this config.
        /// </summary>
        [XmlElement("startup_script")]
        public String StartupScript
        {
            get { return this.Get<String>("StartupScript"); }
            set { this.Set("StartupScript", value); }
        }

        /// <summary>
        /// Gets or sets the show border property of this config.
        /// </summary>
        [XmlElement("show_border")]
        public Boolean ShowBorder
        {
            get { return this.Get<Boolean>("ShowBorder"); }
            set { this.Set("ShowBorder", value); }
        }

        /// <summary>
        /// Gets or sets the log level of this config.
        /// </summary>
        [XmlElement("log_level")]
        public Int32 LogLevel
        {
            get { return this.Get<Int32>("LogLevel"); }
            set { this.Set("LogLevel", value); }
        }

        /// <summary>
        /// Gets or sets the unhook mouse property of this config.
        /// </summary>
        [XmlElement("unhook_mouse")]
        public Boolean UnhookMouse
        {
            get { return this.Get<Boolean>("UnhookMouse"); }
            set { this.Set("UnhookMouse", value); }
        }

        /// <summary>
        /// Gets or sets the buffer count of this config.
        /// </summary>
        [XmlElement("d3d_presentparams_buffercount")]
        public Int32 D3DPresentParamsBufferCount
        {
            get { return this.Get<Int32>("D3DPresentParamsBufferCount"); }
            set { this.Set("D3DPresentParamsBufferCount", value); }
        }

        /// <summary>
        /// Gets or sets the swap effect of this config.
        /// </summary>
        [XmlElement("d3d_presentparams_swapeffect")]
        public Int32 D3DPresentParamsSwapEffect
        {
            get { return this.Get<Int32>("D3DPresentParamsSwapEffect"); }
            set { this.Set("D3DPresentParamsSwapEffect", value); }
        }

        /// <summary>
        /// Gets or sets the test server property of this config.
        /// </summary>
        [XmlElement("test_server")]
        public Boolean TestServer
        {
            get { return this.Get<Boolean>("TestServer"); }
            set { this.Set("TestServer", value); }
        }
    }
}
