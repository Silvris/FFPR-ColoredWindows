using FFPR_ColoredWindows.IL2CPP;
using HarmonyLib;
using Last.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFPR_ColoredWindows
{
    [HarmonyPatch(typeof(ResourceManager),nameof(ResourceManager.IsLoadAssetCompleted))]
    public sealed class ResourceManager_IsLoadAssetCompleted : Il2CppSystem.Object
    {
        public ResourceManager_IsLoadAssetCompleted(IntPtr ptr) : base(ptr)
        {
        }
        public static void Postfix(string addressName, ResourceManager __instance)
        {
            //I'm abusing the concept of a postfix for this
            ModComponent.Log.LogInfo(addressName);
        }
    }
}
