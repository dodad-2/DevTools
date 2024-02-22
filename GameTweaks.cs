using DevTools;
using Il2Cpp;
using MelonLoader;
using QList.OptionTypes;
using UnityEngine;

namespace DevTools.GameTweaks;

public static class Main
{
    #region Variables
    // AutoRotate
    public static bool AutoRotate;
    public static bool ValidPlacement;
    internal static Il2Cpp.StrategyMode? StrategyModeInstance;

    // ReBuild
    internal static Il2CppSilica.UI.StructureProductionUI? LastProduction;
    private static KeybindOption? structureKeybind;
    private static KeybindOption? autoRotateKeybind;
    #endregion

    #region Options
    public static void CreateOptions()
    {
        var category = MelonPreferences.CreateCategory("Dodad's Tweaks");
        category.SetFilePath(PreferencesConfig.filePath);

        var structurePreference = MelonPreferences.CreateEntry<string>(
            category.Identifier,
            "REP_STRUC",
            "",
            "Keybind",
            "Build last structure"
        );

        structureKeybind = new KeybindOption(structurePreference);
        structureKeybind.OnKeybindDown += ReProduce;

        QList.Options.AddOption(structureKeybind);

        //

        var autoRotatePreference = MelonPreferences.CreateEntry<string>(
            category.Identifier,
            "REP_AUTOR",
            "",
            "Keybind",
            "Auto rotate construction site"
        );

        autoRotateKeybind = new KeybindOption(autoRotatePreference);
        autoRotateKeybind.OnKeybindDown += EnableAutoRotate;
        autoRotateKeybind.OnKeybindUp += DisableAutoRotate;

        QList.Options.AddOption(autoRotateKeybind);

        category.SaveToFile();
    }
    #endregion

    #region ReProduce
    private static void ReProduce(KeybindOption option)
    {
        if (
            LastProduction == null
            || Player.CurrentPlayer == null
            || !Player.CurrentPlayer.IsCommander
        )
            return;

        LastProduction.Produce();
    }
    #endregion

    #region Auto Rotate
    internal static void StrategyMode_OnEnable(QList.UI.OnEnableNotifier notifier)
    {
        StrategyModeInstance = notifier.GetComponent<Il2Cpp.StrategyMode>();
    }

    internal static void AutoRotateUpdate()
    {
        if (
            !AutoRotate
            || StrategyModeInstance == null
            || StrategyModeInstance.CurrentStructurePlacementData == null
            || StrategyModeInstance.CurrentStructurePlacementData.ObjectPreview == null
            || ValidPlacement
            || Mathf.FloorToInt(
                Mathf.Abs(
                    StrategyModeInstance
                        .CurrentStructurePlacementData
                        .ObjectPreview
                        .transform
                        .rotation
                        .eulerAngles
                        .y
                )
            ) % 90
                != 0
        )
            return;

        StrategyModeInstance.CurrentStructurePlacementRotation =
            StrategyModeInstance.CurrentStructurePlacementRotation == 3
                ? 0
                : StrategyModeInstance.CurrentStructurePlacementRotation + 1;
    }

    private static void EnableAutoRotate(KeybindOption option)
    {
        if (Player.CurrentPlayer == null || !Player.CurrentPlayer.IsCommander)
            return;

        AutoRotate = true;
    }

    private static void DisableAutoRotate(KeybindOption option)
    {
        AutoRotate = false;
    }
    #endregion
}
