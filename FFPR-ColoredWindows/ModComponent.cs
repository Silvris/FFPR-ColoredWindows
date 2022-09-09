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
                Log.LogMessage((object)$"[{nameof(ModComponent)}].{nameof(Awake)}: Processed successfully.");
            }
            catch(Exception ex)
            {
                _isDisabled = true;
                Log.LogError((object)$"[{nameof(ModComponent)}].{nameof(Awake)}(): {ex}");
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
                Log.LogError((object)$"[{nameof(ModComponent)}].{nameof(Update)}(): {ex}");
                throw;
            }

        }
    }
}
