using UnityEngine;

public class PrettyLog : MonoBehaviour
{
    private static float defaultTagSize = 14f;
    private static float defaultMessageSize = 12f;
    private static readonly Color DefaultMessageColor = Color.white;
    private static readonly Color ErrorColor = ColorUtility.TryParseHtmlString("#FE4A49", out Color color) ? color : Color.red;
    private static readonly Color WarningColor = ColorUtility.TryParseHtmlString("#FED766", out Color color) ? color : Color.yellow;

    #region CORE LOG

    public static void Log(string tag, string message, Color colorTag)
    {
        Log(tag, message, colorTag, DefaultMessageColor);
    }

    public static void Log(string tag, string message, string colorHexTag)
    {
        if (TryParse(colorHexTag, out Color colorTag))
            Log(tag, message, colorTag, DefaultMessageColor);
    }

    public static void Log (string tag, string message, Color colorTag, Color colorMessage)
    {
        print(Format(tag, message, colorTag, colorMessage));
    }

    public static void Log (string tag, string message, string colorHexTag, string colorHexMessage)
    {
        if (TryParse(colorHexTag, out Color colorTag) && TryParse(colorHexMessage, out Color colorMessage))
            print(Format(tag, message, colorTag, colorMessage));
    }

    #endregion

    public static void LogWarning(string tag, string message, Color colorTag, Color colorMessage)
    {
        Debug.LogWarning(Format(tag, message, colorTag, colorMessage));
    }

    public static void LogWarning(string tag, string message, string colorHexTag, string colorHexMessage)
    {
        if (TryParse(colorHexTag, out Color colorTag) && TryParse(colorHexMessage, out Color colorMessage))
        {
            Debug.LogWarning(Format(tag, message, colorTag, colorMessage));
        }
    }

    public static void LogError(string tag, string message, Color colorTag, Color colorMessage)
    {
        Debug.LogError(Format(tag, message, colorTag, colorMessage));
    }

    public static void LogError(string tag, string message, string colorHexTag, string colorHexMessage)
    {
        if (TryParse(colorHexTag, out Color colorTag) && TryParse(colorHexMessage, out Color colorMessage))
        {
            Debug.LogError(Format(tag, message, colorTag, colorMessage));
        }
    }

    #region HELPERS
    private static string Format(string tag, string message, Color colorTag, Color colorMessage)
    {
        string tagColored = GetColoredMessage(tag, colorTag);
        string messageColored = GetColoredMessage(message, colorMessage);

        //print($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{tag}]</color>{debug_test}");


        return $"[{tagColored}] {messageColored}";
    }

    private static string GetColoredMessage(string message, Color color)
    {
        return $"<color=#{ColorToHex(color)}>{message}</color>";
    }

    private static bool TryParse(string hex, out Color color)
    {
        return ColorUtility.TryParseHtmlString(hex, out color);
    }

    private static string ColorToHex(Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }
    
    #endregion
}