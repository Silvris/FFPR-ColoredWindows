using Last.Management;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFPR_ColoredWindows.IL2CPP;
using UnityEngine;

namespace FFPR_ColoredWindows.Main
{
    public sealed class WindowPainter : MonoBehaviour
    {

        public ResourceManager _resourceManager;
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
        public Texture2D tintCursor(Color tint)
        {
            Texture2D cursor = _resourceManager.completeAssetDic["Assets/GameAssets/Common/UI/Common/Sprites/UI_Common_Cursor01"].Cast<Texture2D>();
            return tintTexture(cursor, tint);
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
                if (_windowLoaded == false)
                {
                    if (!_resourceManager.CheckLoadAssetCompleted("Assets/GameAssets/Common/UI/Common/Sprites/UI_Common_Cursor01"))
                    {
                        return;
                    }
                    _resourceManager.completeAssetDic["Assets/GameAssets/Common/UI/Common/Sprites/UI_Common_Cursor01"] = tintCursor(new Color(160, 40, 240));
                    _windowLoaded = true;
                }
            }
            catch(Exception ex)
            {
                ModComponent.Log.LogError($"[{nameof(WindowPainter)}.{nameof(Update)}]: {ex}");
            }
        }
    }
}
