---
tags:
  - modding/REPO
  - dev
---
The [[REPOLib]] modding SDK is a set of [[Unity]] packages and a Unity project that help unify and automate the process of developing and building mods for REPO. It automatically rips assets and scripts from the vanilla game and allows you to play in editor. It supports more than one mod in the same project.
### Links
- [REPOLib](https://github.com/ZehsTeam/REPOLib)
- [REPOLib SDK](https://github.com/ZehsTeam/REPOLib-Sdk)
- [Unity REPO Project Patcher](https://github.com/Kesomannen/unity-repo-project-patcher)
- [Unity Project Patcher BepInEx](https://github.com/nomnomab/unity-project-patcher-bepinex)

## Instructions
### Basic setup
1. Install Unity `2022.3.21f1` (same as the game)
2. Install [.NET 9](https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=9.0.0&arch=x64&rid=win-x64&os=win10)
3. Create a new project using **3D Built-in render pipeline**
### Install packages
Follow [installation instructions](https://github.com/Kesomannen/unity-repo-project-patcher?tab=readme-ov-file#installation) 
1. In Unity, `Window > Package Manager`, add using + button in top left, git URL: `https://github.com/nomnomab/unity-project-patcher.git`
2. Repeat with `https://github.com/Kesomannen/unity-repo-project-patcher.git`
3. Repeat with `https://github.com/ZehsTeam/REPOLib-Sdk.git`
4. In Unity, `Tools > Unity Project Patcher > Open Window`, then "Install BepInEx"
5. In Unity, `Tools > Unity Project Patcher > Configs > UPPatcherUserSettings` and set the paths in inspector
> [!important] 
> Everything before this should be skippable as it can be included in a repository
### Rip game assets
You will use Unity REPO Project Patcher to rip the game's assets into your unity project using [[AssetRipper]] (this is why you needed to install .NET 9). 
1. In Unity, `Tools > Unity Project Patcher > Open Window`
2. "Run Patcher" and wait/follow its instructions
### Add external precompiled assemblies
The mod's code needs to know about code from other places like the game or REPOLib. You need to find the following files:
- From the game's folder (`...\steamapps\common\REPO\REPO_Data\Managed\`):
	- `Assembly-CSharp.dll`
- From [REPOLib](https://thunderstore.io/c/repo/p/Zehs/REPOLib/):
	- `REPOLib.dll`
- From [BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/):
	- `0Harmony.dll`
	- `BepInEx.dll`
...and copy all of them into `Assets/Plugins/`. 

Then in Unity, find `Assets/Plugins/Assembly-CSharp.dll` that you just imported and in the inspector disable "Auto Reference" and "Validate References" _(otherwise the game's code will exist in two places which causes problems)_.

> [!important] Updates
> Note that you will have to copy these files again in the future when the game updates or when [[REPOLib]] updates. In the future I might set up a thing to assist with that.

### Add REPOLib to project
1. Grab `REPOLib.dll` from [Thunderstore](https://thunderstore.io/c/repo/p/Zehs/REPOLib/) or somewhere else _(like building it yourself)_
2. Put it in `Assets/Plugins/` 
### Tips to run in Unity Editor
> [!note] If you get "No cameras rendering" warning
> Right click the "Game" tab and uncheck the warning option there.

> [!note] Shaders
> `Tools > R.E.P.O. Project Patcher > Replace Shaders`
### Adding things to the mod
Follow [instructions on this page](Adds various Star Trek (And other) valuables using REPOLib.)
tl;dr: Right click and `Create > REPOLib > Valuable` (or `Item`) and assign the values necessary.
Any asset of a type that derives from `Content` (`Valuable`, `Item`, etc.) will be automatically detected and bundled in the mod and loaded.
### Build/export mod to the game
1. Open `Window > REPOLib Exporter`
2. Choose the mod asset for the mod you want to build
3. 