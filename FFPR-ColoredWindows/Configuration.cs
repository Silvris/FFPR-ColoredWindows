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
        public ConfigEntry<float> BackgroundRed { get; set; }
        public ConfigEntry<float> BackgroundGreen { get; set; }
        public ConfigEntry<float> BackgroundBlue { get; set; }
        public ConfigEntry<float> BorderRed { get; set; }
        public ConfigEntry<float> BorderGreen { get; set; }
        public ConfigEntry<float> BorderBlue { get; set; }
        public ConfigEntry<KeyCode> RecolorKey { get; set; }
        public ConfigEntry<float> RefreshRate { get; set; }

        public WindowsConfig(ConfigFile file)
        {
            BackgroundRed = file.Bind(Section, nameof(BackgroundRed), (float)100, new ConfigDescription("The red component of the window background, takes values from 0%-100%.", new AcceptableValueRange<float>(0, 100)));
            BackgroundGreen = file.Bind(Section, nameof(BackgroundGreen), (float)100, new ConfigDescription("The green component of the window background, takes values from 0%-100%.", new AcceptableValueRange<float>(0, 100)));
            BackgroundBlue = file.Bind(Section, nameof(BackgroundBlue), (float)100, new ConfigDescription("The blue component of the window background, takes values from 0%-100%.", new AcceptableValueRange<float>(0, 100)));
            BorderRed = file.Bind(Section, nameof(BorderRed), (float)100, new ConfigDescription("The red component of the window border, takes values from 0%-100%.", new AcceptableValueRange<float>(0, 100)));
            BorderGreen = file.Bind(Section, nameof(BorderGreen), (float)100, new ConfigDescription("The green component of the window border, takes values from 0%-100%.", new AcceptableValueRange<float>(0, 100)));
            BorderBlue = file.Bind(Section, nameof(BorderBlue), (float)100, new ConfigDescription("The blue component of the window border, takes values from 0%-100%.", new AcceptableValueRange<float>(0, 100)));
            RecolorKey = file.Bind(Section, nameof(RecolorKey), KeyCode.F6, $"Window color refresh key.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");
            RefreshRate = file.Bind(Section, nameof(RefreshRate), 10.0f, $"The amount of time the plugin should wait before trying to find more valid textures, in seconds.{Environment.NewLine} NOTE: if this is set too low, you will see major performance issues.");
            
        }
        public Color BackgroundColor => new Color(BackgroundRed.Value/100, BackgroundGreen.Value/100, BackgroundBlue.Value/100);
        public Color BorderColor => new Color(BorderRed.Value / 100, BorderGreen.Value / 100, BorderBlue.Value / 100);
        public KeyCode Recolor => RecolorKey.Value;
        public float Refresh => RefreshRate.Value;
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
