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
        private String _filePath = "";
        private Boolean _windowLoaded = false;

        public WindowPainter()
        {
        }

        public void Awake()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogError($"[{nameof(WindowPainter)}.{nameof(Awake)}]: {ex}");
            }
        }
        public void OnDestroy()
        {
            ModComponent.Log.LogInfo($"[{nameof(WindowPainter)}].{nameof(OnDestroy)}()");
        }
        public Texture2D tintTexture(Texture2D source, Color tint)
        {
            Texture2D newTex = new Texture2D(source.width, source.height);
            Color[] cols = source.GetPixels();
            for (int i = 0; i < cols.Length; ++i)
            {
                cols[i] = Color.Lerp(cols[i], tint, 0.33f);
            }
            newTex.SetPixels(cols);
            return newTex;
        }
        public Texture2D removeTrim(Texture2D source)
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

        public Texture2D addTrim(Texture2D source, Color rimColor)
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
        }
        public void tintSprite(Sprite source, Texture2D tex, Color tint)
        {
            Texture2D texture = source.texture;
            texture = tintTexture(tex,tint);
        }
        public static Texture2D ReadTextureFromFile(String fullPath)
        {
            Byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            if (!ImageConversion.LoadImage(texture, bytes))
                throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");
            return texture;
        }

        public bool ProccessColorChanges()
        {
            try
            {
                List<Object> sprites = new List<Object>(Resources.FindObjectsOfTypeAll(Il2CppType.Of<Sprite>()));
                List<Object> windowFrame = sprites.FindAll(x => textureList.Contains(x.name));
                foreach (Object spr in windowFrame)
                {
                    //ModComponent.Log.LogInfo(spr.Pointer);
                    Sprite sprit = Object.FindObjectFromInstanceID(spr.GetInstanceID()).Cast<Sprite>();
                    //ModComponent.Log.LogInfo(sprit.Pointer);
                    tintSprite(sprit, ReadTextureFromFile(_filePath + sprit.name + ".png"), new Color(96, 77, 177));
                }
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
                    //go ahead and search for our non-addressables

                    Assembly thisone = Assembly.GetExecutingAssembly();
                    _filePath = Path.GetDirectoryName(thisone.Location) + "/ColoredWindows/";
                    
                    ModComponent.Log.LogInfo($"Waiting for window loading.");
                }
                Boolean isPressed = InputManager.GetKeyUp(KeyCode.F6);//todo:make this configurable
                if (isPressed)
                {
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
