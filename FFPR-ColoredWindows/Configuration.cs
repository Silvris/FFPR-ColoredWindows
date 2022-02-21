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
    public class ColorWrapper
    {
        public static float ClampColorVal(float input)
        {
            if(input > 1)
            {
                return input / 255;
            }
            else
            {
                return input;
            }
        }
        private Color _Color;
        public float r { get => _Color.r; set => _Color.r = ClampColorVal(value); }
        public float g { get => _Color.g; set => _Color.g = ClampColorVal(value); }
        public float b { get => _Color.b; set => _Color.b = ClampColorVal(value); }
        public float a { get => _Color.a; set => _Color.a = ClampColorVal(value); }
        public Color color { get => _Color; set => _Color = value; }
        public ColorWrapper(float R, float G, float B, float A)
        {
            
            _Color = new Color(ClampColorVal(R), ClampColorVal(G), ClampColorVal(B), ClampColorVal(A));
        }
        public ColorWrapper(float R, float G, float B)
        {

            _Color = new Color(ClampColorVal(R), ClampColorVal(G), ClampColorVal(B));
        }
        public ColorWrapper()
        {
            _Color = Color.white;
        }
    }
    public class WindowsConfig
    {
        private const String Windows = "Windows";
        private const String Text = "Text";
        internal const String CTG_ID = "Colored Windows";
        public ConfigFile Config;
        public ConfigEntry<ColorWrapper> _TextColor { get; set; }
        public ConfigEntry<ColorWrapper> _ShadowColor { get; set; }
        public ConfigEntry<ColorWrapper> _SpecialTextColor { get; set; }
        public ConfigEntry<ColorWrapper> _FocusTextColor { get; set; }
        public ConfigEntry<ColorWrapper> _FocusShadowColor { get; set; }

        public WindowsConfig()
        {
            //used from Sinai's ConfigManager example
            TomlTypeConverter.AddConverter(typeof(ColorWrapper), new TypeConverter()
            {
                ConvertToObject = (string s, Type t) =>
                {
                    if (s.StartsWith("#"))
                    {
                        return new ColorWrapper(
                            Convert.ToSingle(int.Parse(s.Substring(1, 2), System.Globalization.NumberStyles.HexNumber)),
                            Convert.ToSingle(int.Parse(s.Substring(3, 2), System.Globalization.NumberStyles.HexNumber)),
                            Convert.ToSingle(int.Parse(s.Substring(5, 2), System.Globalization.NumberStyles.HexNumber)),
                            Convert.ToSingle(int.Parse(s.Substring(7, 2), System.Globalization.NumberStyles.HexNumber))
                            );
                    }
                    else
                    {
                        var split = s.Split(',');
                        var c = new CultureInfo("en-US");
                        return new ColorWrapper()
                        {
                            r = float.Parse(split[0], c),
                            g = float.Parse(split[1], c),
                            b = float.Parse(split[2], c),
                            a = float.Parse(split[3], c)
                        };
                    }

                },
                ConvertToString = (object o, Type t) =>
                {
                    var x = (ColorWrapper)o;
                    return string.Format(new CultureInfo("en-US"), "{0},{1},{2},{3}",
                        x.r, x.g, x.b, x.a);
                }
            });
            Config = EntryPoint.Instance.Config;
            /*
            _TextColor = Config.Bind(new ConfigDefinition(Text, "Text Color"), new ColorWrapper(.91f, .91f, .91f, 1f), new ConfigDescription("The color of non-focused text in menus"));
            _ShadowColor = Config.Bind(new ConfigDefinition(Text, "Shadow Color"), new ColorWrapper(0.68f, 0.68f, 0.68f,1f), new ConfigDescription("The color of non-focused shadows in menus"));
            _SpecialTextColor = Config.Bind(new ConfigDefinition(Text, "Special Text Color"), new ColorWrapper(0.26f, 0.87f, 1f, 1f), new ConfigDescription("The color of special text such as the autosave slot."));
            _FocusTextColor = Config.Bind(new ConfigDefinition(Text, "Focused Text Color"), new ColorWrapper(1f, 1f, 0f,1f), new ConfigDescription("The color of focused text in menus"));
            _FocusShadowColor = Config.Bind(new ConfigDefinition(Text, "Focused Shadow Color"), new ColorWrapper(0f, 0f, 0f, 1f), new ConfigDescription("The color of focused shadows in menus"));
            */
        }
        public Color TextColor => _TextColor.Value.color;
        public Color ShadowColor => _ShadowColor.Value.color;
        public Color FocusTextColor => _FocusTextColor.Value.color;
        public Color FocusShadowColor => _FocusShadowColor.Value.color;
        public Color SpecialTextColor => _SpecialTextColor.Value.color;
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
