using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace IronExiles.Editor
{
    public static class ServerBuildMenu
    {
        const string WindowsServerPath = "../Builds/Server/Windows/IronExilesServer.exe";
        const string LinuxServerPath = "../Builds/Server/Linux/IronExilesServer";

        [MenuItem("Iron Exiles/Build Dedicated Server (Windows)")]
        public static void BuildWindowsServer()
        {
            if (!TryBuildServer(BuildTarget.StandaloneWindows64, WindowsServerPath, StandaloneBuildSubtarget.Server))
            {
                Debug.LogWarning("[ServerBuild] Dedicated Server module unavailable; building headless player fallback.");
                if (!TryBuildServer(BuildTarget.StandaloneWindows64, WindowsServerPath, StandaloneBuildSubtarget.Player, BuildOptions.EnableHeadlessMode))
                {
                    EditorApplication.Exit(1);
                }
            }
        }

        [MenuItem("Iron Exiles/Build Dedicated Server (Linux)")]
        public static void BuildLinuxServer()
        {
            if (!TryBuildServer(BuildTarget.StandaloneLinux64, LinuxServerPath, StandaloneBuildSubtarget.Server))
            {
                Debug.LogWarning("[ServerBuild] Dedicated Server module unavailable; building headless player fallback.");
                if (!TryBuildServer(BuildTarget.StandaloneLinux64, LinuxServerPath, StandaloneBuildSubtarget.Player, BuildOptions.EnableHeadlessMode))
                {
                    EditorApplication.Exit(1);
                }
            }
        }

        static bool TryBuildServer(
            BuildTarget target,
            string outputPath,
            StandaloneBuildSubtarget subtarget,
            BuildOptions extraOptions = BuildOptions.None)
        {
            var scenes = new[] { "Assets/Scenes/Test/EmptySector.unity" };

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                subtarget = (int)subtarget,
                options = extraOptions
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[ServerBuild] {target} ({subtarget}) build succeeded: {outputPath}");
                return true;
            }

            var messages = string.Join("; ", report.steps.SelectMany(step => step.messages).Select(message => message.content));
            Debug.LogError($"[ServerBuild] {target} ({subtarget}) build failed: {report.summary.totalErrors} errors. {messages}");
            return false;
        }
    }
}
