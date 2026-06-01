using UnityEngine;

/// <summary>
/// Small example component showing how to use `PrettyLog` from a scene.
/// Attach to any GameObject and press Play.
/// </summary>
public class PrettyLogSampleUsage : MonoBehaviour
{
    void Start()
    {
        Console.log("This is a regular log message without formatting.");
        PrettyLog.Log("PrettyLogSample", "Basic log using default colors");

        // Hex colors
        PrettyLog.Log("PrettyLogSample", "Hex colors", "#7DBA84", "#FFFFFF");

        // Unity Color + explicit sizes
        PrettyLog.Log("UI", "Button clicked", Color.cyan, Color.white, 18f, 14f);

        // Warnings and errors (uses PrettyLog.LogWarning/LogError)
        PrettyLog.LogWarning("PrettyLogSample", "This is a warning", "#FED766", "#000000");
        PrettyLog.LogError("PrettyLogSample", "This is an error", "#FE4A49", "#FFFFFF");
    }
}
