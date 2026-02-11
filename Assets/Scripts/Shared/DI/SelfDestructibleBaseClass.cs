using System;
using R3;

namespace Shared.DI
{
    /// <summary>
    /// 自身でDIContainerに対してメモリ解放を行うように伝えることができる
    /// 自身でメモリ解放のタイミングを調整したい時に使用
    /// </summary>
    public abstract class SelfDestructibleBaseClass
    {
        protected readonly Subject<Object> onDestroySubject = new();
        public Observable<Object> DestroyAsObservable => onDestroySubject.AsObservable();
    }
}