using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    private static string[] scenes = new string[]
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/GuestMode.unity",
        "Assets/Scenes/DirectorMode.unity",
        "Assets/Scenes/EndingScene.unity"
    };

    [MenuItem("Build/Windows EXE")]
    public static void BuildWindows()
    {
        string buildPath = "Build/Windows/糟糕！是心动鸭！.exe";
        string buildDirectory = Path.GetDirectoryName(buildPath);

        if (!Directory.Exists(buildDirectory))
        {
            Directory.CreateDirectory(buildDirectory);
        }

        BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );

        Debug.Log("Windows build completed!");
    }

    [MenuItem("Build/Windows EXE (Development)")]
    public static void BuildWindowsDevelopment()
    {
        string buildPath = "Build/Windows_Dev/糟糕！是心动鸭！_Dev.exe";
        string buildDirectory = Path.GetDirectoryName(buildPath);

        if (!Directory.Exists(buildDirectory))
        {
            Directory.CreateDirectory(buildDirectory);
        }

        BuildPipeline.BuildPlayer(
            scenes,
            buildPath,
            BuildTarget.StandaloneWindows64,
            BuildOptions.Development | BuildOptions.AllowDebugging
        );

        Debug.Log("Windows development build completed!");
    }
}
