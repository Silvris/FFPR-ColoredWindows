using HarmonyLib;
using Last.UI.KeyInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FFPR_ColoredWindows.IL2CPP
{
    [HarmonyPatch(typeof(SaveContentController), nameof(SaveContentController.SetFocus))]
    class SaveContentView_SetFocus
    {
        public static void Postfix(bool isFocus, SaveContentController __instance)
        {
            SaveContentView view = __instance.view;
            List<Text> texts = new List<Text>();
            List<Shadow> shadows = new List<Shadow>();
            List<GameObject> gobs = ModComponent.GetAllChildren(__instance.gameObject);
            foreach(GameObject gob in gobs)
            {
                Text text = gob.GetComponent<Text>();
                Shadow shadow = gob.GetComponent<Shadow>();
                if(text != null)
                {
                    texts.Add(text);
                }
                if(shadow != null)
                {
                    shadows.Add(shadow);
                }
            }
            foreach(Text t in texts)
            {
                t.color = ModComponent.Config.Window.TextColor;
            }
            foreach(Shadow s in shadows)
            {
                s.effectColor = ModComponent.Config.Window.ShadowColor;
            }
            if (__instance.slotNum == null)
            {
                //this is a quicksave/autosave
                view.SlotNameText.color = ModComponent.Config.Window.SpecialTextColor;
            }
            //now set the time to also use special color
            view.TimeStampDate.color = ModComponent.Config.Window.SpecialTextColor;
            view.TimeStampTime.color = ModComponent.Config.Window.SpecialTextColor;
            view.fixedTimeText.color = ModComponent.Config.Window.SpecialTextColor;
            view.fixedLevelText.color = ModComponent.Config.Window.SpecialTextColor;
            if (isFocus)
            {
                view.SlotNameText.color = ModComponent.Config.Window.FocusTextColor;
                Shadow nameShadow = view.SlotNameText.gameObject.GetComponent<Shadow>();
                if (nameShadow != null)
                {
                    nameShadow.effectColor = ModComponent.Config.Window.FocusShadowColor;
                }
                view.SlotNumText.color = ModComponent.Config.Window.FocusTextColor;
                Shadow NumShadow = view.SlotNumText.gameObject.GetComponent<Shadow>();
                if (NumShadow != null)
                {
                    NumShadow.effectColor = ModComponent.Config.Window.FocusShadowColor;
                }

            }

        }
        [HarmonyPatch(typeof(LoadGameWindowView), nameof(LoadGameWindowView.Initialize))]
        class LoadGameWindowView_Initialize
        {
            public static void Postfix(LoadGameWindowView __instance)
            {
                Text text = __instance.windowNameText;
                Shadow shadow = __instance.windowNameText.gameObject.GetComponent<Shadow>();
                text.color = ModComponent.Config.Window.TextColor;
                if(shadow != null)
                {
                    shadow.effectColor = ModComponent.Config.Window.ShadowColor;
                }
            }
        }
        [HarmonyPatch(typeof(KeyIconView), nameof(KeyIconView.Initialize))]
        class KeyIconView_Initialize
        {
            public static void Postfix(KeyIconView __instance)
            {
                Text text = __instance.KeyText;
                Shadow shadow = __instance.KeyText.gameObject.GetComponent<Shadow>();
                text.color = ModComponent.Config.Window.TextColor;
                if (shadow != null)
                {
                    shadow.effectColor = ModComponent.Config.Window.ShadowColor;
                }
            }
        }
    }
}
