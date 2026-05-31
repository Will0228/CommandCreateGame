using UnityEngine;

namespace Presentation.Test
{
    internal sealed class TestDemoView : DemoViewBase
    {
        [SerializeField] private int _testIntValue;
        [SerializeField] private string _testStringValue;
        
        [DemoSetup]
        public void DemoSetup(int testIntValue, string testStringValue)
        {
            _testIntValue = testIntValue;
            _testStringValue = testStringValue;
            Debug.Log($"{_testIntValue} + {_testStringValue}");
        }
    }
}