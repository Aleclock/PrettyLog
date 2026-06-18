using UnityEngine;

public static class PrettyLogExtensions
{
    public static string Bold(this string text) => $"<b>{text}</b>";
    public static string Italic(this string text) => $"<i>{text}</i>";
    public static string Size(this string text, int size) => $"<size={size}>{text}</size>";
    
    // Quick inline hex colors
    public static string Color(this string text, string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor)) return text;
        string hex = hexColor.Trim();
        if (!hex.StartsWith("#"))
        {
            bool isNamedColor = hex.Equals("red", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("green", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("blue", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("white", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("black", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("yellow", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("cyan", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("magenta", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("gray", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("grey", System.StringComparison.OrdinalIgnoreCase) ||
                                hex.Equals("clear", System.StringComparison.OrdinalIgnoreCase);
            if (!isNamedColor)
            {
                hex = "#" + hex;
            }
        }
        return $"<color={hex}>{text}</color>";
    }
    public static string Color(this string text, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
}