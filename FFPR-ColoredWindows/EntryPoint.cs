﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using FFPR_ColoredWindows.IL2CPP;
using HarmonyLib;
using System;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace FFPR_ColoredWindows
{

    [BepInPlugin("silvris.ffpr.colored_windows", "Colored Windows", "2.6.0.0")]
    [BepInDependency("ffpr.Syldra")]
    [BepInProcess("FINAL FANTASY.exe")]
    [BepInProcess("FINAL FANTASY II.exe")]
    [BepInProcess("FINAL FANTASY III.exe")]
    [BepInProcess("FINAL FANTASY IV.exe")]
    [BepInProcess("FINAL FANTASY V.exe")]
    [BepInProcess("FINAL FANTASY VI.exe")]
    public class EntryPoint : BasePlugin
    {
        public static EntryPoint Instance { get; private set; }
        public override void Load()
        {
            Log.LogInfo("Loading...");
            Instance = this;
            ClassInjector.RegisterTypeInIl2Cpp<ModComponent>();
            ClassInjector.RegisterTypeInIl2Cpp<ResourceManager_IsLoadAssetCompleted>();
            String name = typeof(ModComponent).FullName;
            Log.LogInfo((object)$"Initializing in-game singleton: {name}");
            GameObject singleton = new GameObject(name);
            singleton.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(singleton);
            Log.LogInfo((object)"Adding ModComponent to singleton...");
            ModComponent component = singleton.AddComponent<ModComponent>();
            if (component is null)
            {
                GameObject.Destroy(singleton);
                throw new Exception($"The object is missing the required component: {name}");
            }
            PatchMethods();
            ModComponent.Log.LogInfo("Plugin initialized!");
        }
        private void PatchMethods()
        {
            try
            {
                Log.LogInfo("Patching methods...");
                Harmony harmony = new Harmony("silvris.ffpr.colored_windows");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to patch methods.", ex);
            }
        }
    }
}
