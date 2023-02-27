using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GamingUnityEditor
{
    internal sealed class GamingEditorSettings : EditorWindow
    {
        private static MiniRx<bool> _isActivate;
        private static MiniRx<bool> _isEditMode;
        private static MiniRx<bool> _isPlayMode;
        private static MiniRx<bool> _isRect;
        private static MiniRx<bool> _isBlit;
        private static MiniRx<bool> _isClip;
        private static MiniRx<bool> _isClipText;
        private static MiniRx<PlayType> _modePlayType;
        private static List<IDisposable> _disposables;

        private const string TITLE = "Gaming Editor";

        [MenuItem("Tools/" + TITLE + "/Settings")]
        private static void OpenWindow()
        {
            GetInstance().settings.Show();
        }

        private static GamingData GetInstance()
        {
            var gamingData = GamingData.instance;
            if (gamingData.settings == null)
            {
                gamingData.settings = CreateInstance<GamingEditorSettings>();
                gamingData.settings.minSize = new Vector2(350, 400);
                gamingData.settings.titleContent = new GUIContent(TITLE);
            }

            return gamingData;
        }

        #region Init

        public static void Init(
            Action<bool> activateRect,
            Action<bool> activateRectBorder,
            Action<bool> activateTexture,
            Action<bool> activateBlit,
            Action<bool> activateClip,
            Action<bool> activateClipText,
            // Action<bool> activateBlitCopy,
            Action repaint)
        {
            var gamingData = GetInstance();

            _isActivate = new MiniRx<bool>();
            _isEditMode = new MiniRx<bool>();
            _isPlayMode = new MiniRx<bool>();
            _isRect = new MiniRx<bool>();
            _isBlit = new MiniRx<bool>();
            _isClip = new MiniRx<bool>();
            _isClipText = new MiniRx<bool>();
            _modePlayType = new MiniRx<PlayType>();
            _disposables = new List<IDisposable>();

            // Update and Save gaming flag
            _isActivate
                .Subscribe(x =>
                {
                    gamingData.isGamingActivation = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingActivation), x);
                })
                .AddTo(_disposables);
            _isEditMode
                .Subscribe(x =>
                {
                    gamingData.isGamingPlayEditMode = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingPlayEditMode), x);
                })
                .AddTo(_disposables);
            _isPlayMode
                .Subscribe(x =>
                {
                    gamingData.isGamingPlayPlayMode = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingPlayPlayMode), x);
                })
                .AddTo(_disposables);
            _isRect
                .Subscribe(x =>
                {
                    gamingData.isGamingPlayRect = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingPlayRect), x);
                })
                .AddTo(_disposables);
            _isBlit
                .Subscribe(x =>
                {
                    gamingData.isGamingPlayBlit = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingPlayBlit), x);
                })
                .AddTo(_disposables);
            _isClip
                .Subscribe(x =>
                {
                    gamingData.isGamingPlayClip = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingPlayClip), x);
                })
                .AddTo(_disposables);
            _isClipText
                .Subscribe(x =>
                {
                    gamingData.isGamingPlayClipText = x;
                    EditorPrefs.SetBool(nameof(gamingData.isGamingPlayClipText), x);
                })
                .AddTo(_disposables);

            // Update shader property
            _isActivate
                .Where(x => x)
                .Subscribe(_ =>
                {
                    activateRect?.Invoke(gamingData.isGamingPlayRect);
                    activateRectBorder?.Invoke(gamingData.isGamingPlayRect);
                    activateTexture?.Invoke(gamingData.isGamingPlayBlit);
                    activateBlit?.Invoke(gamingData.isGamingPlayBlit);
                    activateClip?.Invoke(gamingData.isGamingPlayClip);
                    activateClipText?.Invoke(gamingData.isGamingPlayClipText);
                    // activateBlitCopy?.Invoke(gamingData.isGamingPlayClip);
                })
                .AddTo(_disposables);
            _isActivate
                .Where(x => x == false)
                .Subscribe(_ =>
                {
                    activateRect?.Invoke(false);
                    activateRectBorder?.Invoke(false);
                    activateTexture?.Invoke(false);
                    activateBlit?.Invoke(false);
                    activateClip?.Invoke(false);
                    activateClipText?.Invoke(false);
                    // activateBlitCopy?.Invoke(false);
                    repaint?.Invoke();
                })
                .AddTo(_disposables);
            _isRect
                .Where(_ => gamingData.isGamingActivation)
                .Subscribe(x =>
                {
                    activateRect?.Invoke(x);
                    activateRectBorder?.Invoke(x);
                })
                .AddTo(_disposables);
            _isBlit
                .Where(_ => gamingData.isGamingActivation)
                .Subscribe(x =>
                {
                    activateTexture?.Invoke(x);
                    activateBlit?.Invoke(x);
                })
                .AddTo(_disposables);
            _isClip
                .Where(_ => gamingData.isGamingActivation)
                .Subscribe(x =>
                {
                    activateClip?.Invoke(x);
                    // activateBlitCopy?.Invoke(x);
                })
                .AddTo(_disposables);
            _isClipText
                .Where(_ => gamingData.isGamingActivation)
                .Subscribe(x =>
                {
                    activateClipText?.Invoke(x);
                })
                .AddTo(_disposables);
            _modePlayType
                .Subscribe(x =>
                {
                    var value = x == PlayType.On;
                    activateRect?.Invoke(value && gamingData.isGamingPlayRect);
                    activateRectBorder?.Invoke(value && gamingData.isGamingPlayRect);
                    activateTexture?.Invoke(value && gamingData.isGamingPlayBlit);
                    activateBlit?.Invoke(value && gamingData.isGamingPlayBlit);
                    activateClip?.Invoke(value && gamingData.isGamingPlayClip);
                    activateClipText?.Invoke(value && gamingData.isGamingPlayClipText);
                    // activateBlitCopy?.Invoke(value && gamingData.isGamingPlayClip);
                    repaint?.Invoke();
                })
                .AddTo(_disposables);

            // Get save data from EditorPrefs
            _isActivate.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingActivation), gamingData.isGamingActivation));
            _isEditMode.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingPlayEditMode), gamingData.isGamingPlayEditMode));
            _isPlayMode.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingPlayPlayMode), gamingData.isGamingPlayPlayMode));
            _isRect.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingPlayRect), gamingData.isGamingPlayRect));
            _isBlit.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingPlayBlit), gamingData.isGamingPlayBlit));
            _isClip.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingPlayClip), gamingData.isGamingPlayClip));
            _isClipText.OnNext(EditorPrefs.GetBool(nameof(gamingData.isGamingPlayClipText), gamingData.isGamingPlayClipText));
        }

        #endregion

        #region OnGUI

        private void OnGUI()
        {
            var gamingData = GamingData.instance;

            SetTitle("Activation");
            {
                Draw("ON", () => _isActivate.OnNext(GetToggle(gamingData.isGamingActivation)));
            }

            SetTitle("Play Timing");
            {
                Draw("Edit Mode", () => _isEditMode.OnNext(GetToggle(gamingData.isGamingPlayEditMode)));
                Draw("Play Mode", () => _isPlayMode.OnNext(GetToggle(gamingData.isGamingPlayPlayMode)));
            }

            SetTitle("Target Area");
            {
                Draw("Background", () => _isBlit.OnNext(GetToggle(gamingData.isGamingPlayBlit)));
                Draw("EditorWindowTitle's background / Input field", () => _isRect.OnNext(GetToggle(gamingData.isGamingPlayRect)));
                Draw("Icon", () => _isClip.OnNext(GetToggle(gamingData.isGamingPlayClip)));
                Draw("Text", () => _isClipText.OnNext(GetToggle(gamingData.isGamingPlayClipText)));
            }
        }

        private static void SetTitle(string title)
        {
            GUILayout.Space(7.5f);
            GUILayout.Label(title);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2.5f));
        }

        private static void Draw(string label, Action button)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    GUILayout.Space(10);
                    button?.Invoke();
                    GUILayout.Label(label);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private static bool GetToggle(bool value)
        {
            return GUILayout.Toggle(value, "", GUILayout.Width(20));
        }

        #endregion

        public static void OnNextPlayMode(PlayType value)
        {
            _modePlayType.OnNext(value);
        }

        public static void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }
    }
}