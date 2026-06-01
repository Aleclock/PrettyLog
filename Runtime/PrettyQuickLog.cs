using System.Diagnostics;
using UnityEngine;

namespace PrettyLogSystem
{
    public static class PrettyQuickLog
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message, Color color, bool isBold = false)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            string formatted = $"<color=#{hex}>{message}</color>";
            if (isBold) formatted = $"<b>{formatted}</b>";
            UnityEngine.Debug.Log(formatted);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message, Color color, bool isBold = false)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            string formatted = $"<color=#{hex}>{message}</color>";
            if (isBold) formatted = $"<b>{formatted}</b>";
            UnityEngine.Debug.LogWarning(formatted);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message, Color color, bool isBold = false)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            string formatted = $"<color=#{hex}>{message}</color>";
            if (isBold) formatted = $"<b>{formatted}</b>";
            UnityEngine.Debug.LogError(formatted);
        }
    }
}