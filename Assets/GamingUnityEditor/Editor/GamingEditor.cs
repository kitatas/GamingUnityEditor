using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamingUnityEditor
{
    [InitializeOnLoad]
    internal sealed class GamingEditor
    {
        private static readonly Type _guiView = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
        private static readonly BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly PropertyInfo _visualTree = _guiView.GetProperty("visualTree", _bindingFlags);

        private static readonly GamingShaderData _rect       = new("InternalGUIRoundedRect");
        private static readonly GamingShaderData _rectBorder = new("InternalGUIRoundedRectWithColorPerBorder");
        private static readonly GamingShaderData _texture    = new("InternalGUITexture");
        private static readonly GamingShaderData _blit       = new("InternalGUITextureBlit");
        private static readonly GamingShaderData _clip       = new("InternalGUITextureClip");
        private static readonly GamingShaderData _clipText   = new("InternalGUITextureClipText");
        // private static readonly GamingShaderData _blitCopy   = new("InternalUIRAtlasBlitCopy");

        static GamingEditor()
        {
            GamingEditorSettings.Init(
                _rect.Activate,
                _rectBorder.Activate,
                _texture.Activate,
                _blit.Activate,
                _clip.Activate,
                _clipText.Activate,
                // _blitCopy.Activate,
                Repaint);

            var gamingData = GamingData.instance;

            EditorApplication.update += () =>
            {
                if (gamingData.isGamingActivation == false) return;

                var isPlaying = EditorApplication.isPlaying;

                var mode = isPlaying
                    ? gamingData.isGamingPlayPlayMode ? PlayType.On : PlayType.Off
                    : gamingData.isGamingPlayEditMode ? PlayType.On : PlayType.Off;
                GamingEditorSettings.OnNextPlayMode(mode);

                if ((gamingData.isGamingPlayPlayMode && isPlaying) ||
                    (gamingData.isGamingPlayEditMode && isPlaying == false))
                {
                    Repaint();
                    Tick(gamingData);
                }
            };
        }

        ~GamingEditor()
        {
            GamingEditorSettings.Dispose();
        }

        private static void Repaint()
        {
            var guiViews = Resources.FindObjectsOfTypeAll(_guiView);
            foreach (var guiView in guiViews)
            {
                var visualElement = _visualTree.GetValue(guiView, null) as VisualElement;
                visualElement?.MarkDirtyRepaint();
            }
        }

        private static void Tick(GamingData gamingData)
        {
            var mainColor = gamingData.mainHue.ToGamingColor();
            var complementColor = gamingData.complementHue.ToGamingColor();

            if (gamingData.isGamingPlayRect)
            {
                _rect.SetColor(mainColor);
                _rectBorder.SetColor(complementColor);
            }

            if (gamingData.isGamingPlayBlit)
            {
                _texture.SetColor(mainColor);
                _blit.SetColor(mainColor);
            }

            if (gamingData.isGamingPlayClip)
            {
                _clip.SetColor(complementColor);
                // _blitCopy.SetColor(complementColor);
            }

            if (gamingData.isGamingPlayClipText)
            {
                _clipText.SetColor(complementColor);
            }
        }
    }
}