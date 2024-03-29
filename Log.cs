using MelonLoader;
using QList;

namespace DevTools;

internal static class Log // TODO rewrite this
{
    internal static LogLevel logLevel = LogLevel.None;
    internal static MelonMod? mod;

    internal static bool SetMod(MelonMod newMod, LogLevel logLevel = LogLevel.None)
    {
        if (newMod == null)
        {
            newMod?.LoggerInstance.Error($"Unable to initialize null mod.");
            return false;
        }

        mod = newMod;

        var category = MelonPreferences.GetCategory("Debug");
        MelonPreferences_Entry? entry = null;

        if (category == null)
        {
            category = MelonPreferences.CreateCategory("Debug");
            category.SetFilePath(PreferencesConfig.filePath);
            entry = category.CreateEntry<int>(
                "LOG_LEVEL",
                7,
                "Log Level",
                "None = 0, Message = 1, Info = 2, Warning = 3, Error = 4, Fatal = 5, Debug = 6, All = 7"
            );
            category.SaveToFile();
        }

        if (entry == null)
            entry = category.GetEntry("LOG_LEVEL");

        var logLevelOption = new QList.OptionTypes.IntOption(entry, true, 0, 0, 7, 1);
        logLevelOption.OnValueChangedUntyped += OnValueUpdatedUntyped;
        Options.AddOption(logLevelOption);

        Log.logLevel = logLevel == LogLevel.None ? (LogLevel)(int)entry.BoxedValue : logLevel;

        return true;
    }

    internal static void LogOutput(object data, LogLevel level = LogLevel.Debug)
    {
        if (level > logLevel || logLevel == LogLevel.None || mod == null)
            return;

        switch (level)
        {
            case LogLevel.Message:
                mod.LoggerInstance.Msg($"{data}");
                break;
            case LogLevel.Info:
                mod.LoggerInstance.Msg($"{data}");
                break;
            case LogLevel.Warning:
                mod.LoggerInstance.Warning($"{data}");
                break;
            case LogLevel.Error:
                mod.LoggerInstance.Error($"{data}");
                break;
            case LogLevel.Fatal:
                mod.LoggerInstance.BigError($"{data}");
                break;
            case LogLevel.Debug:
                mod.LoggerInstance.Msg($"{data}");
                break;
        }
    }

    internal static void OnValueUpdatedUntyped(object oldValue, object newValue)
    {
        logLevel = (LogLevel)Convert.ToInt32(newValue);
    }

    public enum LogLevel
    {
        None = 0,
        Message = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
        Debug = 6,
        All = 7
    }
}
