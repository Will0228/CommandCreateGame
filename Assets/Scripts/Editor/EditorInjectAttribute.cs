using System;

namespace Editor
{
    /// <summary>
    /// Editor拡張でDIを行えるようにするAttribute
    /// コンストラクタでのみ呼べるように
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    internal sealed class EditorInjectAttribute : Attribute
    {
        
    }
}
