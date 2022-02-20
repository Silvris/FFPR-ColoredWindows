using BepInEx.Configuration;
using FFPR_ColoredWindows.IL2CPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFPR_ColoredWindows
{
    public enum RecolorMode
    {
        TintBase = 0,
        UsePalette = 1
    }
    public class WindowTexture
    {
        //ReferenceColors is a static 8 colors that you use to map each palette to the real texture
        //Our pixel generation function will match these to the Config colors
        //why the fuck is Color.yellow not 1,1,0
        //it's like, 1, 0.9, 0.1
        private readonly static List<Color> ReferenceColors = new List<Color> { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, new Color(1,1,0), Color.white, Color.black };
        private ConfigFile Config { get; set; }
        private ConfigEntry<Color> Color1 { get; set; }
        private ConfigEntry<Color> Color2 { get; set; }
        private ConfigEntry<Color> Color3 { get; set; }
        private ConfigEntry<Color> Color4 { get; set; }
        private ConfigEntry<Color> Color5 { get; set; }
        private ConfigEntry<Color> Color6 { get; set; }
        private ConfigEntry<Color> Color7 { get; set; }
        private ConfigEntry<Color> Color8 { get; set; }
        private ConfigEntry<float> TintFactor { get; set; }
        private ConfigEntry<RecolorMode> RecolorMode {get;set;}
        private SpriteData spriteData = null;
        public SpriteData SpriteData { get => spriteData; set => spriteData = value; }
        private Texture2D TextureBase { get; set; }
        private string _Name = "";
        public string Name { get => _Name; set => _Name = value; }
        public int Width => TextureBase.width;
        public int Height => TextureBase.height;
        
        public WindowTexture(ConfigFile file, string name, Texture2D baseTex)
        {
            Config = file;
            Name = name;
            TextureBase = baseTex;
            TextureBase.name = name;
            RecolorMode = Config.Bind(new ConfigDefinition(Name, nameof(RecolorMode)), FFPR_ColoredWindows.RecolorMode.TintBase, new ConfigDescription("The mode to use when recoloring this texture. \n Tint: the original texture will be tinted based on a chosen color. \n Palette: the pixels of the image will be replaced based on a chosen palette."));
            Color1 = Config.Bind(new ConfigDefinition(Name, nameof(Color1)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the red pixels of the texture in palette mode. \n\n For tint mode, this will be the color the texture is tinted with."));
            Color2 = Config.Bind(new ConfigDefinition(Name, nameof(Color2)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the green pixels of the texture in palette mode."));
            Color3 = Config.Bind(new ConfigDefinition(Name, nameof(Color3)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the blue pixels of the texture in palette mode."));
            Color4 = Config.Bind(new ConfigDefinition(Name, nameof(Color4)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the cyan pixels of the texture in palette mode."));
            Color5 = Config.Bind(new ConfigDefinition(Name, nameof(Color5)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the magenta pixels of the texture in palette mode."));
            Color6 = Config.Bind(new ConfigDefinition(Name, nameof(Color6)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the yellow pixels of the texture in palette mode."));
            Color7 = Config.Bind(new ConfigDefinition(Name, nameof(Color7)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the white pixels of the texture in palette mode."));
            Color8 = Config.Bind(new ConfigDefinition(Name, nameof(Color8)), new Color(1f, 1f, 1f), new ConfigDescription("The color to replace the black pixels of the texture in palette mode."));
            TintFactor = Config.Bind(new ConfigDefinition(Name, nameof(TintFactor)), 0.33f, new ConfigDescription("The strength of the tinting between the original texture and chosen color, where 0 is the unedited texture, while 1 is full chosen color. \n Note that transparency will be maintained from the original image. \n This has no effect when not in tint mode."));
            
            RecolorMode.SettingChanged += wc_SettingsChanged;
            Color1.SettingChanged += wc_SettingsChanged;
            Color2.SettingChanged += wc_SettingsChanged;
            Color3.SettingChanged += wc_SettingsChanged;
            Color4.SettingChanged += wc_SettingsChanged;
            Color5.SettingChanged += wc_SettingsChanged;
            Color6.SettingChanged += wc_SettingsChanged;
            Color7.SettingChanged += wc_SettingsChanged;
            Color8.SettingChanged += wc_SettingsChanged;
            TintFactor.SettingChanged += wc_SettingsChanged;
        }

        private List<Color> ColorsList => new List<Color> { Color1.Value, Color2.Value, Color3.Value, Color4.Value, Color5.Value, Color6.Value, Color7.Value, Color8.Value };
        private float tintFactor => TintFactor.Value;
        private Color tintColor => Color1.Value;
        private RecolorMode mode => RecolorMode.Value;
        public Color[] TintTexture()
        {
            Color[] cols = TextureBase.GetPixels();
            for (int i = 0; i < cols.Length; ++i)
            {
                float a = cols[i].a;
                cols[i] = Color.Lerp(cols[i], tintColor, tintFactor);
                cols[i].a = a;
            }
            return cols;
        }
        public bool MatchColor(Color first, Color second)
        {
            //need this to have an alpha-ignoring comparison
            if(first.r == second.r)
            {
                if(first.g == second.g)
                {
                    if(first.b == second.b)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Color[] PaletteTexture()
        {
            List<Color> runningColors = ColorsList;
            Color[] mask = TextureBase.GetPixels();
            Color[] cols = new Color[mask.Length];
            for(int i = 0; i < cols.Length; i++)
            {
                int index = ReferenceColors.FindIndex(x => MatchColor(x,mask[i]));
                if(index != -1)
                {
                    cols[i] = runningColors[index];
                }
                else
                {
                    cols[i] = mask[i];
                }
                cols[i].a = mask[i].a;
            }
            return cols;
        }

        public Color[] GetPixels()
        {
            if(mode == FFPR_ColoredWindows.RecolorMode.UsePalette)
            {
                return PaletteTexture();
            }
            else
            {
                return TintTexture();
            }
        }
        public static void wc_SettingsChanged(object sender, EventArgs e)
        {
            ModComponent.Instance.Painter.RecolorTextures();
        }
    }
}
