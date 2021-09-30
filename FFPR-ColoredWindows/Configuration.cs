using BepInEx;
using BepInEx.Configuration;
using FFPR_ColoredWindows.IL2CPP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFPR_ColoredWindows
{
    public class WindowsConfig
    {
        private const String Section = "Windows";
        internal const String CTG_ID = "Colored Windows";
        public ConfigFile Config;
        public ConfigEntry<Color> BackgroundColor { get; set; }
        public ConfigEntry<Color> BorderColor { get; set; }
        public ConfigEntry<float> BackgroundFactor { get; set; }
        public ConfigEntry<float> BorderFactor {get; set; }
        public ConfigEntry<KeyCode> RecolorKey { get; set; }
        public ConfigEntry<float> RefreshRate { get; set; }

        public WindowsConfig()
        {
            //used from Sinai's ConfigManager example
            TomlTypeConverter.AddConverter(typeof(Color), new TypeConverter()
            {
                ConvertToObject = (string s, Type t) =>
                {
                    var split = s.Split(',');
                    var c = new CultureInfo("en-US");
                    return new Color()
                    {
                        r = float.Parse(split[0], c),
                        g = float.Parse(split[1], c),
                        b = float.Parse(split[2], c),
                        a = float.Parse(split[3], c)
                    };
                },
                ConvertToString = (object o, Type t) =>
                {
                    var x = (Color)o;
                    return string.Format(new CultureInfo("en-US"), "{0},{1},{2},{3}",
                        x.r, x.g, x.b, x.a);
                }
            });
            Config = EntryPoint.Instance.Config;
            BackgroundColor = Config.Bind(new ConfigDefinition(Section, nameof(BackgroundColor)), new Color(0f,0f,1f), new ConfigDescription("The color to tint the background with."));
            BorderColor = Config.Bind(new ConfigDefinition(Section, nameof(BorderColor)), Color.white, new ConfigDescription("The color to tint the border with."));
            BackgroundFactor = Config.Bind(Section, nameof(BackgroundFactor), 0.5f, new ConfigDescription("The strength of the tinting between the original window background and chosen color, where 0 is the unedited background, while 1 is full chosen color. Note that transparency will be maintained from the original image.", new AcceptableValueRange<float>(0f, 1f)));
            BorderFactor = Config.Bind(Section, nameof(BorderFactor), 0f, new ConfigDescription("The strength of the tinting between the original window border and chosen color, where 0 is the unedited border, while 1 is full chosen color. Note that transparency will be maintained from the original image.", new AcceptableValueRange<float>(0f, 1f)));
            RecolorKey = Config.Bind(Section, nameof(RecolorKey), KeyCode.F6, $"The key to pop up the GUI menu to select colors and apply them.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");
            RefreshRate = Config.Bind(Section, nameof(RefreshRate), 10.0f, $"The amount of time the plugin should wait before trying to find more valid textures, in seconds.{Environment.NewLine} NOTE: if this is set too low, you will see major performance issues.");
            BackgroundColor.SettingChanged += wc_SettingsChanged;
            BorderColor.SettingChanged += wc_SettingsChanged;
            BackgroundFactor.SettingChanged += wc_SettingsChanged;
            BorderFactor.SettingChanged += wc_SettingsChanged;
        }
        public Color BGColor => BackgroundColor.Value;
        public Color BRColor => BorderColor.Value;
        public KeyCode Recolor => RecolorKey.Value;
        public float Refresh => RefreshRate.Value;
        public static void wc_SettingsChanged(object sender, EventArgs e)
        {
            ModComponent.Instance.Painter.RecolorTextures();
        }
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

                    Window = new WindowsConfig();

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
