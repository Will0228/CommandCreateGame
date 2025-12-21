using System;
using R3;

namespace Application.Base
{
    public interface IState : IDisposable
    {
        Observable<StateBase> TransitionToNextStateAsObservable { get; }

        void Configure();
    }
}