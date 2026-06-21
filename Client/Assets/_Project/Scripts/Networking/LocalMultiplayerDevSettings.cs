using System;
using System.IO;
using UnityEngine;

namespace IronExiles.Networking
{
    public static class LocalMultiplayerDevSettings
    {
        public const string AutoConnectEnvironmentVariable = "IRON_EXILES_AUTO_CONNECT";
        public const string AutoConnectFlagFileName = ".iron-exiles-auto-connect";

        public static bool ShouldAutoConnectInEditor()
        {
#if UNITY_EDITOR
            if (Application.isBatchMode)
            {
                return false;
            }

            var envValue = Environment.GetEnvironmentVariable(AutoConnectEnvironmentVariable);
            if (string.Equals(envValue, "1", StringComparison.Ordinal))
            {
                return true;
            }

            return File.Exists(GetAutoConnectFlagPath());
#else
            return false;
#endif
        }

        public static string GetAutoConnectFlagPath()
        {
            var clientRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (string.IsNullOrEmpty(clientRoot))
            {
                return AutoConnectFlagFileName;
            }

            var repoRoot = Directory.GetParent(clientRoot)?.FullName;
            if (string.IsNullOrEmpty(repoRoot))
            {
                return Path.Combine(clientRoot, AutoConnectFlagFileName);
            }

            return Path.Combine(repoRoot, AutoConnectFlagFileName);
        }
    }
}
