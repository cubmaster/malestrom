using UnityEditor;
using System.Diagnostics;
using System.IO;

[InitializeOnLoad]
public static class GitHelper
{
    static GitHelper()
    {
        RunGit();
    }

    [MenuItem("Tools/Run Git Helper")]
    public static void RunGit()
    {
        try
        {
            string output = "";
            output += "=== GIT DIFF NetworkPlayerShipFactory.cs ===\n";
            output += RunProcess("git", "diff Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs");

            File.WriteAllText("Assets/_Project/Editor/git_output.txt", output);
            UnityEngine.Debug.Log("GitHelper wrote output to Assets/_Project/Editor/git_output.txt");
        }
        catch (System.Exception e)
        {
            File.WriteAllText("Assets/_Project/Editor/git_output.txt", "Error: " + e.ToString());
        }
    }

    private static string RunProcess(string filename, string args)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = filename,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Directory.GetCurrentDirectory()
        };

        using (Process process = Process.Start(startInfo))
        {
            string outStr = process.StandardOutput.ReadToEnd();
            string errStr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return outStr + "\n" + errStr;
        }
    }
}

