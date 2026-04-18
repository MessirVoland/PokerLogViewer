using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class AppLocalization
{
    private static Dictionary<string, string> _strings = new();

    public static string Language { get; private set; } = "RU";

    public static void Load(string lang)
    {
        Language = lang;

        var file = lang == "EN"
            ? "Localization/Strings.en.json"
            : "Localization/Strings.ru.json";

        var json = File.ReadAllText(file);
        _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }

    public static string Get(string key)
    {
        if (_strings != null && _strings.TryGetValue(key, out var value))
            return value;

        return key;
    }
}