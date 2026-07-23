using UnityEngine;
using PrettyLogSystem;

/// <summary>
/// Small example component showing how to use `PrettyLog` from a scene.
/// Attach to any GameObject and press Play.
/// </summary>
public class PrettyLogSampleUsage : MonoBehaviour
{
    private const string SCRIPT_CHANNEL = "Gameplay";
    private const string SCRIPT_CHANNEL_COLOR = "#5af9e9";

    void Start()
    {
        Debug.Log("This is a regular log message, without any styling.");
        
        // Quick logging with default styling
        PrettyQuickLog.Log("Hello world", Color.cyan, isBold: true);
        PrettyQuickLog.LogWarning("Be careful", Color.yellow);
        PrettyQuickLog.LogError("Oops", Color.red, isBold: true);

        // Register a channel (recommended at startup)
        PrettyLog.RegisterChannel(SCRIPT_CHANNEL, SCRIPT_CHANNEL_COLOR);

        // Register a sub-channel with custom styling
        PrettyLog.RegisterSubChannel(SCRIPT_CHANNEL, "Input", "#7DBA84", isBold: true, fontSize: 14);
        PrettyLog.RegisterSubChannel(SCRIPT_CHANNEL, "Networking", "#795dd6");

        // Log channel-level
        PrettyLog.Log(SCRIPT_CHANNEL, "Player moved");

        // Log sub-channel-level
        PrettyLog.Log(SCRIPT_CHANNEL, "Input", "Button pressed");
        PrettyLog.LogWarning(SCRIPT_CHANNEL, "Networking", "Suspicious velocity");
        PrettyLog.LogError(SCRIPT_CHANNEL, "Networking", "Lost connection");

        // Personalized log with custom styling
        PrettyLog.Log(SCRIPT_CHANNEL, $"Player health: {"75".Bold().Color("#ff00ff")}");

        // --- VERBOSITY DEMO ---
        // Register a channel with a Warning limit (Info, Verbose, and Debug will be ignored)
        PrettyLog.RegisterChannel("Physics", "#FFA500", isBold: true, LogVerbosity.Warning);

        // This will be logged (Error <= Warning threshold)
        PrettyLog.LogError("Physics", "Gravity failure!");
        // This will be ignored (Info > Warning threshold)
        PrettyLog.Log("Physics", "Colliders initialised.");

        // Register a sub-channel with a Verbose limit override (independent of main channel's Warning limit)
        PrettyLog.RegisterSubChannel("Physics", "Collisions", "#FFA500", isBold: false, fontSize: 12, LogVerbosity.Verbose);

        // This will be logged (Verbose <= sub-channel's Verbose threshold)
        PrettyLog.Log("Physics", "Collisions", "A collision occurred.", LogVerbosity.Verbose);
        // This will be ignored (Debug > sub-channel's Verbose threshold)
        PrettyLog.Log("Physics", "Collisions", "Deep contact solver pass.", LogVerbosity.Debug);
    }
}
