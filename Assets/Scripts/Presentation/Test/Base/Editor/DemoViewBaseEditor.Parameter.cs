using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Presentation.DemoViewTest
{
    internal partial class DemoViewBaseEditor
    {
        internal interface IGuiParameter
        {
            public string ParameterName { get; }
            
            void Initialize(string parameterName, string defaultValue);
            void SetField(string value);
        }
        
        internal sealed class IntParameter : IGuiParameter
        {
            public int Value { get; private set; }
            
            private string _parameterName;
            string IGuiParameter.ParameterName => _parameterName;

            void IGuiParameter.Initialize(string parameterName, string defaultValue)
            {
                _parameterName = parameterName;
                ((IGuiParameter)this).SetField(defaultValue);
            }

            void IGuiParameter.SetField(string value) => Value = int.Parse(value);
        }

        internal class SpriteParameter : IGuiParameter
        {
            public Sprite Value { get; private set; }
            private string _parameterName;
            string IGuiParameter.ParameterName => _parameterName;

            void IGuiParameter.Initialize(string parameterName, string defaultValue)
            {
                _parameterName = parameterName;
                ((IGuiParameter)this).SetField(defaultValue);
            }

            void IGuiParameter.SetField(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Value = null;
                    return;
                }

                // 1. 文字列（GUID）からアセットのパス（"Assets/.../image.png"）を検索
                string assetPath = AssetDatabase.GUIDToAssetPath(value);

                // もし入力された文字列がGUIDではなく直接の「パス」だった場合のフォールバック
                if (string.IsNullOrEmpty(assetPath))
                {
                    assetPath = value; 
                }

                // 2. パスからSpriteとしてアセットをロードして保持
                Value = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

                if (Value == null)
                {
                    Debug.LogWarning($"[SpriteParameter] アセットが見つかりませんでした: {value}");
                }
            }
        }
    }
}

#endif