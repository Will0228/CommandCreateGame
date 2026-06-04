using System;

namespace Presentation.DemoViewTest
{
    internal sealed class DemoViewBaseEditorAttribute
    {
        [AttributeUsage(AttributeTargets.Method)]
        internal class DemoSetupAttribute : Attribute
        {
                
        }
        
        [AttributeUsage(AttributeTargets.Method)]
        internal class DemoAnimatorSetupAttribute : Attribute
        {
                
        }
    }
}
