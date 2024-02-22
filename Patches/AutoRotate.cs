using HarmonyLib;
using UnityEngine;

namespace DevTools.Patches;

[HarmonyPatch("Il2Cpp.StrategyMode", "Awake")]
internal static class StrategyMode_Patch_Il2Cpp_StrategyMode_Awake
{
    public static void Prefix(Il2Cpp.StrategyMode __instance)
    {
        GameTweaks.Main.StrategyModeInstance = __instance;
        //if (__instance.gameObject.GetComponent<QList.UI.OnEnableNotifier>() == null)
        //{
        //    var notifier = __instance.gameObject.AddComponent<QList.UI.OnEnableNotifier>();
        //    notifier.OnEnabled += GameTweaks.Main.StrategyMode_OnEnable;
        //}
        //Log.LogOutput($"StrategyMode.Awake");
    }
}

[HarmonyPatch("Il2Cpp.ConstructionPreview", "SetColor")]
internal static class StrategyMode_Patch_Il2Cpp_ConstructionPreview_SetColor
{
    public static void Prefix(Color color)
    {
        GameTweaks.Main.ValidPlacement = color.b > color.r;
    }
}
