using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFPR_ColoredWindows
{
    public class WindowsConfig
    {
        private const String Section = "Windows";
        public ConfigEntry<float> Red { get; set; }
        public ConfigEntry<float> Green { get; set; }
        public ConfigEntry<float> Blue { get; set; }
        public ConfigEntry<KeyCode> RefreshKey { get; set; }

        public WindowsConfig(ConfigFile file)
        {
            Red = file.Bind(Section, nameof(Red), (float)255, new ConfigDescription("The red component of the window color, takes values from 0-255.", new AcceptableValueRange<float>(0, 255)));
            Green = file.Bind(Section, nameof(Green), (float)255, new ConfigDescription("The green component of the window color, takes values from 0-255.", new AcceptableValueRange<float>(0, 255)));
            Blue = file.Bind(Section, nameof(Blue), (float)255, new ConfigDescription("The blue component of the window color, takes values from 0-255.", new AcceptableValueRange<float>(0, 255)));
            RefreshKey = file.Bind(Section, nameof(RefreshKey), KeyCode.F6, $"Window color refresh key.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");

            
        }
        public Color Color => new Color(Red.Value, Green.Value, Blue.Value);
        public KeyCode Refresh => RefreshKey.Value;
    }
    public class Configuration
    {
        public WindowsConfig Window { get; set; }
        public Configuration()
        {
            using (var log = BepInEx.Logging.Logger.CreateLogSource("ColoredWindows Config"))
            {
                try
                {
                    log.LogInfo($"Initializing {nameof(Configuration)}");

                    ConfigFile file = new ConfigFile(GetConfigurationPath(), true, ownerMetadata: null);
                    Window = new WindowsConfig(file);

                    log.LogInfo($"{nameof(Configuration)} initialized successfully.");
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to initialize {nameof(Configuration)}: {ex}");
                    throw;
                }
            }
        }

        private static String GetConfigurationPath()
        {
            return Utility.CombinePaths(Paths.ConfigPath, "silvris.ffpr.colored_windows.cfg");
        }
    }
}
