using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PrettyLogSystem
{
    public static class PrettyQuickLog
    {
        private static readonly Dictionary<Color, string> _hexCache = new Dictionary<Color, string>();
        private static readonly object _lock = new object();

        private static string GetHexColor(Color color)
        {
            lock (_lock)
            {
                if (!_hexCache.TryGetValue(color, out string hex))
                {
                    hex = ColorUtility.ToHtmlStringRGBA(color);
                    _hexCache[color] = hex;
                }
                return hex;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void Log(string message, Color color, bool isBold = false)
        {
            string hex = GetHexColor(color);
            string formatted = $"<color=#{hex}>{message}</color>";
            if (isBold) formatted = $"<b>{formatted}</b>";
            UnityEngine.Debug.Log(formatted);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message, Color color, bool isBold = false)
        {
            string hex = GetHexColor(color);
            string formatted = $"<color=#{hex}>{message}</color>";
            if (isBold) formatted = $"<b>{formatted}</b>";
            UnityEngine.Debug.LogWarning(formatted);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message, Color color, bool isBold = false)
        {
            string hex = GetHexColor(color);
            string formatted = $"<color=#{hex}>{message}</color>";
            if (isBold) formatted = $"<b>{formatted}</b>";
            UnityEngine.Debug.LogError(formatted);
        }
    }
}