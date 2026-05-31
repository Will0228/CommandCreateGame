using System;

namespace Presentation.DemoViewTest
{
    internal partial class DemoViewBaseEditor
    {
        [AttributeUsage(AttributeTargets.Method)]
        internal class DemoSetupAttribute : Attribute
        {
                
        }
    }
}
