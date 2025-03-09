using UnityEditor;
using UnityEditor.Compilation;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[InitializeOnLoad]
public static class PostCompileActions
{
    static PostCompileActions()
    {
        // Add a function to unity's compilation pipeline 
        //CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationFinished(object obj)
    {
        string destinationFolder = @"C:\Users\darkf\AppData\Roaming\com.kesomannen.gale\repo\profiles\Mod Dev\BepInEx\plugins\StarTrekValuables";

        try
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            string relFile = Path.Combine(Application.dataPath, "../Library/ScriptAssemblies/StarTrekValuables.dll");
            string sourcePath = Path.GetDirectoryName(relFile);
            string destinationPath = Path.Combine(destinationFolder, Path.GetFileName(relFile));
            File.Copy(relFile, destinationPath, true);
            Debug.Log($"Copied {relFile} to {destinationPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during post-compile file copy: {e.Message}");
        }
    }
}
