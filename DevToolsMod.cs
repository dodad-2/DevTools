using MelonLoader;
using DevTools;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using QList;
using QList.OptionTypes;

[assembly: MelonInfo(typeof(DevToolsMod), "DevTools", "0.0.1", "dodad")]
[assembly: MelonGame("Bohemia Interactive", "Silica")]
[assembly: MelonOptionalDependencies("QList")]

namespace DevTools;

public class DevToolsMod : MelonMod
{
    private static string[] TeamNames = new string[3]
    {
        "alien",
        "centauri",
        "sol",
    };

    public static DevToolsMod? instance;
    internal static Dictionary<string, BoolOption>? TeamOptions = new();

    private bool QListPresent() => RegisteredMelons.Any(m => m.Info.Name == "QList");

    public override void OnLateInitializeMelon()
    {
        if (!QListPresent())
        {
            LoggerInstance.Error($"QList not found, mod disabled");
            return;
        }

        instance = this;

        PreferencesConfig.SetFilePath(this);

        RegisterQListOptions();

        Log.SetMod(this);

        var generalCategory = MelonPreferences.CreateCategory("General");
        generalCategory.SetFilePath(PreferencesConfig.filePath);

    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void RegisterQListOptions()
    {
        if (TeamNames == null || TeamOptions == null)
            return;

        QList.Options.RegisterMod(this);

        var alienCommanderAI = new QList.OptionTypes.BoolOption(null, false);
        alienCommanderAI.OnValueChangedOption += new Action<QList.OptionTypes.BoolOption>(ToggleAlienCommander);
        var centauriCommanderAI = new QList.OptionTypes.BoolOption(null, false);
        centauriCommanderAI.OnValueChangedOption += new Action<QList.OptionTypes.BoolOption>(ToggleCentauriCommander);
        var solCommanderAI = new QList.OptionTypes.BoolOption(null, false);
        solCommanderAI.OnValueChangedOption += new Action<QList.OptionTypes.BoolOption>(ToggleSolCommander);

        QList.Options.AddOption(alienCommanderAI, "Alien Commander", null, "Commander AI");
        QList.Options.AddOption(centauriCommanderAI, "Centauri Commander", null, "Commander AI");
        QList.Options.AddOption(solCommanderAI, "Sol Commander", null, "Commander AI");

        TeamOptions.Add(Il2Cpp.Team.GetTeamByName(TeamNames[0]).name, alienCommanderAI);
        TeamOptions.Add(Il2Cpp.Team.GetTeamByName(TeamNames[1]).name, centauriCommanderAI);
        TeamOptions.Add(Il2Cpp.Team.GetTeamByName(TeamNames[2]).name, solCommanderAI);
    }
    public static void ToggleAlienCommander(QList.OptionTypes.BoolOption option)
    {
        SetCommanderStateForTeam(TeamNames[0], option.GetValue());
    }
    public static void ToggleSolCommander(QList.OptionTypes.BoolOption option)
    {
        SetCommanderStateForTeam(TeamNames[1], option.GetValue());
    }
    public static void ToggleCentauriCommander(QList.OptionTypes.BoolOption option)
    {
        SetCommanderStateForTeam(TeamNames[2], option.GetValue());
    }
    public static void SetCommanderStateForTeam(string teamName, bool state)
    {
        var team = (Il2Cpp.Team.GetTeamByName(teamName));

        if (team != null)
            SetCommanderStateForTeam(team, state);
    }
    public static void SetCommanderStateForTeam(Il2Cpp.Team team, bool state)
    {
        if (team == null)
        {
            Log.LogOutput($"SetCommanderStateForTeam: Can't set commander state: team is null");
            return;
        }

        Log.LogOutput($"{(state ? "Enabling" : "Disabling")} commander for {team.TeamShortName}");
        Il2CppSilica.AI.AIManager.EnableCommander(team, state);

    }
}
