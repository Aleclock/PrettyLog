using UnityEngine;

/// <summary>
/// Facade for pretty logging. Use the static methods to write colored, sized
/// logs to the Unity console. This class is lightweight and contains only
/// convenience overloads; the formatting logic lives in PrettyLogCore.
/// </summary>
public class PrettyLog : MonoBehaviour
{
    private static readonly float DefaultTagSize = 12;
    private static readonly float DefaultMessageSize = 12f;
    private static readonly Color DefaultTagColor;
    private static readonly Color DefaultMessageColor;
    private static readonly Color ErrorColor;
    private static readonly Color WarningColor;

    static PrettyLog()
    {
        DefaultTagColor = PrettyLogCore.TryParseHexOrDefault("#7DBA84", Color.green);
        DefaultMessageColor = Color.white;
        ErrorColor = PrettyLogCore.TryParseHexOrDefault("#FE4A49", Color.red);
        WarningColor = PrettyLogCore.TryParseHexOrDefault("#FED766", Color.yellow);
    }

    public static void Log(string tag, string message, Color colorTag = default)
    {
        if (colorTag == default)
            colorTag = DefaultTagColor;

        PrintFormatted(PrettyLogType.Log, tag, message, colorTag, DefaultMessageColor);
    }

    public static void Log(string tag, string message, string hexColorTag)
    {
        var colorTag = PrettyLogCore.TryParseHexOrDefault(hexColorTag, DefaultTagColor);
        PrintFormatted(PrettyLogType.Log, tag, message, colorTag, DefaultMessageColor);
    }

    public static void LogWarning(string tag, string message, Color colorTag = default)
    {
        if (colorTag == default)
            colorTag = DefaultTagColor;

        PrintFormatted(PrettyLogType.Warning, tag, message, colorTag, WarningColor);
    }

    public static void LogWarning(string tag, string message, string hexColorTag)
    {
        var colorTag = PrettyLogCore.TryParseHexOrDefault(hexColorTag, DefaultTagColor);
        PrintFormatted(PrettyLogType.Warning, tag, message, colorTag, WarningColor);
    }

    public static void LogError(string tag, string message, Color colorTag = default)
    {
        if (colorTag == default)
            colorTag = DefaultTagColor;

        PrintFormatted(PrettyLogType.Error, tag, message, colorTag, ErrorColor);
    }

    public static void LogError(string tag, string message, string hexColorTag)
    {
        var colorTag = PrettyLogCore.TryParseHexOrDefault(hexColorTag, DefaultTagColor);
        PrintFormatted(PrettyLogType.Error, tag, message, colorTag, ErrorColor);
    }

    private static void PrintFormatted(PrettyLogType type, string tag, string message, Color colorTag, Color colorMessage, float tagSize = -1, float messageSize = -1)
    {
        if (tagSize <= 0)
            tagSize = DefaultTagSize;

        if (messageSize <= 0)
            messageSize = DefaultMessageSize;

        var formatted = PrettyLogCore.Format(tag, message, colorTag, colorMessage, tagSize, messageSize);
        
        switch(type)
        {
            case PrettyLogType.Log:
                Debug.Log(formatted);
                break;
            case PrettyLogType.Warning:
                Debug.LogWarning(formatted);
                break;
            case PrettyLogType.Error:
                Debug.LogError(formatted);

                break;
        }
    }
}