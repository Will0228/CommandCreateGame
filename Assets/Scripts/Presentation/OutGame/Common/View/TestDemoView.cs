using Presentation.DemoViewTest;
using UnityEngine;
using UnityEngine.UI;

namespace Presentation.Test
{
    internal sealed class TestDemoView : DemoViewBase
    {
        [SerializeField] private Image _testImage; 
        [SerializeField] private string _testStringValue;
        
        [DemoViewBaseEditor.DemoSetup]
        private void DemoSetup(int testIntValue, Sprite testSprite)
        {
            _testImage.sprite = testSprite;
        }
    }
}