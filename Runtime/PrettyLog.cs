using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PrettyLogSystem
{

/// <summary>
/// Facade for pretty logging. Use the static methods to write colored, sized
/// logs to the Unity console.
/// </summary>
    public static class PrettyLog
    {
        private static readonly Dictionary<string, LogChannel> _channels = new Dictionary<string, LogChannel>();
        private static readonly LogChannel _defaultChannel = new LogChannel("Log", Color.white);
        private static readonly object _lock = new object();

        #region REGISTRATION

        /// <summary> Registers a main logging channel. </summary>
        [Conditional("UNITY_EDITOR")]
        public static void RegisterChannel(string channelName, Color mainColor, bool isBold = true)
        {
            lock (_lock)
            {
                if (!_channels.TryGetValue(channelName, out var channel))
                {
                    _channels[channelName] = new LogChannel(channelName, mainColor, isBold);
                }
                else
                {
                    channel.MainColor = mainColor;
                    channel.FontStyle = isBold ? LogFontStyle.Bold : LogFontStyle.Normal;
                }
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void RegisterChannel(string channelName, string hexColor, bool isBold = true)
        {
            var color = TryParseHexOrDefault(hexColor, Color.white);
            RegisterChannel(channelName, color, isBold);
        }

        [Conditional("UNITY_EDITOR")]
        public static void RegisterSubChannel(string channelName, string subChannelName, Color? customColor = null, bool isBold = false, int fontSize = 12)
        {
            LogChannel channel = GetOrCreateChannel(channelName);
            var subChannel = new LogSubChannel(subChannelName, customColor, isBold, fontSize);
            subChannel.CacheHtmlTags(channel.MainColor);

            lock (_lock)
            {
                channel.SubChannels[subChannelName] = subChannel;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void RegisterSubChannel(string channelName, string subChannelName, string hexColor = null, bool isBold = false, int fontSize = 12)
        {
            Color? color = null;
            if (hexColor != null && TryParseColor(hexColor, out var parsedColor))
            {
                color = parsedColor;
            }
            RegisterSubChannel(channelName, subChannelName, color, isBold, fontSize);
        }

        #endregion

        #region MUTING

        [Conditional("UNITY_EDITOR")]
        public static void SetChannelMute(string channelName, bool isMuted)
        {
            lock (_lock)
            {
                if (_channels.TryGetValue(channelName, out LogChannel channel)) 
                    channel.IsMuted = isMuted;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetSubChannelMute(string channelName, string subChannelName, bool isMuted)
        {
            lock (_lock)
            {
                if (_channels.TryGetValue(channelName, out LogChannel channel) && channel.SubChannels.TryGetValue(subChannelName, out LogSubChannel sub))
                {
                    sub.IsMuted = isMuted;
                    channel.SubChannels[subChannelName] = sub; // Structs require reassignment
                }
            }
        }

        #endregion

        #region LOGGING

        [Conditional("UNITY_EDITOR")]
        public static void Log(string channelName, string subChannelName, string message)
        {
            bool isMuted = IsSubChannelMuted(channelName, subChannelName, out LogChannel channel, out LogSubChannel subChannel);
            if (isMuted) return;

            Debug.Log(BuildHTMLString(channel, subChannel, true, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string channelName, string subChannelName, string message)
        {
            bool isMuted = IsSubChannelMuted(channelName, subChannelName, out LogChannel channel, out LogSubChannel subChannel);
            if (isMuted) return;

            Debug.LogWarning(BuildHTMLString(channel, subChannel, true, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string channelName, string subChannelName, string message)
        {
            bool isMuted = IsSubChannelMuted(channelName, subChannelName, out LogChannel channel, out LogSubChannel subChannel);
            if (isMuted) return;

            Debug.LogError(BuildHTMLString(channel, subChannel, true, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void Log(string channelName, string message)
        {
            if (IsChannelMuted(channelName, out LogChannel channel)) return;
            Debug.Log(BuildHTMLString(channel, default, false, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string channelName, string message)
        {
            if (IsChannelMuted(channelName, out LogChannel channel)) return;
            Debug.LogWarning(BuildHTMLString(channel, default, false, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string channelName, string message)
        {
            if (IsChannelMuted(channelName, out LogChannel channel)) return;
            Debug.LogError(BuildHTMLString(channel, default, false, message));
        }

        #endregion

        #region HELPERS

        private static LogChannel GetOrCreateChannel(string channelName)
        {
            lock (_lock)
            {
                if (!_channels.TryGetValue(channelName, out var channel))
                {
                    channel = new LogChannel(channelName, Color.gray, isBold: true);
                    _channels[channelName] = channel;
                }
                return channel;
            }
        }

        private static bool IsChannelMuted(string channelName, out LogChannel channel)
        {
            channel = GetOrCreateChannel(channelName);
            return channel.IsMuted;
        }

        private static bool IsSubChannelMuted(string channelName, string subChannelName, out LogChannel channel, out LogSubChannel subChannel)
        {
            if (IsChannelMuted(channelName, out channel))
            {
                subChannel = default;
                return true;
            }

            lock (_lock)
            {
                if (channel.SubChannels.TryGetValue(subChannelName, out subChannel))
                {
                    return subChannel.IsMuted;
                }

                subChannel = new LogSubChannel(subChannelName, null);
                subChannel.CacheHtmlTags(channel.MainColor);
                channel.SubChannels[subChannelName] = subChannel;
                return false;
            }
        }   

        private static string BuildHTMLString(LogChannel channel, LogSubChannel subChannel, bool hasSubChannel, string rawMessage)
        {
            string tags = channel.CachedTag;
            
            if (channel.PrintTimestamp)
            {
                tags = $"[{System.DateTime.Now:HH:mm:ss.fff}] {tags}";
            }

            string message = rawMessage;

            if (hasSubChannel)
            {
                tags += subChannel.CachedTag;

                if (subChannel.FontSize != 12 && subChannel.FontSize > 0)
                {
                    message = $"<size={subChannel.FontSize}>{message}</size>";
                }
            }

            return $"{tags} {message}";
        }

        public static Color TryParseHexOrDefault(string hex, Color fallback)
        {
            return TryParseColor(hex, out var c) ? c : fallback;
        }

        public static bool TryParseColor(string hex, out Color color)
        {
            color = default;
            if (string.IsNullOrWhiteSpace(hex))
                return false;

            var input = hex.Trim();
            if (!input.StartsWith("#"))
                input = "#" + input;

            return ColorUtility.TryParseHtmlString(input, out color);
        }

        #endregion
        /*
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
        */
    }
}