using BepInEx;
using BepInEx.IL2CPP;
using System;

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
        }

        void Awake()
        {
            Log.LogInfo("Hello World!");
        }
    }
}
