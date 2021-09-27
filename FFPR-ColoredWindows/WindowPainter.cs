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

namespace FFPR_ColoredWindows.Main
{
    public sealed class WindowPainter
    {

        public ResourceManager _resourceManager;
        public List<int> tintedTextures;
        public static String[] textureList =
        {
            "UI_Common_WindowFrame01", //originally had 1, and may still do it, but tinting with alpha messes up
            "UI_Common_WindowFrame02",
            "UI_Common_WindowFrame03",
            "UI_Common_WindowFrame04"//no 05 as it is a speaker box
        };
        public List<Texture2D> windows;
        private String _filePath = "";

        public float refreshRate = 0.0f;//an immediate start

        public WindowPainter()
        {
            try
            {
                Assembly thisone = Assembly.GetExecutingAssembly();
                _filePath = Path.GetDirectoryName(thisone.Location) + "/ColoredWindows/";
                windows = new List<Texture2D>();
                tintedTextures = new List<int>();
                foreach (String name in textureList)
                {
                    Texture2D tex = ReadTextureFromFile(_filePath + name + ".png", name);
                    tex.hideFlags = HideFlags.HideAndDontSave;
                    windows.Add(tex);
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
        /* commenting out since they're not needed as of now
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
        public void TintTexture(Texture2D source, Color tint)
        {
            //this is only to be used on textures we generate
            Color[] cols = source.GetPixels();
            for (int i = 0; i < cols.Length; ++i)
            {
                float a = cols[i].a;
                cols[i] = Color.Lerp(cols[i], tint, 0.33f);
                cols[i].a = a;
            }
            source.SetPixels(cols);
            source.Apply();
        }
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
                    ModComponent.Log.LogInfo(tex.name);
                    if (tex.name == "UI_Common_WindowFrame01")
                    {
                        ModComponent.Log.LogInfo(ModComponent.Instance.Config.Window.BorderColor.ToString());

                        TintTexture(tex, ModComponent.Instance.Config.Window.BorderColor);
                    }
                    else
                    {
                        ModComponent.Log.LogInfo("Is Background");
                        TintTexture(tex, ModComponent.Instance.Config.Window.BackgroundColor);
                    }
                }

            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[WindowPainter].[{nameof(RecolorTextures)}]:{ex}");
            }
        }


        public void GatherKnownTextures()
        {
            try
            {
                List<Object> gobs = new List<Object>(Resources.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()));
                List<Object> images = gobs.FindAll(x => x.name == "image");//this is cursed, don't do this
                foreach (Object img in images)
                {
                    if (!tintedTextures.Contains(img.GetInstanceID()))
                    {
                        GameObject gob = Object.FindObjectFromInstanceID(img.GetInstanceID()).Cast<GameObject>();
                        Image image = gob.GetComponent<Image>();
                        //go ahead and tint on the way in
                        if(image != null)
                        {
                            if(image.sprite != null)
                            {
                                if(image.sprite.texture != null)
                                {
                                    if (textureList.Contains(image.sprite.texture.name))
                                    {
                                        Sprite original = image.sprite;
                                        //dunno if this will work properly
                                        image.sprite = Sprite.Create(windows.Find(x => x.name == image.sprite.texture.name), original.rect, original.pivot, original.pixelsPerUnit,0,SpriteMeshType.Tight,original.border);
                                        image.sprite.name = original.name;
                                        Object.Destroy(original);//make sure to use destroy, and not destroyImmediate
                                        tintedTextures.Add(gob.GetInstanceID());
                                    }
                                }
                            }

                        }


                    }
                }
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError(ex);
                throw ex;
            }

        }

        public bool ProccessColorChanges()
        {
            try
            {
                GatherKnownTextures();
                return true;
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"Unable to tint windows: {ex}");
                return false;
            }
        }

        public void Update()
        {
            refreshRate -= Time.deltaTime;
            try
            {
                if (_resourceManager is null)
                {
                    _resourceManager = ResourceManager.Instance;
                    if (_resourceManager is null)
                        return;
                    //wait for loading to happen
                    
                    ModComponent.Log.LogInfo($"Waiting for window loading.");
                }
                //ModComponent.Log.LogInfo("RefreshKey.Value");
                Boolean isPressed = InputManager.GetKeyUp(ModComponent.Instance.Config.Window.Recolor);//todo:make this configurable
                //ModComponent.Log.LogInfo("isPressed?");
                if (isPressed)
                {
                    //ModComponent.Log.LogInfo("isPressed!");
                    RecolorTextures();
                }


                if (refreshRate <= 0.0f)
                {
                    refreshRate = ModComponent.Instance.Config.Window.Refresh;
                    ProccessColorChanges();
                }
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[{nameof(WindowPainter)}.{nameof(Update)}]: {ex}");
            }
        }
    }
}
