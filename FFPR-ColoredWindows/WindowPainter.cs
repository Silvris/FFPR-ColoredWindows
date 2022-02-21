using Last.Management;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFPR_ColoredWindows.IL2CPP;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnhollowerRuntimeLib;
using UnityEngine.InputSystem;
using HarmonyLib;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace FFPR_ColoredWindows.Main
{
    public sealed class WindowPainter
    {
        public WindowPainter Instance;

        public ResourceManager _resourceManager;
        public List<string> textureList = new List<string>
        {
            "UI_Common_WindowFrame01",
            "UI_Common_WindowFrame02",
            "UI_Common_WindowFrame03",
            "UI_Common_WindowFrame04",//no 05 as it is a speaker box

        };
        public List<string> targetGobs = new List<string> {// as a side note, if you were somehow to get the game to load in touch mode, none of this would work
            "Assets/GameAssets/Common/UI/Common/common_window",
            "Assets/GameAssets/Common/UI/Key/Common/message_window",
            "Assets/GameAssets/Common/UI/Key/Common/message_multiple_window",
            "Assets/GameAssets/Common/UI/Key/Common/message_choices_window",
            "Assets/GameAssets/Common/UI/Key/Common/message_system_window",
            "Assets/GameAssets/Common/UI/Key/Common/message_selected_window",
            "Assets/GameAssets/Common/UI/Key/Common/battle_message_window",
            "Assets/GameAssets/Serial/Res/UI/Key/NewGame/Prefab/new_game",//there's like 7 within this one
            "Assets/GameAssets/Common/UI/Key/License/Prefab/license_window",
            "Assets/GameAssets/Common/UI/Key/LoadGame/Prefab/loadGame",
            "Assets/GameAssets/Common/UI/Key/Option/Prefab/option",
            "Assets/GameAssets/Common/UI/Key/MainMenu/Item/Prefab/item_info",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/StatusList/Prefabs/status_info",
            "Assets/GameAssets/Common/UI/Key/MainMenu/Save/Prefab/save_popup",
            "Assets/GameAssets/Common/UI/Key/MainMenu/Equipment/Prefab/equipment_info",
            "Assets/GameAssets/Common/UI/Key/MainMenu/Save/Prefab/save_info",
            "Assets/GameAssets/Common/UI/Key/MainMenu/Common/Prefab/key_help",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/Item/Prefabs/item_target_select_list",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/Equipment/Prefabs/equipment_status_window",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/Config/Prefab/config",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/Equipment/Prefabs/equipment_description_window",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/Ability/Prefabs/ability_info",
            "Assets/GameAssets/Common/UI/Key/MainMenu/Common/Prefab/crystal_info",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/Common/Prefabs/comand_menu",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/StatusDetails/Prefabs/status_details",
            "Assets/GameAssets/Common/UI/Key/Popup/Prefabs/jobchange_popup", //FF5 job menu
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/JobChange/Prefabs/jobchange_info",
            "Assets/GameAssets/Serial/Res/UI/Key/MainMenu/AbilityChange/Prefabs/ability_change_info",
            "Assets/GameAssets/Common/UI/Key/Popup/Prefabs/input_popup",
            "Assets/GameAssets/Common/UI/Key/Popup/Prefabs/game_over_select_popup",
            "Assets/GameAssets/Common/UI/Key/Popup/Prefabs/change_name_popup",
            "Assets/GameAssets/Common/UI/Key/Popup/Prefabs/common_popup",
            "Assets/GameAssets/Common/UI/Key/Popup/Prefabs/change_magic_stone_popup",//FF6 Magicite
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/trade_popup",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/item_menu_window",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/equip_menu_window",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/sell_list_window",
            "Assets/GameAssets/Serial/Res/UI/Key/Shop/Prefabs/magic_list_window",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/equip_list_window",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/item_list_window",
            "Assets/GameAssets/Serial/Res/UI/Key/Shop/Prefabs/magic_trade_popup",
            "Assets/GameAssets/Serial/Res/UI/Key/Shop/Prefabs/get_magic_list_window",
            "Assets/GameAssets/Serial/Res/UI/Key/Shop/Prefabs/magic_menu_window",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/magic_item_list_window",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/shop_base",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/trade_magic_popup",
            "Assets/GameAssets/Common/UI/Key/Shop/Prefabs/magic_item_menu_window",
            "Assets/GameAssets/Common/UI/Key/ChangeName/Prefabs/change_name_base",
            "Assets/GameAssets/Common/UI/Key/PartySetting/Prefabs/party_setting",
            "Assets/GameAssets/Common/UI/Key/ScreenTimer/screen_timer",
            "Assets/GameAssets/Common/UI/Touch/Field/Prefabs/field_help",
            "Assets/GameAssets/Serial/Res/UI/Key/Battle/Prefabs/battle_info_window",
            "Assets/GameAssets/Common/UI/Key/Battle/Prefabs/command_input_window",
            "Assets/GameAssets/Common/UI/Key/Battle/Prefabs/game_over_popup",
            "Assets/GameAssets/Serial/Res/UI/Key/Result/Prefabs/result_menu",
            "Assets/GameAssets/Serial/Res/UI/Key/Battle/Prefabs/equip_select_window",
            "Assets/GameAssets/Common/UI/Key/Battle/Prefabs/command_message_window",
            "Assets/GameAssets/Serial/Res/UI/Key/Battle/Prefabs/ability_info_window",
            "Assets/GameAssets/Common/UI/Key/Battle/Prefabs/special_ability_help",
            "Assets/GameAssets/Common/UI/Key/GpsMenu/gps_menu_base",
            "Assets/GameAssets/Common/UI/Key/ExpandMinimap/expand_minimap_base",
            "Assets/GameAssets/Common/UI/Key/PlayData/Prefab/play_data_window",
            "Assets/GameAssets/Common/UI/Key/ExtraLibrary/library_menu_base",
            "Assets/GameAssets/Common/UI/Key/ExtraLibrary/library_dungeon_base",
            "Assets/GameAssets/Common/UI/Key/ExtraLibrary/library_field_base",
            "Assets/GameAssets/Common/UI/Key/ExtraLibrary/library_info_base",
            "Assets/GameAssets/Common/UI/Key/SoundPlayer/sound_player_base_controller",
            "Assets/GameAssets/Common/UI/Key/ExtraGallery/gallerydetails_base",
            "Assets/GameAssets/Common/UI/Key/ExtraGallery/gallerytop_base",
            "Assets/GameAssets/Common/UI/Key/SaveWindow/Prefabs/save_window"

        };
        public List<Texture2D> windows;
        public List<WindowTexture> windowDefs;
        public List<string> loadedScenes;
        private String _filePath = "";
        public Dictionary<string, int> knownObjects;
        public WindowPainter()
        {
            try
            {
                Instance = this;
                Assembly thisone = Assembly.GetExecutingAssembly();
                _filePath = Path.GetDirectoryName(thisone.Location) + "/ColoredWindows/";
                
                if (ModComponent.isATB)
                {
                    targetGobs.Add("Assets/GameAssets/Common/UI/Key/Battle/Prefabs/pause_view_ATB");
                    targetGobs.Add("Assets/GameAssets/Serial/Res/UI/Key/Battle/Prefabs/player_info_content");
                    textureList.Add("UI_Common_ATBgauge02");
                    textureList.Add("UI_Common_ATBgauge03");
                }
                else
                {
                    targetGobs.Add("Assets/GameAssets/Common/UI/Key/Battle/Prefabs/pause_view");
                }
                windows = new List<Texture2D>();
                windowDefs = new List<WindowTexture>();
                loadedScenes = new List<string>();
                knownObjects = new Dictionary<string, int>();
                List<string> texListDel = new List<string>();
                foreach (String name in textureList)
                {
                    try
                    {
                        Texture2D tex = ReadTextureFromFile(_filePath + name + ".png", name);
                        tex.hideFlags = HideFlags.HideAndDontSave;
                        ModComponent.Log.LogInfo($"Loaded texture:{_filePath + name + ".png"}");
                        WindowTexture wTex = new WindowTexture(EntryPoint.Instance.Config, name, tex);
                        if (File.Exists(_filePath + name + ".spriteData"))
                        {
                            SpriteData sd = new SpriteData(File.ReadAllLines(_filePath + name + ".spriteData"), name);
                            tex.wrapMode = sd.hasWrap ? sd.wrapMode : tex.wrapMode;
                            wTex.SpriteData = sd;

                        }
                        windowDefs.Add(wTex);
                    }catch(Exception ex)
                    {
                        ModComponent.Log.LogError($"Unable to load texture: {ex}");
                        texListDel.Add(name);
                    }

                }
                foreach(string name in texListDel)
                {
                    textureList.Remove(name);
                }
                foreach (WindowTexture tex in windowDefs)
                {
                    //create a copy to be edited
                    Texture2D nTex = new Texture2D(tex.Width, tex.Height) { name = tex.Name, filterMode = FilterMode.Point };
                    nTex.hideFlags = HideFlags.HideAndDontSave;
                    nTex.SetPixels(tex.GetPixels());
                    nTex.Apply();
                    windows.Add(nTex);
                }
                RecolorTextures();
                ModComponent.Log.LogInfo("Window Painter initialized.");
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogError($"[{nameof(WindowPainter)}.ctor]: {ex}");
            }
        }
        public void OnDestroy()
        {
            ModComponent.Log.LogInfo($"[{nameof(WindowPainter)}].{nameof(OnDestroy)}()");
        }
        
        /* commenting out since they're not needed as of now, may become of use later
        public Texture2D RemoveTrim(Texture2D source)
        {
            Texture2D newTex = new Texture2D(source.width - 6, source.height - 3);
            Color[] colors = source.GetPixels();
            Color[,] colorArr = new Color[newTex.width, newTex.height]; ;
            Color[] newColors = new Color[(newTex.width) * (newTex.height)];
            for(int i = 3; i < source.width - 3; i++)
            {
                for(int j = 0; j < source.height - 3; j++)
                {
                    colorArr[i, j] = colors[i + (j * source.width)];
                }
            }
            for(int j = 0; j < newTex.height; j++)
            {
                for(int i = 0; i < newTex.width; i++)
                {
                    newColors[i + (j * newTex.width)] = colorArr[i, j];
                }
            }
            newTex.SetPixels(newColors);
            return newTex;
        }

        public Texture2D AddTrim(Texture2D source, Color rimColor)
        {
            Texture2D newTex = new Texture2D(source.width + 6, source.height + 3);
            for(int i = 0; i < newTex.width; i++)
            {
                for(int j = 0; j < newTex.height; j++)
                {
                    if((3 < i )&&(i< source.width)&&(j<source.height))
                    {
                        newTex.SetPixel(i, j, source.GetPixel(i - 3, j));
                    }
                    else
                    {
                        newTex.SetPixel(i, j, new Color(77, 96, 177));
                    }
                }
            }
            return newTex;
        }*/

        public static Texture2D ReadTextureFromFile(String fullPath,String Name)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = Name };
                texture.filterMode = FilterMode.Point;
                if (!ImageConversion.LoadImage(texture, bytes))
                    throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");
                
                return texture;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        public void RecolorTextures()
        {
            try
            {
                foreach (Texture2D tex in windows)
                {
                    WindowTexture wTex = windowDefs.Find(x => x.Name == tex.name);
                    tex.SetPixels(wTex.GetPixels());
                    tex.Apply();
                }

            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[WindowPainter].[{nameof(RecolorTextures)}]:{ex}");
            }
        }

        public void SetImageSprite(Image image)
        {
            if(image.sprite != null)
            {
                Sprite original = image.sprite;
                //check for overriding spriteData
                WindowTexture wt = windowDefs.Find(x => x.Name == image.sprite.texture.name);
                bool hasSD = (wt.SpriteData != null);
                //ModComponent.Log.LogInfo(hasSD);
                if (hasSD)
                {
                    SpriteData sd = wt.SpriteData;
                    //ModComponent.Log.LogInfo($"{sd.name} {sd.hasRect} {sd.hasPivot} {sd.hasBorder} {sd.hasType}");
                    Rect r = sd.hasRect ? sd.rect : original.rect;
                    Vector2 p = sd.hasPivot ? sd.pivot : original.pivot;
                    Vector4 b = sd.hasBorder ? sd.border : original.border;
                    Image.Type t = sd.hasType ? sd.type : image.type;
                    image.sprite = Sprite.Create(windows.Find(x => x.name == image.sprite.texture.name), r, p, original.pixelsPerUnit, 0, SpriteMeshType.Tight, b);
                    image.type = t;

                }
                else
                {

                    image.sprite = Sprite.Create(windows.Find(x => x.name == image.sprite.texture.name), original.rect, original.pivot, original.pixelsPerUnit, 0, SpriteMeshType.Tight, original.border);

                }

                //dunno if this will work properly
                image.sprite.name = original.name;
                image.sprite.hideFlags = HideFlags.HideAndDontSave;
                //Object.Destroy(original);//make sure to use destroy, and not destroyImmediate 
                //actually if you destroy at all there is problems
                //the resulting "memory leak" isn't large enough to be a problem
            }

        }

        public void Update()
        {
            try
            {
                //there is nothing to wait for anymore, perish
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[{nameof(WindowPainter)}.{nameof(Update)}]: {ex}");
            }
        }
    }
}
