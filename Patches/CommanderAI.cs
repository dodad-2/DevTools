namespace DevTools.Patches;

using HarmonyLib;

[HarmonyPatch("Il2CppSilica.AI.AIManager", "EnableCommander")]
internal static class AI_Patch_Il2CppSilica_AI_AIManager_EnableCommander
{
    public static void Prefix(Il2Cpp.Team team, ref bool enable)
    {
        if (!Il2Cpp.Game.CheatsEnabled)
            return;

        if (
            enable
            && Il2Cpp.GameMode.CurrentGameMode != null
            && team != null
            && DevToolsMod.TeamOptions != null
            && DevToolsMod.TeamOptions.ContainsKey(team.name)
        )
        {
            var strategy = Il2Cpp.GameMode.CurrentGameMode.Cast<Il2Cpp.MP_Strategy>();

            if (strategy != null && strategy.GetCommanderForTeam(team) == null)
            {
                enable = DevToolsMod.TeamOptions[team.name].GetValue();
                Log.LogOutput(
                    $"Patch_Il2CppSilica_AI_AIManager_EnableCommander_Prefix: Overriding '{team.name}' commander to '{enable}'"
                );
            }
            else
                Log.LogOutput(
                    $"Patch_Il2CppSilica_AI_AIManager_EnableCommander_Prefix: No current gamemode or player has control of team"
                );
        }
        else
            Log.LogOutput(
                $"Patch_Il2CppSilica_AI_AIManager_EnableCommander_Prefix: Ignoring override"
            );
    }
}
