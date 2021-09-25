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
            "UI_Common_WindowFrame01",
            "UI_Common_WindowFrame02",
            "UI_Common_WindowFrame03",
            "UI_Common_WindowFrame04"//no 05 as it is a speaker box
        };
        public List<Texture2D> windows;
        private String _filePath = "";

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
                    windows.Add(ReadTextureFromFile(_filePath + name + ".png"));
                }
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
        public void MakeReadable(Texture2D texture)
        {
            try
            {
                Traverse trav = Traverse.Create(texture);
                foreach(string field in trav.Fields())
                {
                    ModComponent.Log.LogInfo(field);
                }
                
                trav.Property("isReadable").SetValue(true);
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogError($"[WindowPainter].[{nameof(MakeReadable)}]:{ex}");
            }
        }
        public void TintTexture(Texture2D texture, Texture2D source, Color tint)
        {
            if(!(texture.width == source.width) || !(texture.height == source.height))
            {
                return;
            }
            Color[] cols = source.GetPixels();
            for (int i = 0; i < cols.Length; ++i)
            {
                cols[i] = Color.Lerp(cols[i], tint, 0.33f);
            }
            texture.SetPixels(cols);
        }
        public static Texture2D ReadTextureFromFile(String fullPath)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
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
        public void RecolorKnown()
        {
            try
            {
                foreach (int instanceID in tintedTextures)
                {
                    Texture2D texture = Object.FindObjectFromInstanceID(instanceID).Cast<Texture2D>();
                    if (!texture.isReadable)
                    {
                        MakeReadable(texture);
                    }
                    TintTexture(texture, windows.Find(x => x.name == texture.name), ModComponent.Instance.Config.Window.Color);
                }
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[WindowPainter].[{nameof(RecolorKnown)}]:{ex}");
            }
        }


        public void GatherKnownTextures()
        {
            List<Object> textures = new List<Object>(Resources.FindObjectsOfTypeAll(Il2CppType.Of<Texture2D>()));
            List<Object> windowFrame = textures.FindAll(x => textureList.Contains(x.name));
            foreach(Object tex in windowFrame)
            {
                if (!tintedTextures.Contains(tex.GetInstanceID()))
                {
                    Texture2D texture = Object.FindObjectFromInstanceID(tex.GetInstanceID()).Cast<Texture2D>();
                    //go ahead and tint on the way in
                    if (!texture.isReadable)
                    {
                        MakeReadable(texture);
                    }
                    TintTexture(texture, windows.Find(x => x.name == texture.name), ModComponent.Instance.Config.Window.Color);
                    tintedTextures.Add(texture.GetInstanceID());
                }
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
                ModComponent.Log.LogInfo("RefreshKey.Value");
                Boolean isPressed = InputManager.GetKeyUp(ModComponent.Instance.Config.Window.Refresh);//todo:make this configurable
                ModComponent.Log.LogInfo("isPressed?");
                if (isPressed)
                {
                    ModComponent.Log.LogInfo("isPressed!");
                    RecolorKnown();
                }
                ProccessColorChanges();
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[{nameof(WindowPainter)}.{nameof(Update)}]: {ex}");
            }
        }
    }
}
