using UnityEditor;
using UnityEngine;

namespace GamingUnityEditor
{
    internal sealed class GamingData : ScriptableSingleton<GamingData>
    {
        public GamingEditorSettings settings;

        public int mainHue = 0;
        public int complementHue = 127;

        public bool isGamingActivation;
        public bool isGamingPlayEditMode;
        public bool isGamingPlayPlayMode;
        public bool isGamingPlayRect;
        public bool isGamingPlayBlit;
        public bool isGamingPlayClip;
        public bool isGamingPlayClipText;
    }

    internal sealed class GamingShaderData
    {
        private readonly string _shaderName;

        public GamingShaderData(string shaderName)
        {
            _shaderName = shaderName;
        }

        private static int GetShaderPropertyToID(string propertyName)
        {
            return Shader.PropertyToID(propertyName);
        }

        public void Activate(bool value)
        {
            Shader.SetGlobalInt(GetShaderPropertyToID($"_Activate{_shaderName}"), value ? 1 : 0);
        }

        public void SetColor(Color color)
        {
            Shader.SetGlobalColor(GetShaderPropertyToID($"_{_shaderName}Color"), color);
        }
    }

    internal enum PlayType
    {
        None,
        On,
        Off,
    }
}