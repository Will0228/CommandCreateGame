using System;
using System.Reflection;
using Editor;
using Editor.Application;
using Presentation.DemoViewTest;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Test
{
    internal sealed class TestDemoView : DemoViewBase
    {
        [SerializeField] private Image _testImage; 
        [SerializeField] private string _testStringValue;
        
#if UNITY_EDITOR
        [DemoViewBaseEditor.DemoSetup]
        private void DemoSetup(int testIntValue, Sprite testSprite, AudioClip testAudioClip)
        {
            _testImage.sprite = testSprite;
            AudioUtilityOnlyEditor.PlayClip(testAudioClip);
        }
#endif
    }
}