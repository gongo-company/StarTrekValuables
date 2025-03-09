using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace StarTrekValuables
{
    internal static class ConfigManager
    {
        public static ConfigFile ConfigFile { get; private set; }

        public static Dictionary<string, List<string>> ValuableLevelSpawns { get; private set; }
        internal static Dictionary<string, List<string>> _valuableLevelSpawnsDefaults = new Dictionary<string, List<string>>()
        {
            {"Valuable Antimatter Pod", new(){"Valuables - Arctic" } },
            {"Valuable Blue Barrel", new(){"Valuables - Arctic" } },
            {"Valuable Clock", new(){"Valuables - Wizard" } },
            {"Valuable Exocomp", new(){"Valuables - Arctic" } },
            {"Valuable Horgahn", new(){"Valuables - Manor", "Valuables - Wizard" } },
            {"Valuable Metal Pipe", new(){"Valuables - Manor", "Valuables - Arctic" } },
            {"Valuable Mug", new(){"Valuables - Manor", "Valuables - Arctic" } },
            {"Valuable Odos Bucket", new(){"Valuables - Manor", "Valuables - Wizard" } },
            {"Valuable Pyramid Game", new(){"Valuables - Arctic" } },
            {"Valuable Strange Rock", new(){"Valuables - Manor", "Valuables - Arctic" } },
            {"Valuable Widedoc Plush", new(){"Valuables - Manor", "Valuables - Arctic" } },
            {"Valuable Worfs Chair", new(){"Valuables - Manor" } },
        };

        public static void Initialize(ConfigFile configFile)
        {
            ConfigFile = configFile;
            ValuableLevelSpawns = new Dictionary<string, List<string>>();
            BindConfigs();
        }

        private static void BindConfigs()
        {
            foreach (string valuable in Assets.ValuablesPrefabNames)
            {
                List<string> defaultList;
                if (!_valuableLevelSpawnsDefaults.TryGetValue(valuable, out defaultList))
                {
                    defaultList = new() { "Valuables - Generic" };
                }
                ValuableLevelSpawns[valuable] = ConfigFile.Bind("Valuable Spawns", valuable, defaultValue: defaultList != null ? String.Join(",", defaultList) : "Valuables - Generic").Value.Split(",").ToList();
            }
        }
    }
}
