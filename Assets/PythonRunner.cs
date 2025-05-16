using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PythonRunner : MonoBehaviour
{
    private Process pythonProcess;

    void Start()
    {
        RunPythonScript();
    }

    void RunPythonScript()
    {
        string pythonPath = "/usr/bin/python3";  // python 경로 확인 필요 (터미널에서 `which python3`)
        string scriptPath = Path.Combine(Application.dataPath, "PythonScripts/main.py");

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("Python script not found: " + scriptPath);
            return;
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        pythonProcess = new Process { StartInfo = psi };

        pythonProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log("[Python] " + args.Data);
        pythonProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError("[Python-Error] " + args.Data);

        try
        {
            pythonProcess.Start();
            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to start Python script: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }
}
