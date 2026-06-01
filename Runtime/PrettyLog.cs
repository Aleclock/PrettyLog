using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PrettyLogSystem
{

/// <summary>
/// Facade for pretty logging. Use the static methods to write colored, sized
/// logs to the Unity console. This class is lightweight and contains only
/// convenience overloads; the formatting logic lives in PrettyLogCore.
/// </summary>
    public static class PrettyLog
    {
        private static readonly Dictionary<string, LogChannel> _channels = new Dictionary<string, LogChannel>();
        private static readonly LogChannel _defaultChannel = new LogChannel("Log", Color.white);

        #region REGISTRATION

        /// <summary> Registers a main logging channel. </summary>
        public static void RegisterChannel(string channelName, Color mainColor)
            {
                if (!_channels.ContainsKey(channelName))
                {
                    _channels[channelName] = new LogChannel(channelName, mainColor);
                }
            }

        public static void RegisterChannel(string channelName, string hexColor)
        {
            var color = TryParseHexOrDefault(hexColor, Color.white);
            RegisterChannel(channelName, color);
        }

        public static void RegisterSubChannel(string channelName, string subChannelName, Color? customColor = null, bool isBold = false, int fontSize = 12)
        {
            if (_channels.TryGetValue(channelName, out LogChannel channel))
            {
                channel.SubChannels[subChannelName] = new LogSubChannel(subChannelName, customColor, isBold, fontSize);
            }
            else
            {
                RegisterChannel(channelName, Color.gray);
                _channels[channelName].SubChannels[subChannelName] = new LogSubChannel(subChannelName, customColor, isBold, fontSize);
            }
        }

        public static void RegisterSubChannel(string channelName, string subChannelName, string hexColor = null, bool isBold = false, int fontSize = 12)
        {
            var color = TryParseHexOrDefault(hexColor, Color.white);
            RegisterSubChannel(channelName, subChannelName, color, isBold, fontSize);
        }

        #endregion

        #region MUTING

        public static void SetChannelMute(string channelName, bool isMuted)
        {
            if (_channels.TryGetValue(channelName, out LogChannel channel)) 
                channel.IsMuted = isMuted;
        }

        public static void SetSubChannelMute(string channelName, string subChannelName, bool isMuted)
        {
            if (_channels.TryGetValue(channelName, out LogChannel channel) && channel.SubChannels.TryGetValue(subChannelName, out LogSubChannel sub))
            {
                sub.IsMuted = isMuted;
                channel.SubChannels[subChannelName] = sub; // Structs require reassignment
            }
        }

        #endregion

        #region LOGGING

        [Conditional("UNITY_EDITOR")]
        public static void Log(string channelName, string subChannelName, string message)
        {
            // Try to find the channel and subchannel configurations
            bool isMuted = IsSubChannelMuted(channelName, subChannelName, out LogChannel channel, out LogSubChannel? subChannel);
            if (isMuted) return;

            // Pass the nullable subChannel straight to the builder
            Debug.Log(BuildHTMLString(channel, subChannel, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string channelName, string subChannelName, string message)
        {
            bool isMuted = IsSubChannelMuted(channelName, subChannelName, out LogChannel channel, out LogSubChannel? subChannel);
            if (isMuted) return;

            Debug.LogWarning(BuildHTMLString(channel, subChannel, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string channelName, string subChannelName, string message)
        {
            bool isMuted = IsSubChannelMuted(channelName, subChannelName, out LogChannel channel, out LogSubChannel? subChannel);
            if (isMuted) return;

            Debug.LogError(BuildHTMLString(channel, subChannel, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void Log(string channelName, string message)
        {
            if (IsChannelMuted(channelName, out LogChannel channel)) return;
            Debug.Log(BuildHTMLString(channel, null, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string channelName, string message)
        {
            if (IsChannelMuted(channelName, out LogChannel channel)) return;
            Debug.LogWarning(BuildHTMLString(channel, null, message));
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string channelName, string message)
        {
            if (IsChannelMuted(channelName, out LogChannel channel)) return;
            Debug.LogError(BuildHTMLString(channel, null, message));
        }

        #endregion

        #region HELPERS

        private static bool IsChannelMuted(string channelName, out LogChannel channel)
        {
            if (!_channels.TryGetValue(channelName, out channel))
            {
                channel = _defaultChannel;
                return false;
            }
            return channel.IsMuted;
        }

        // Note how subChannel is now an 'out LogSubChannel?' (Nullable)
        private static bool IsSubChannelMuted(string channelName, string subChannelName, out LogChannel channel, out LogSubChannel? subChannel)
        {
            subChannel = null; // Default to null if not found
            
            if (IsChannelMuted(channelName, out channel)) return true;

            // Check if the dictionary actually contains this subchannel
            if (channel.SubChannels.TryGetValue(subChannelName, out LogSubChannel registeredSub))
            {
                subChannel = registeredSub; // It exists! Pass it along.
                return registeredSub.IsMuted;
            }

            // It was never registered! Treat it as null (no sub-channel tag will be drawn)
            return false; 
        }   

        private static string BuildHTMLString(LogChannel channel, LogSubChannel? subChannel, string rawMessage)
        {
            string hexMain = ColorUtility.ToHtmlStringRGBA(channel.MainColor);
            string tags = $"<color=#{hexMain}><b>[{channel.Name}]</b></color>";
            
            string message = rawMessage;

            // Apply Sub-Channel styling if it exists
            if (subChannel.HasValue)
            {
                LogSubChannel sub = subChannel.Value;
                Color subColor = sub.CustomColor ?? (channel.MainColor * 0.85f);
                subColor.a = 1f;
                string hexSub = ColorUtility.ToHtmlStringRGBA(subColor);

                // Append the sub-channel tag to the main tag
                tags += $"<color=#{hexSub}>[{sub.Name}]</color>";

                if (sub.IsBold) message = $"<b>{message}</b>";
                if (sub.FontSize != 12 && sub.FontSize > 0) message = $"<size={sub.FontSize}>{message}</size>";
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