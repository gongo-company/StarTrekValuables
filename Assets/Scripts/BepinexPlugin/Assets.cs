using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StarTrekValuables
{
    internal static class Assets
    {
        public static List<GameObject> ValuablesPrefabs { get; private set; } = new();

        public static List<string> ValuablesPrefabNames { get; private set; } = new();

        public static Dictionary<string, Object> NonPrefabAssets { get; private set; } = new();

        public static AssetBundle AssetBundle { get; private set; }

        public static void Load()
        {
            Assets.LoadAssetsFromAssetBundle();
        }

        private static void LoadAssetsFromAssetBundle()
        {
            AssetBundle assetBundle = Assets.LoadAssetBundle("startrekvaluables_assets");
            Assets.AssetBundle = assetBundle;
            if (assetBundle == null)
            {
                return;
            }
            string[] prefabNames = assetBundle.GetAllAssetNames();
            foreach (string n in prefabNames)
            {
                if (n.EndsWith(".prefab"))
                {
                    Plugin.Logger.LogInfo($"Loading prefab \"{n}\" from AssetBundle \"{assetBundle.name}\"");
                    var prefab = Assets.LoadAssetFromAssetBundle<GameObject>(n, assetBundle);
                    ValuablesPrefabs.Add(prefab);
                    ValuablesPrefabNames.Add(prefab.name);
                }
                else
                {
                    string assetName = Path.GetFileNameWithoutExtension(n);
                    Plugin.Logger.LogInfo($"Loading asset \"{assetName}\" (path: \"{n}\") from AssetBundle \"{assetBundle.name}\"");
                    NonPrefabAssets.Add(assetName, Assets.LoadAssetFromAssetBundle<Object>(n, assetBundle));
                }
            }
            Plugin.Logger.LogInfo("Successfully loaded assets from AssetBundle!");
        }

        private static AssetBundle LoadAssetBundle(string fileName)
        {
            try
            {
                string dllFolderPath = Path.GetDirectoryName(Plugin.Instance.Info.Location);
                string assetBundleFilePath = Path.Combine(dllFolderPath, fileName);
                return AssetBundle.LoadFromFile(assetBundleFilePath);
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError($"Failed to load AssetBundle \"{fileName}\". {e}");
            }
            return null;
        }

        public static T LoadAssetFromAssetBundle<T>(string name, AssetBundle assetBundle) where T : Object
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Plugin.Logger.LogError($"Failed to load asset of type \"{typeof(T).Name}\" from AssetBundle. Name is null or whitespace.");
                return default(T);
            }
            if (assetBundle == null)
            {
                Plugin.Logger.LogError($"Failed to load asset of type \"{typeof(T).Name}\" with name \"{name}\" from AssetBundle. AssetBundle is null");
                return default(T);
            }
            T asset = assetBundle.LoadAsset<T>(name);
            if (asset == null)
            {
                Plugin.Logger.LogError($"Failed to load asset of type \"{typeof(T).Name}\" with name \"{name}\" from AssetBundle. No asset found with that type and name.");
                return default(T);
            }
            return asset;
        }
    }
}
