using BepInEx.Logging;
using System;
using UnityEngine;
using FFPR_ColoredWindows.Main;
using Exception = System.Exception;
using IntPtr = System.IntPtr;
using Logger = BepInEx.Logging.Logger;
using System.Linq;
using System.Collections.Generic;
using Last.Management;

namespace FFPR_ColoredWindows.IL2CPP
{
    public sealed class ModComponent : MonoBehaviour
    {
        public static ModComponent Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }
        public static Configuration Config { get; private set; }
        [field: NonSerialized]public WindowPainter Painter { get; private set; }
        private Boolean _isDisabled;
        public ModComponent(IntPtr ptr) : base(ptr)
        {
        }
        public void Awake()
        {
            Log = Logger.CreateLogSource("FFPR_ColoredWindows");
            try
            {
                Instance = this;
                Config = new Configuration();
                Painter = new WindowPainter();
                Log.LogMessage($"[{nameof(ModComponent)}].{nameof(Awake)}: Processed successfully.");
            }
            catch(Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(Awake)}(): {ex}");
                throw;
            }
            
        }
        public void Update()
        {
            try
            {
                if (_isDisabled)
                {
                    return;
                }
                Painter.Update();
            }
            catch(Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(Update)}(): {ex}");
                throw;
            }

        }
        public static List<GameObject> GetAllChildren(GameObject obj)
        {
            List<GameObject> children = new List<GameObject>();

            if (obj != null)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    if (child != null)
                    {
                        if (child.gameObject != null)
                        {
                            children.Add(child.gameObject);
                            if (child.childCount != 0)
                            {
                                children.AddRange(GetAllChildren(child.gameObject));
                            }
                        }
                    }


                }
            }
            else
            {
                Log.LogWarning("Root object is null!");
            }

            return children;
        }
    }
}
