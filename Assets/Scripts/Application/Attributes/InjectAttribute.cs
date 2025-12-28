using System;

namespace Application.Attributes
{
    /// <summary>
    /// DIを行えるようにするAttribute
    /// コンストラクタでのみ呼べるように
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute
    {
        
    }
}