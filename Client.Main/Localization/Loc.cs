using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Client.Main.Localization
{
    /// <summary>
    /// Simple localization manager. Loads embedded JSON resources
    /// and provides a global Get() method for UI strings.
    /// </summary>
    public static class Loc
    {
        private static Dictionary<string, string> _strings = new();
        private static string _currentLang = "zh";

        public static string CurrentLanguage
        {
            get => _currentLang;
            set
            {
                if (_currentLang == value) return;
                _currentLang = value;
                LoadLanguage(value);
                LanguageChanged?.Invoke();
            }
        }

        public static event Action? LanguageChanged;

        public static string Get(string key) =>
            _strings.TryGetValue(key, out var val) ? val : key;

        public static void Initialize(string? preferredLang = null)
        {
            string lang = preferredLang ?? "zh";
            LoadLanguage(lang);
        }

        private static void LoadLanguage(string lang)
        {
            _strings = new Dictionary<string, string>();
            _currentLang = lang;

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = $"Client.Main.Content.lang_{lang}.json";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Loc] Embedded resource not found: {resourceName}");
                    return;
                }

                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (dict != null)
                {
                    _strings = dict;
                    System.Diagnostics.Debug.WriteLine($"[Loc] Loaded {_strings.Count} strings ({lang})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Loc] Failed to load lang '{lang}': {ex.Message}");
            }
        }
    }
}
