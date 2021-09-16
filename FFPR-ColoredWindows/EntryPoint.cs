using BepInEx;
using BepInEx.IL2CPP;
using FFPR_ColoredWindows.IL2CPP;
using System;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace FFPR_ColoredWindows
{
    [BepInPlugin("silvris.ffpr.colored_windows", "Colored Windows", "1.0.0.0")]
    [BepInProcess("FINAL FANTASY.exe")]
    [BepInProcess("FINAL FANTASY II.exe")]
    [BepInProcess("FINAL FANTASY III.exe")]
    [BepInProcess("FINAL FANTASY IV.exe")]
    [BepInProcess("FINAL FANTASY V.exe")]
    [BepInProcess("FINAL FANTASY VI.exe")]
    public class EntryPoint : BasePlugin
    {
        public override void Load()
        {
            Log.LogInfo("Loading...");

            //TypeRegister typeRegister = new TypeRegister(Log);
            //typeRegister.RegisterAssemblyTypes();
            ClassInjector.RegisterTypeInIl2Cpp<ModComponent>();
            String name = typeof(ModComponent).FullName;
            Log.LogInfo($"Initializing in-game singleton: {name}");
            GameObject singleton = new GameObject(name);
            singleton.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(singleton);
            Log.LogInfo("Adding ModComponent to singleton...");
            ModComponent component = singleton.AddComponent<ModComponent>();
            if (component is null)
            {
                GameObject.Destroy(singleton);
                throw new Exception($"The object is missing the required component: {name}");
            }
        }
    }
}
