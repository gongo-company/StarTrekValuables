using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using System.IO.Compression;
using StarTrekValuables;
using System.Linq;
using UnityEditor.UIElements;

public class UtilsWindow : EditorWindow
{
    private static string GaleExePath = @"C:\Program Files\Gale\gale.exe";
    private static string GaleProfileName = "Mod Dev";
    private static string GaleGame = "repo";
    // Paths relative to the /Assets/ root
    private static List<string> includeFiles = new List<string>()
    {
        "../Library/ScriptAssemblies/StarTrekValuables.dll",
        "Scripts/ThunderstoreIO/CHANGELOG.md",
        "Scripts/ThunderstoreIO/icon.png",
        "Scripts/ThunderstoreIO/manifest.json",
        "Scripts/ThunderstoreIO/README.md"
    };

    internal struct BuildOption
    {
        public string Label;
        public string Tooltip;
        public bool Value;
        public Action Action;

        public BuildOption(string label, bool value, Action action=null, string tooltip = "")
        {
            Label = label;
            Tooltip = tooltip;
            Value = value;
            Action = action;
        }
    }

    private static bool buildAssetBundles = true;
    private static bool installMod = true;
    private static bool launchGame = false;

    private TextField galePathField;

    [MenuItem("Tools/Mod Utilities")]
    public static void ShowWindow()
    {
        GetWindow<UtilsWindow>("Mod Utilities");
    }

    private void CreateGUI()
    {
        rootVisualElement.Add(new Label("Paths"));
        var topRow = new VisualElement();
        topRow.style.flexDirection = FlexDirection.Row;

        galePathField = new TextField("Gale exe path")
        {
            style = { flexGrow = 1 },
            value = GaleExePath,
        };
        galePathField.RegisterValueChangedCallback((evt) => GaleExePath = evt.newValue);
        topRow.Add(galePathField);

        var browseButton = new UnityEngine.UIElements.Button(() => GaleExePath = EditorUtility.OpenFilePanel("Select Gale.exe", @"C:\Program Files\Gale\", "exe")) { text = "Browse" };
        topRow.Add(browseButton);
        rootVisualElement.Add(topRow);


        var secondRow = new VisualElement();
        secondRow.style.flexDirection = FlexDirection.Row;

        var galeGameField = new TextField("Game") { 
            value = GaleGame,
            tooltip = "Set this to the lowercase name of the game as seen in the Thunderstore URL.",
        };
        galeGameField.RegisterValueChangedCallback((evt) => GaleGame = evt.newValue.Trim().ToLower());
        secondRow.Add(galeGameField);

        secondRow.Add(new VisualElement() { style = { width = 50f } });

        var galeProfileField = new TextField("Gale profile name")
        {
            value = GaleProfileName,
            style = { flexGrow = 1 },
        };
        galeProfileField.RegisterValueChangedCallback((evt) => GaleProfileName = evt.newValue.Trim());
        secondRow.Add(galeProfileField);

        rootVisualElement.Add(secondRow);
        rootVisualElement.Add(new VisualElement() { style = { height = 8f } });


        var modDetailsRow = new VisualElement();
        modDetailsRow.style.flexDirection = FlexDirection.Row;

        var modNameField = new TextField()
        {
            value = MyPluginInfo.PLUGIN_NAME,
            tooltip = "Mod name",
            isReadOnly = true
        }; 
        modDetailsRow.Add(modNameField);
        var modGUIDField = new TextField()
        {
            value = MyPluginInfo.PLUGIN_GUID,
            tooltip = "Mod GUID (must be unique)",
            isReadOnly = true
        };
        modDetailsRow.Add(modGUIDField);
        var versionField = new TextField()
        {
            value = MyPluginInfo.PLUGIN_VERSION,
            tooltip = "Mod version. Change this in MyPluginInfo.cs",
            isReadOnly = true
        };
        modDetailsRow.Add(versionField);

        rootVisualElement.Add(modDetailsRow);
        rootVisualElement.Add(new VisualElement() { style = { height = 8f } });


        var buildButton = new Button();
        buildButton.text = "Build";
        buildButton.clicked += Build;
        rootVisualElement.Add(buildButton);

        var openBuildFolderButton = new Button();
        openBuildFolderButton.text = "Show built zip in Explorer";
        openBuildFolderButton.clicked += () => { EditorUtility.RevealInFinder(Path.Combine(Application.dataPath, @"..\ModBuilds", $"{MyPluginInfo.PLUGIN_GUID}-{MyPluginInfo.PLUGIN_VERSION}.zip")); };
        rootVisualElement.Add(openBuildFolderButton);

        var playButton = new Button();
        playButton.text = "Launch (without building)";
        playButton.clicked += LaunchGame;
        rootVisualElement.Add(playButton);

        rootVisualElement.Add(new VisualElement() { style = { height = 16f } });


        var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);

        rootVisualElement.Add(splitView);

        // LEFT PANE
        var leftPane = new ScrollView();
        splitView.Add(leftPane);
        leftPane.Add(new Label("Files to Include"));
        leftPane.Add(new VisualElement() { style = { height = 10f } });

        CreateFilesList(leftPane);


        // RIGHT PANE
        var rightPane = new ScrollView();
        splitView.Add(rightPane);
        rightPane.Add(new Label("Build Options"));
        rightPane.Add(new VisualElement() { style = { height = 10f } });

        CreateBuildOptions(rightPane);
    }

    private static void CreateFilesList(VisualElement rootVisualElement)
    {
        var container = rootVisualElement;

        foreach (string file in includeFiles)
        {
            var label = new Label();
            string relFile = Path.Combine(Application.dataPath, file);
            label.text = Path.GetFileName(relFile);
            container.Add(label);
        }
    }

    private static void CreateBuildOptions(VisualElement rootVisualElement)
    {
        var container = rootVisualElement;

        var buildToggle = new Toggle() { value = buildAssetBundles };
        var installToggle = new Toggle() { value = installMod };
        var launchToggle = new Toggle() { value = launchGame };

        buildToggle.text = "Build AssetBundle(s)";
        buildToggle.RegisterValueChangedCallback(evt => buildAssetBundles = evt.newValue);
        buildToggle.tooltip = "Build all the assets into the assetbundle before performing installation. Makes the build process take longer, uncheck if you're only changing code.";
        container.Add(buildToggle);

        installToggle.text = "Install mod through Gale";
        installToggle.RegisterValueChangedCallback(evt => installMod = evt.newValue);
        installToggle.tooltip = "Copy files to a build folder, zip it up, and install it as a local mod through Gale.";
        container.Add(installToggle);

        launchToggle.text = "Launch game when build complete";
        installToggle.RegisterValueChangedCallback(evt => launchToggle.SetEnabled(installMod));
        launchToggle.RegisterValueChangedCallback(evt => launchGame = evt.newValue);
        launchToggle.tooltip = "Launch the game through Gale after installing the mod.";
        container.Add(launchToggle);
    }

    private void Build()
    {
        Debug.Log("Beginning build...");

        string manifestJSON = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts/ThunderstoreIO/manifest.json"));
        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(manifestJSON);
        json["version_number"] = MyPluginInfo.PLUGIN_VERSION;
        var output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(Path.Combine(Application.dataPath, "Scripts/ThunderstoreIO/manifest.json"), output);

        if (buildAssetBundles)
        {
            BuildAssetBundles();
        }
        if (installMod)
        {
            ZipAndInstallMod(launchGame ? " --launch" : null);
        }
    }

    public static void BuildAssetBundles()
    {
        string buildFolder = Path.Combine(Application.dataPath, @"..\ModBuilds", $"StarTrekValuables-{MyPluginInfo.PLUGIN_VERSION}");
        Debug.Log($"Building AssetBundles to {buildFolder}");
        if (!Directory.Exists(buildFolder))
        {
            Directory.CreateDirectory(buildFolder);
        }
        var manifest = BuildPipeline.BuildAssetBundles(buildFolder, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
        if (manifest == null)
        {
            Debug.LogError("Failed building asset bundles.");
            return;
        }
        else
        {
            string[] builtBundles = manifest.GetAllAssetBundles();
            Debug.Log($"Built AssetBundle{(builtBundles.Length == 1 ? "" : "s")}: {string.Join(", ", builtBundles)}.");
        }
    }

    public static string CreateZippedMod()
    {
        string buildFolder = Path.Combine(Application.dataPath, @"..\ModBuilds", $"StarTrekValuables-{MyPluginInfo.PLUGIN_VERSION}");
        Debug.Log($"Copying mod files to {buildFolder}");
        CopyFiles(includeFiles, buildFolder);

        string zipFileName = $"StarTrekValuables-{MyPluginInfo.PLUGIN_VERSION}.zip";

        string zipFilePath = Path.Combine(Application.dataPath, @"..\ModBuilds", zipFileName);

        if (File.Exists(zipFilePath))
        {
            Debug.Log($"Zip file \"{zipFilePath}\" already exists, deleting.");
            File.Delete(zipFilePath);
        }

        Debug.Log($"Zipping build folder \"{buildFolder}\" to \"{zipFilePath}\"");
        ZipFile.CreateFromDirectory(buildFolder, zipFilePath);

        return zipFilePath;
    }

    public static void ZipAndInstallMod(string extraLaunchOptions = null)
    {
        var zippedModPath = CreateZippedMod();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = GaleExePath;
        startInfo.Arguments = $"--game {GaleGame} --profile \"{GaleProfileName}\" --install \"{zippedModPath}\" --no-gui";
        if (extraLaunchOptions != null)
        {
            startInfo.Arguments += $" {extraLaunchOptions}";
        }

        try
        {
            var existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GaleExePath));
            if (existingProcesses.Length > 0)
            {
                foreach (var existingProcess in existingProcesses)
                {
                    Debug.LogWarning("Gale is already running, killing it.");
                    existingProcess.Kill();
                }
            }
            using (Process galeProcess = Process.Start(startInfo))
            {
                galeProcess.WaitForExit();
            }
        }
        catch
        {
            Debug.LogError("Error installing mod to Gale");
        }
    }

    public static void LaunchGame()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = GaleExePath;
        startInfo.Arguments = $"--game {GaleGame} --profile \"{GaleProfileName}\" --no-gui --launch";

        try
        {
            var existingProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GaleExePath));
            if (existingProcesses.Length > 0)
            {
                foreach (var existingProcess in existingProcesses)
                {
                    Debug.LogWarning("Gale is already running, killing it.");
                    existingProcess.Kill();
                }
            }
            using (Process galeProcess = Process.Start(startInfo))
            {
                galeProcess.WaitForExit();
            }
        }
        catch
        {
            Debug.LogError("Error launching game with Gale");
        }
    }

    public static void CopyFiles(string file, string destination)
    {
        CopyFiles(new List<string>() { file }, destination);
    }

    public static void CopyFiles(List<string> files, string destination)
    {
        try
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (string file in files)
            {
                string relFile = Path.Combine(Application.dataPath, file);
                string sourcePath = Path.GetDirectoryName(relFile);
                string destinationPath = Path.Combine(destination, Path.GetFileName(relFile));
                File.Copy(relFile, destinationPath, true);
                Debug.Log($"Copied {relFile} to {destinationPath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during post-compile file copy: {e.Message}");
        }
    }
}
