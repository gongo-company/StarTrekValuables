using System.Reflection;
using System;
using BepInEx;
using BepInEx.Logging;
using REPOLib.Modules;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using REPOLib.Commands;
using REPOLib;
using System.IO;

namespace StarTrekValuables
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("REPOLib", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        internal static ManualLogSource Logger { get; private set; }

        internal static Harmony harmony;

        private void Awake()
        {
            if (Plugin.Instance == null)
            {
                Plugin.Instance = this;
            }
            Plugin.Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            Plugin.Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} awake!");

            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID); 
            harmony.PatchAll();

            BundleLoader.LoadBundle(Path.Combine(Path.GetDirectoryName(Plugin.Instance.Info.Location), "StarTrekValuables_assets"), "");
            //Assets.Load();

            ConfigManager.Initialize(Config);

            //this.RegisterValuables();
        }

        //private void RegisterValuables()
        //{
        //    foreach (GameObject valuable in Assets.ValuablesPrefabs)
        //    {
        //        //Plugin.Logger.LogInfo($"Registering \"{valuable.name}\"");
        //        Valuables.RegisterValuable(valuable, ConfigManager.ValuableLevelSpawns[valuable.name]);
        //    }
        //    Plugin.Logger.LogInfo($"Registered {Assets.ValuablesPrefabs.Count} valuables!");
        //}
    }
}