using System;
using UnityEngine;

namespace IronExiles.Networking
{
    public static class NetworkCommandLineArgs
    {
        const string ConnectAddressArg = "-connectAddress";
        const string ConnectPortArg = "-connectPort";
        const string ServerPortArg = "-serverPort";

        public const string DefaultAddress = "127.0.0.1";
        public const ushort DefaultPort = 7878;

        public static string GetConnectAddress()
        {
            return GetArgValue(ConnectAddressArg, DefaultAddress);
        }

        public static ushort GetConnectPort()
        {
            var value = GetArgValue(ConnectPortArg, DefaultPort.ToString());
            return ushort.TryParse(value, out var port) ? port : DefaultPort;
        }

        public static ushort GetServerPort()
        {
            var value = GetArgValue(ServerPortArg, DefaultPort.ToString());
            return ushort.TryParse(value, out var port) ? port : DefaultPort;
        }

        static string GetArgValue(string argName, string defaultValue)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], argName, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return defaultValue;
        }
    }
}
