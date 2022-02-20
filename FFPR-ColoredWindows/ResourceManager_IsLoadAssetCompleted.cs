using FFPR_ColoredWindows.IL2CPP;
using HarmonyLib;
using Last.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FFPR_ColoredWindows
{
    [HarmonyPatch(typeof(ResourceManager),nameof(ResourceManager.IsLoadAssetCompleted))]
    public sealed class ResourceManager_IsLoadAssetCompleted : Il2CppSystem.Object
    {
        public ResourceManager_IsLoadAssetCompleted(IntPtr ptr) : base(ptr)
        {
        }
        public static void Postfix(string addressName, ResourceManager __instance, bool __result)
        {
            //I'm abusing the concept of a postfix for this
            //ModComponent.Log.LogInfo(addressName);
            if(__result == true)
            {
                if (ModComponent.Instance.Painter.targetGobs.Contains(addressName))
                {
                    if (__instance.completeAssetDic.ContainsKey(addressName))//should be true after the top one, but just in case
                    {
                        GameObject gob = __instance.completeAssetDic[addressName].Cast<GameObject>();
                        if (ModComponent.Instance.Painter.knownObjects.ContainsKey(addressName))
                        {
                            if(ModComponent.Instance.Painter.knownObjects[addressName] == gob.GetInstanceID())
                            {
                                return;//if either are false, continue
                            }
                        }
                        List<GameObject> all = ModComponent.GetAllChildren(gob);
                        all.Insert(0, gob);
                        //ModComponent.Log.LogInfo(all.Count);
                        foreach (GameObject go in all)
                        {
                            Image img = go.GetComponent<Image>();
                            if (img != null)
                            {
                                //ModComponent.Log.LogInfo(img.mainTexture.name);
                                if (ModComponent.Instance.Painter.textureList.Contains(img.mainTexture.name))
                                {
                                    ModComponent.Log.LogInfo("Replaced sprite");
                                    ModComponent.Instance.Painter.SetImageSprite(img);
                                }
                            }
                        }
                        ModComponent.Instance.Painter.knownObjects[addressName] = gob.GetInstanceID();
                    }
                }
            }

        }
    }
}
