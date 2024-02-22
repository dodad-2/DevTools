using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;

namespace DevTools.Patches;

[HarmonyPatch("Il2CppSilica.UI.StructureProductionUI", "Produce")]
internal static class ReProduce_Patch_Il2CppSilica_UI_StructureProductionUI_Produce
{
    public static void Prefix(Il2CppSilica.UI.StructureProductionUI __instance)
    {
        GameTweaks.Main.LastProduction = __instance;
    }
}
