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
        private const String Windows = "Windows";
        private const String Text = "Text";
        private const String General = "General";
        internal const String CTG_ID = "Colored Windows";
        public ConfigFile Config;
        public ConfigEntry<Color> _TextColor { get; set; }
        public ConfigEntry<Color> _ShadowColor { get; set; }
        public ConfigEntry<Color> _SpecialTextColor { get; set; }
        public ConfigEntry<Color> _FocusTextColor { get; set; }
        public ConfigEntry<Color> _FocusShadowColor { get; set; }
        public ConfigEntry<KeyCode> _AccessGUIMenu { get; set; }
        public static string ToHex(float c)
        {
            //check for in bounds of byte for some reason lol
            byte b = 0x00;
            if(c > 1f&&c < 255f)
            {
                b = Convert.ToByte(c);
            }
            //check 0-1 which is regular color boundaries
            else if (c <1 && c > 0)
            {
                b = Convert.ToByte(c * 255f);
            }
            //set max
            else if (c > 255f)
            {
                b = 0xff;
            }
            //anything else is irrelevant to our situation, keep at 0
            return BitConverter.ToString(new byte[] { b});
        }
        public WindowsConfig()
        {
            //used from Sinai's ConfigManager example
            if (!TomlTypeConverter.CanConvert(typeof(Color)))
                { 
                TomlTypeConverter.AddConverter(typeof(Color), new TypeConverter()
                {
                    ConvertToObject = (string s, Type t) =>
                    {
                        if (string.IsNullOrEmpty(s)) throw new FormatException("Color cannot be null");
                        bool check = ColorUtility.TryParseHtmlString(s, out var o);
                        //ModComponent.Log.LogInfo(check);
                        //ModComponent.Log.LogInfo(o);
                        if (!check) throw new FormatException("Color must follow #RRGGBBAA format, or match UnityEngine.color names");
                        return o;
                    },
                    ConvertToString = (object o, Type t) =>
                    {
                        var x = (Color)o;
                        //ToHtmlStringRGBA is broken for some reason, time to go old school
                            var r = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.r * 255), 0, 255);
                            var g = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.g * 255), 0, 255);
                            var b = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.b * 255), 0, 255);
                            var a = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.a * 255), 0, 255);

                            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
                    }
                });
            }

            Config = EntryPoint.Instance.Config;
            /*
            _TextColor = Config.Bind(new ConfigDefinition(Text, "Text Color"), new Color(.91f, .91f, .91f, 1f), new ConfigDescription("The color of non-focused text in menus"));
            _ShadowColor = Config.Bind(new ConfigDefinition(Text, "Shadow Color"), new Color(0.68f, 0.68f, 0.68f,1f), new ConfigDescription("The color of non-focused shadows in menus"));
            _SpecialTextColor = Config.Bind(new ConfigDefinition(Text, "Special Text Color"), new Color(0.26f, 0.87f, 1f, 1f), new ConfigDescription("The color of special text such as the autosave slot."));
            _FocusTextColor = Config.Bind(new ConfigDefinition(Text, "Focused Text Color"), new Color(1f, 1f, 0f,1f), new ConfigDescription("The color of focused text in menus"));
            _FocusShadowColor = Config.Bind(new ConfigDefinition(Text, "Focused Shadow Color"), new Color(0f, 0f, 0f, 1f), new ConfigDescription("The color of focused shadows in menus"));
            */
            //_AccessGUIMenu = Config.Bind(new ConfigDefinition(General, "GUI Toggle"), KeyCode.F9, new ConfigDescription("The key used to open the dedicated menu for Colored Windows."));
        }
        public Color TextColor => _TextColor.Value;
        public Color ShadowColor => _ShadowColor.Value;
        public Color FocusTextColor => _FocusTextColor.Value;
        public Color FocusShadowColor => _FocusShadowColor.Value;
        public Color SpecialTextColor => _SpecialTextColor.Value;
        public KeyCode AccessGUI => _AccessGUIMenu.Value;
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
