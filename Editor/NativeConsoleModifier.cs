#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PrettyLogSystem.Editor
{
    [InitializeOnLoad]
    public static class NativeConsoleModifier
    {
        private static Texture2D _originalInfoTexture;
        private static Texture2D _originalInfoSmallTexture;
        private static Texture2D _originalInfoMonoTexture;

        private static Texture2D _originalStyleBackground;
        private static Texture2D _originalStyleSmallBackground;

        static NativeConsoleModifier()
        {
            // Automatically try to apply when the editor updates or compiles
            EditorApplication.delayCall += AutoInject;
        }

        private static void AutoInject()
        {
            string[] guids = AssetDatabase.FindAssets("PrettyLogIcon t:Texture2D");
            if (guids.Length > 0)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
                InjectIcon(tex);
            }
        }

        public static void InjectIcon(Texture2D newIcon)
        {
            if (newIcon == null) return;

            try
            {
                var assembly = typeof(EditorWindow).Assembly;
                var consoleWindowType = assembly.GetType("UnityEditor.ConsoleWindow");
                if (consoleWindowType == null) return;

                // 1. Override the static internal Texture2D fields (used for toggles/status bar)
                var iconInfoField = consoleWindowType.GetField("iconInfo", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var iconInfoSmallField = consoleWindowType.GetField("iconInfoSmall", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var iconInfoMonoField = consoleWindowType.GetField("iconInfoMono", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                if (iconInfoField != null)
                {
                    Texture2D current = iconInfoField.GetValue(null) as Texture2D;
                    if (current != null && current != newIcon)
                    {
                        _originalInfoTexture = current;
                    }
                    iconInfoField.SetValue(null, newIcon);
                }

                if (iconInfoSmallField != null)
                {
                    Texture2D current = iconInfoSmallField.GetValue(null) as Texture2D;
                    if (current != null && current != newIcon)
                    {
                        _originalInfoSmallTexture = current;
                    }
                    iconInfoSmallField.SetValue(null, newIcon);
                }

                if (iconInfoMonoField != null)
                {
                    Texture2D current = iconInfoMonoField.GetValue(null) as Texture2D;
                    if (current != null && current != newIcon)
                    {
                        _originalInfoMonoTexture = current;
                    }
                    iconInfoMonoField.SetValue(null, newIcon);
                }

                // 2. Override the GUIStyles in Constants (used for drawing list row entry icons)
                var constantsType = consoleWindowType.GetNestedType("Constants", BindingFlags.Public | BindingFlags.NonPublic);
                if (constantsType != null)
                {
                    var iconLogStyleField = constantsType.GetField("IconLogStyle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    var iconLogSmallStyleField = constantsType.GetField("IconLogSmallStyle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    if (iconLogStyleField != null)
                    {
                        GUIStyle style = iconLogStyleField.GetValue(null) as GUIStyle;
                        if (style != null)
                        {
                            Texture2D current = style.normal.background;
                            if (current != null && current != newIcon)
                            {
                                _originalStyleBackground = current;
                            }
                            style.normal.background = newIcon;
                            style.normal.scaledBackgrounds = new Texture2D[] { newIcon };
                        }
                    }

                    if (iconLogSmallStyleField != null)
                    {
                        GUIStyle style = iconLogSmallStyleField.GetValue(null) as GUIStyle;
                        if (style != null)
                        {
                            Texture2D current = style.normal.background;
                            if (current != null && current != newIcon)
                            {
                                _originalStyleSmallBackground = current;
                            }
                            style.normal.background = newIcon;
                            style.normal.scaledBackgrounds = new Texture2D[] { newIcon };
                        }
                    }
                }

                // Force repaint
                var window = EditorWindow.GetWindow(consoleWindowType, false, null, false);
                if (window != null)
                {
                    window.Repaint();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PrettyLog] Could not override native console icons: {ex.Message}");
            }
        }

        public static void RestoreDefaultIcons()
        {
            try
            {
                var assembly = typeof(EditorWindow).Assembly;
                var consoleWindowType = assembly.GetType("UnityEditor.ConsoleWindow");
                if (consoleWindowType == null) return;

                // Load default Unity textures as fallbacks
                Texture2D defaultInfoTex = _originalInfoTexture ?? (EditorGUIUtility.IconContent("console.infoicon").image as Texture2D);
                Texture2D defaultInfoSmallTex = _originalInfoSmallTexture ?? (EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D);
                Texture2D defaultInfoMonoTex = _originalInfoMonoTexture ?? defaultInfoSmallTex;

                // 1. Restore static internal Texture2D fields
                var iconInfoField = consoleWindowType.GetField("iconInfo", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var iconInfoSmallField = consoleWindowType.GetField("iconInfoSmall", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var iconInfoMonoField = consoleWindowType.GetField("iconInfoMono", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                if (iconInfoField != null && defaultInfoTex != null)
                {
                    iconInfoField.SetValue(null, defaultInfoTex);
                }

                if (iconInfoSmallField != null && defaultInfoSmallTex != null)
                {
                    iconInfoSmallField.SetValue(null, defaultInfoSmallTex);
                }

                if (iconInfoMonoField != null && defaultInfoMonoTex != null)
                {
                    iconInfoMonoField.SetValue(null, defaultInfoMonoTex);
                }

                // 2. Restore GUIStyles inside Constants
                var constantsType = consoleWindowType.GetNestedType("Constants", BindingFlags.Public | BindingFlags.NonPublic);
                if (constantsType != null)
                {
                    var iconLogStyleField = constantsType.GetField("IconLogStyle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    var iconLogSmallStyleField = constantsType.GetField("IconLogSmallStyle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    Texture2D defaultStyleBackground = _originalStyleBackground ?? defaultInfoTex;
                    Texture2D defaultStyleSmallBackground = _originalStyleSmallBackground ?? defaultInfoSmallTex;

                    if (iconLogStyleField != null && defaultStyleBackground != null)
                    {
                        GUIStyle style = iconLogStyleField.GetValue(null) as GUIStyle;
                        if (style != null)
                        {
                            style.normal.background = defaultStyleBackground;
                            style.normal.scaledBackgrounds = new Texture2D[] { defaultStyleBackground };
                        }
                    }

                    if (iconLogSmallStyleField != null && defaultStyleSmallBackground != null)
                    {
                        GUIStyle style = iconLogSmallStyleField.GetValue(null) as GUIStyle;
                        if (style != null)
                        {
                            style.normal.background = defaultStyleSmallBackground;
                            style.normal.scaledBackgrounds = new Texture2D[] { defaultStyleSmallBackground };
                        }
                    }
                }

                var window = EditorWindow.GetWindow(consoleWindowType, false, null, false);
                if (window != null)
                {
                    window.Repaint();
                }
            }
            catch
            {
                // Fail silently
            }
        }
    }
}
#endif
