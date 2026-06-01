using System.Collections.Generic;
using UnityEngine;

public class LogChannel
{
    public string Name;
    public Color MainColor;
    public bool IsMuted;
    public bool PrintTimestamp;
    public LogFontStyle FontStyle;
    public Dictionary<string, LogSubChannel> SubChannels = new Dictionary<string, LogSubChannel>();

    public LogChannel(string name, Color mainColor)
    {
        Name = name;
        MainColor = mainColor;
        IsMuted = false;
    }
}

public struct LogSubChannel
{
    public string Name;
    public Color? CustomColor;
    public bool IsBold;
    public int FontSize;
    public bool IsMuted;

    public LogSubChannel(string name, Color? customColor = null, bool isBold = false, int fontSize = 12)
    {
        Name = name;
        CustomColor = customColor;
        IsBold = isBold;
        FontSize = fontSize;
        IsMuted = false;
    }
}

public enum LogFontStyle
{
    Normal,
    Bold,
    Italic,
    BoldItalic
}