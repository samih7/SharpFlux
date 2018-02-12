using System;
using System.Collections.Generic;

namespace SharpFlux.Dispatching
{
    public interface IDispatcher
    {
        bool IsDispatching { get; }

        string Register<TPayload>(Action<TPayload> callback);
        void Unregister(string dispatchToken);
        void Dispatch<TPayload>(TPayload payload);
        void WaitFor<TPayload>(IEnumerable<string> dispatchTokens);
    }
}
