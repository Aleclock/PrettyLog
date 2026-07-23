using System.Collections.Generic;
using UnityEngine;

public class LogChannel
{
    private string _name;
    private Color _mainColor;
    private LogFontStyle _fontStyle;

    public string Name
    {
        get => _name;
        set { _name = value; CacheHtmlTags(); }
    }

    public Color MainColor
    {
        get => _mainColor;
        set { _mainColor = value; CacheHtmlTags(); }
    }

    public LogFontStyle FontStyle
    {
        get => _fontStyle;
        set { _fontStyle = value; CacheHtmlTags(); }
    }

    public bool IsMuted;
    public bool PrintTimestamp;
    public LogVerbosity Verbosity = LogVerbosity.Debug;
    public string CachedTag { get; private set; }
    public Dictionary<string, LogSubChannel> SubChannels = new Dictionary<string, LogSubChannel>();

    public LogChannel(string name, Color mainColor, bool isBold = true)
    {
        _name = name;
        _mainColor = mainColor;
        _fontStyle = isBold ? LogFontStyle.Bold : LogFontStyle.Normal;
        IsMuted = false;
        PrintTimestamp = false;
        CacheHtmlTags();
    }

    public void CacheHtmlTags()
    {
        string hexMain = ColorUtility.ToHtmlStringRGBA(_mainColor);
        string tag = $"[{_name}]";
        switch (_fontStyle)
        {
            case LogFontStyle.Bold:
                tag = $"<b>{tag}</b>";
                break;
            case LogFontStyle.Italic:
                tag = $"<i>{tag}</i>";
                break;
            case LogFontStyle.BoldItalic:
                tag = $"<b><i>{tag}</i></b>";
                break;
        }
        CachedTag = $"<color=#{hexMain}>{tag}</color>";
    }
}

public struct LogSubChannel
{
    public string Name;
    public Color? CustomColor;
    public bool IsBold;
    public int FontSize;
    public bool IsMuted;
    public LogVerbosity? Verbosity;
    public string CachedTag { get; private set; }

    public LogSubChannel(string name, Color? customColor = null, bool isBold = false, int fontSize = 12, LogVerbosity? verbosity = null)
    {
        Name = name;
        CustomColor = customColor;
        IsBold = isBold;
        FontSize = fontSize;
        IsMuted = false;
        Verbosity = verbosity;
        CachedTag = string.Empty;
    }

    public void CacheHtmlTags(Color channelMainColor)
    {
        Color subColor = CustomColor ?? (channelMainColor * 0.85f);
        subColor.a = 1f;
        string hexSub = ColorUtility.ToHtmlStringRGBA(subColor);

        string tag = $"[{Name}]";
        if (IsBold)
        {
            tag = $"<b>{tag}</b>";
        }
        CachedTag = $"<color=#{hexSub}>{tag}</color>";
    }
}

public enum LogFontStyle
{
    Normal,
    Bold,
    Italic,
    BoldItalic
}

public enum LogVerbosity
{
    Silent = 0,
    Error = 1,
    Warning = 2,
    Info = 3,
    Verbose = 4,
    Debug = 5
}