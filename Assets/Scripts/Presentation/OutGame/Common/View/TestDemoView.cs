using Editor.Application;
using Presentation.DemoViewTest;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Test
{
    internal sealed class TestDemoView : DemoViewBase
    {
        [SerializeField] private Image _testImage; 
        [SerializeField] private string _testStringValue;
        [SerializeField] private Button _testButton;
        
#if UNITY_EDITOR
        [DemoViewBaseEditorAttribute.DemoSetup]
        private void DemoSetup(int testIntValue, Sprite testSprite, AudioClip testAudioClip)
        {
            _testImage.sprite = testSprite;
            AudioUtilityOnlyEditor.PlayClip(testAudioClip);
        }
        
        [DemoViewBaseEditorAttribute.DemoAnimatorSetup]
        private void SetupDemoButtonView(AnimatorController animatorController)
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}