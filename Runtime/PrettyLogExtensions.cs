using UnityEngine;

public static class PrettyLogExtensions
{
    public static string Bold(this string text) => $"<b>{text}</b>";
    public static string Italic(this string text) => $"<i>{text}</i>";
    public static string Size(this string text, int size) => $"<size={size}>{text}</size>";
    
    // Quick inline hex colors
    public static string Color(this string text, string hexColor) => $"<color={hexColor}>{text}</color>";
    public static string Color(this string text, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
}