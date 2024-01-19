using QList.OptionTypes;
using DevTools;
using MelonLoader;
using Il2Cpp;
using UnityEngine;

namespace DevTools.RebindCheats;

public static class Rebind
{
    public static bool AddTime;
    public static bool SubtractTime;
    public static FloatOption? TimeScaleOption;
    private static KeybindOption? playPauseKeybind;
    private static KeybindOption? addTimeKeybind;
    private static KeybindOption? subtractTimeKeybind;
    private static bool keybindPaused;
    private static float previousTimeScale;

    public static StringOption? spawnUnitString;
    private static KeybindOption? spawnUnitKeybind;

    public static void CreateOptions()
    {
        var category = MelonPreferences.CreateCategory("Cheat Rebind Fix");
        category.SetFilePath(PreferencesConfig.filePath);

        playPauseKeybind = new QList.OptionTypes.KeybindOption(MelonPreferences.CreateEntry<string>(category.Identifier, "PLAYPAUSE", "", "Play / Pause Time"));
        playPauseKeybind.OnKeybindDown += new Action<KeybindOption>(ToggleTime);
        QList.Options.AddOption(playPauseKeybind);

        TimeScaleOption = new FloatOption(MelonPreferences.CreateEntry<float>(category.Identifier, "TIMESCALE", 0.5f, "Add / Subtract Time Speed"), true, 0.5f, 0.01f, 4f);
        QList.Options.AddOption(TimeScaleOption);

        addTimeKeybind = new QList.OptionTypes.KeybindOption(MelonPreferences.CreateEntry<string>(category.Identifier, "ADDTIME", "", "Add Time"));
        addTimeKeybind.OnKeybindDown += new Action<KeybindOption>(StartAddTime);
        addTimeKeybind.OnKeybindUp += new Action<KeybindOption>(StopAddTime);
        QList.Options.AddOption(addTimeKeybind);

        subtractTimeKeybind = new QList.OptionTypes.KeybindOption(MelonPreferences.CreateEntry<string>(category.Identifier, "SUBTRACTTIME", "", "Subtract Time"));
        subtractTimeKeybind.OnKeybindDown += new Action<KeybindOption>(StartSubtractTime);
        QList.Options.AddOption(subtractTimeKeybind);
        subtractTimeKeybind.OnKeybindUp += new Action<KeybindOption>(StopSubtractTime);

        spawnUnitString = new StringOption();
        QList.Options.AddOption(spawnUnitString, "Spawn Unit Name", null, "Cheat Assists");

        spawnUnitKeybind = new KeybindOption(MelonPreferences.CreateEntry<string>(category.Identifier, "SPAWNUNITKEYBIND", "", "Spawn Unit Keybind"));
        spawnUnitKeybind.OnKeybindDown += new Action<KeybindOption>(SpawnUnit);
        QList.Options.AddOption(spawnUnitKeybind, "Spawn Unit Keybind", null, "Cheat Assists");

        category.SaveToFile();
    }

    public static void ToggleTime(KeybindOption option)
    {
        if (Il2Cpp.GameManager.Instance == null)
            return;

        if (!keybindPaused)
        {
            keybindPaused = true;
            previousTimeScale = Il2Cpp.GameManager.Instance.Time_TargetScale;
        }
        else
            keybindPaused = false;

        Il2Cpp.GameManager.Instance.Time_TargetScale = keybindPaused ? 0 : previousTimeScale;
    }
    public static void StartAddTime(KeybindOption option)
    {
        AddTime = true;
    }
    public static void StopAddTime(KeybindOption option)
    {
        AddTime = false;
    }
    public static void StartSubtractTime(KeybindOption option)
    {
        SubtractTime = true;
    }
    public static void StopSubtractTime(KeybindOption option)
    {
        SubtractTime = false;
    }
    public static void SpawnUnit(KeybindOption option)
    {
        if (spawnUnitString == null)
            return;

        var prefabIndex = GameDatabase.GetSpawnablePrefabIndex(spawnUnitString.GetValue());

        if (prefabIndex < 0)
            return;

        var prefab = GameDatabase.GetSpawnablePrefab(prefabIndex);

        var positionRay = Game.CurrentCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        Vector3 position = new Vector3(0, 2f, 0);

        RaycastHit hitInfo;

        if (UnityEngine.Physics.Raycast(positionRay, out hitInfo, Mathf.Infinity))
        {
            position += hitInfo.point;

            var cross = Vector3.Cross(Game.CurrentCamera.transform.right, Vector3.up);

            Game.SpawnPrefab(prefab, null, Player.CurrentPlayer.Team, position, Quaternion.LookRotation(cross), true, true);
        }
    }
}