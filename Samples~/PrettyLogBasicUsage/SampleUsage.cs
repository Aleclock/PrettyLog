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
    }
}
