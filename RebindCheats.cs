using QList.OptionTypes;
using DevTools;
using MelonLoader;

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
        subtractTimeKeybind.OnKeybindUp += new Action<KeybindOption>(StopSubtractTime);
        QList.Options.AddOption(subtractTimeKeybind);

        category.SaveToFile();
    }

    public static void ToggleTime(KeybindOption option)
    {
        Log.LogOutput($"Rebind.ToggleTime: {keybindPaused}");

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
}