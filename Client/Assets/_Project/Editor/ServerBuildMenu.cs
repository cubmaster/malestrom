using UnityEditor;
using UnityEngine;

namespace IronExiles.Editor
{
    public static class ServerBuildMenu
    {
        const string WindowsServerPath = "Builds/Server/Windows/IronExilesServer.exe";
        const string LinuxServerPath = "Builds/Server/Linux/IronExilesServer";

        [MenuItem("Iron Exiles/Build Dedicated Server (Windows)")]
        public static void BuildWindowsServer()
        {
            BuildServer(BuildTarget.StandaloneWindows64, WindowsServerPath);
        }

        [MenuItem("Iron Exiles/Build Dedicated Server (Linux)")]
        public static void BuildLinuxServer()
        {
            BuildServer(BuildTarget.StandaloneLinux64, LinuxServerPath);
        }

        static void BuildServer(BuildTarget target, string outputPath)
        {
            var scenes = new[] { "Assets/Scenes/Test/EmptySector.unity" };

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                subtarget = (int)StandaloneBuildSubtarget.Server,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"[ServerBuild] {target} server build succeeded: {outputPath}");
            }
            else
            {
                Debug.LogError($"[ServerBuild] {target} server build failed: {report.summary.totalErrors} errors");
                EditorApplication.Exit(1);
            }
        }
    }
}
