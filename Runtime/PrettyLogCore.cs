using UnityEngine;

/// <summary>
/// Core formatting utilities used by PrettyLog. Separated to keep the public
/// facade small and easy to consume from user code. Internal to avoid
/// unnecessary public surface area.
/// </summary>
internal static class PrettyLogCore
{
    public static string Format(string tag, string message, Color colorTag, Color colorMessage, float tagSize = 12f, float messageSize = 12f)
    {
        var tagColored = GetColoredSizedMessage(tag, colorTag, tagSize);
        var messageColored = GetColoredSizedMessage(message, colorMessage, messageSize);
        return $"[{tagColored}] {messageColored}";
    }

    private static string GetColoredSizedMessage(string text, Color color, float size)
    {
        var hex = ColorToHex(color);
        var colored = $"<color=#{hex}>{text}</color>";
        if (size > 0f)
            return $"<size={(int)size}>{colored}</size>";
        return colored;
    }

    public static bool TryParse(string hex, out Color color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(hex))
            return false;

        var input = hex.Trim();
        if (!input.StartsWith("#"))
            input = "#" + input;

        return ColorUtility.TryParseHtmlString(input, out color);
    }

    public static Color TryParseHexOrDefault(string hex, Color fallback)
    {
        return TryParse(hex, out var c) ? c : fallback;
    }

    private static string ColorToHex(Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }
}

public enum PrettyLogType
{
    Log,
    Warning,
    Error
}