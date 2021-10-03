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
        public List<SpriteData> spriteDatas;
        public static String[] textureList =
        {
            "UI_Common_WindowFrame01",//01 is separated from this, as it is addressable, we only have to replace it once
            "UI_Common_WindowFrame02",
            "UI_Common_WindowFrame03",
            "UI_Common_WindowFrame04"//no 05 as it is a speaker box
            //"UI_Common_ATBgauge02"

        };
        public List<Texture2D> windows;
        public List<Texture2D> windowDefs;
        private String _filePath = "";

        public float refreshRate = 0.0f;//an immediate start

        public WindowPainter()
        {
            try
            {
                Assembly thisone = Assembly.GetExecutingAssembly();
                _filePath = Path.GetDirectoryName(thisone.Location) + "/ColoredWindows/";
                windows = new List<Texture2D>();
                windowDefs = new List<Texture2D>();
                spriteDatas = new List<SpriteData>();
                tintedTextures = new List<int>();
                foreach (String name in textureList)
                {
                    Texture2D tex = ReadTextureFromFile(_filePath + name + ".png", name);
                    tex.hideFlags = HideFlags.HideAndDontSave;
                    ModComponent.Log.LogInfo($"Loaded texture:{_filePath+name+".png"}");
                    if (File.Exists(_filePath + name + ".spriteData"))
                    {
                        SpriteData sd = new SpriteData(File.ReadAllLines(_filePath + name + ".spriteData"), name);
                        spriteDatas.Add(sd);
                        tex.wrapMode = sd.hasWrap ? sd.wrapMode : tex.wrapMode;

                    }
                    windowDefs.Add(tex);
                }
                foreach (Texture2D tex in windowDefs)
                {
                    //create a copy to be edited
                    Texture2D nTex = new Texture2D(tex.width, tex.height) { name = tex.name, filterMode = FilterMode.Point };
                    nTex.hideFlags = HideFlags.HideAndDontSave;
                    Graphics.CopyTexture(tex, nTex);
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
        public void TintTexture(Texture2D replace, Color[] cols, Color tint, float factor)
        {
            //this is only to be used on textures we generate
            Color[] colcheck = replace.GetPixels();
            if (!(colcheck.Length == cols.Length)) return;
            for (int i = 0; i < cols.Length; ++i)
            {
                float a = cols[i].a;
                cols[i] = Color.Lerp(cols[i], tint, factor);
                cols[i].a = a;
            }
            replace.SetPixels(cols);
            replace.Apply();
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
                    //ModComponent.Log.LogInfo(tex.name);
                    if (tex.name == "UI_Common_WindowFrame01")
                    {
                        //ModComponent.Log.LogInfo(ModComponent.Instance.Config.Window.BorderColor.ToString());

                        TintTexture(tex, windowDefs.Find(x => x.name == tex.name).GetPixels(), ModComponent.Instance.Config.Window.BRColor,ModComponent.Instance.Config.Window.BorderFactor.Value);
                    }
                    else
                    {
                        //ModComponent.Log.LogInfo("Is Background");
                        TintTexture(tex, windowDefs.Find(x => x.name == tex.name).GetPixels(), ModComponent.Instance.Config.Window.BGColor,ModComponent.Instance.Config.Window.BackgroundFactor.Value);
                    }
                }

            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[WindowPainter].[{nameof(RecolorTextures)}]:{ex}");
            }
        }

        public void SetImageSprite(Image image,int instanceId)
        {
            Sprite original = image.sprite;
            //check for overriding spriteData
            bool hasSD = spriteDatas.Exists(x => x.name == image.sprite.texture.name);
            ModComponent.Log.LogInfo(hasSD);
            if (hasSD)
            {
                SpriteData sd = spriteDatas.Find(x => x.name == image.sprite.texture.name);
                ModComponent.Log.LogInfo($"{sd.name} {sd.hasRect} {sd.hasPivot} {sd.hasBorder} {sd.hasType}");
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
            tintedTextures.Add(instanceId);
        }

        public void GatherKnownTextures()
        {
            try
            {
                List<Object> gobs = new List<Object>(Resources.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()));
                List<Object> images = gobs.FindAll(x => x.name == "image");//this is cursed, don't do this
                List<Object> gauges = gobs.FindAll(x => x.name == "gauge");//why am I adding another
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
                                        SetImageSprite(image, gob.GetInstanceID());
                                    }
                                }
                            }

                        }


                    }
                }
                foreach (Object img in gauges)
                {
                    if (!tintedTextures.Contains(img.GetInstanceID()))
                    {
                        GameObject gob = Object.FindObjectFromInstanceID(img.GetInstanceID()).Cast<GameObject>();
                        Image image = gob.GetComponent<Image>();
                        //go ahead and tint on the way in
                        if (image != null)
                        {
                            if (image.sprite != null)
                            {
                                if (image.sprite.texture != null)
                                {
                                    if (textureList.Contains(image.sprite.texture.name))
                                    {
                                        SetImageSprite(image, gob.GetInstanceID());
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
                /*
                if (_resourceManager is null)
                {
                    _resourceManager = ResourceManager.Instance;
                    if (_resourceManager is null)
                        return;
                    //wait for loading to happen
                    
                    ModComponent.Log.LogInfo($"Waiting for ResourceManager.");
                }
                if((_resourceManager != null) && (!atbLoaded))
                {
                    if (_resourceManager.completeAssetDic.ContainsKey("Assets/GameAssets/Common/UI/Common/Sprites/UI_Common_ATBgauge02")) 
                    {
                        Sprite original = _resourceManager.completeAssetDic["Assets/GameAssets/Common/UI/Common/Sprites/UI_Common_ATBgauge02"].Cast<Sprite>();
                        Sprite newSpr = Sprite.Create(windows.Find(x => x.name == original.name), original.rect, original.pivot, original.pixelsPerUnit, 0, SpriteMeshType.Tight, original.border);
                        newSpr.name = original.name;
                        _resourceManager.completeAssetDic["Assets/GameAssets/Common/UI/Common/Sprites/UI_Common_ATBgauge02"] = newSpr;
                        //Object.Destroy(original);//make sure to use destroy, and not destroyImmediate
                        ModComponent.Log.LogInfo("ATB bar loaded!");
                        atbLoaded = true;//assures this only runs once, getting us a much cleaner replacement
                    }
                    else
                    {
                        ModComponent.Log.LogInfo("ATB bar not loaded.");
                    }
                }*///somehow, none of this works
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
