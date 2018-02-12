using System;
using System.Collections.Generic;

namespace SharpFlux.Dispatching
{
    public class Dispatcher : IDispatcher
    {
        private readonly IDictionary<string, object> callbacks = new Dictionary<string, object>();
        private readonly IDictionary<string, bool> isHandledCallbacks = new Dictionary<string, bool>(); 
        private readonly IDictionary<string, bool> isPendingCallbacks = new Dictionary<string, bool>();
        private string prefix = "id_";
        private int lastId;
        private object pendingPayload = new object();
        public bool IsDispatching { get; private set; }

        public Dispatcher()
        {
            lastId = 1;
        }

        public string Register<TPayload>(Action<TPayload> callback)
        {
            var dispatchToken = prefix + lastId++;
            callbacks[dispatchToken] = callback;

            return dispatchToken;
        }

        public void Dispatch<TPayload>(TPayload payload)
        {
            if (IsDispatching)
                throw new InvalidOperationException("Cannot dispatch while dispatching");

            try
            {
                StartDispatching(payload);

                foreach (var id in callbacks.Keys)
                {
                    if (isPendingCallbacks.ContainsKey(id) && isPendingCallbacks[id])
                        continue;

                    InvokeCallback<TPayload>(id);
                }
            }
            finally
            {
                StopDispatching();
            }
        }

        private void StartDispatching<TPayload>(TPayload payload)
        {
            foreach (var id in callbacks.Keys) 
            {
                isPendingCallbacks[id] = false;
                isHandledCallbacks[id] = false;
            }

            pendingPayload = payload;
            IsDispatching = true;
        }

        private void InvokeCallback<TPayload>(string id)
        {
            isPendingCallbacks[id] = true;

            var callback = callbacks[id] as Action<TPayload>;
            callback((TPayload)pendingPayload);
            
            isHandledCallbacks[id] = true;
        }

        private void StopDispatching()
        {
            pendingPayload = null;
            IsDispatching = false;
        }

        public void WaitFor<TPayload>(IEnumerable<string> dispatchTokens)
        {
            if (!IsDispatching)
                throw new InvalidOperationException("Must be handled when dispatching");
            
            foreach (var token in dispatchTokens)
            {
                if (isPendingCallbacks[token])
                {
                    if (!isHandledCallbacks[token]) //Store with this token is also waiting for us... Not allowed.
                        throw new InvalidOperationException($"Dispatcher WaitFor: circular dependency detected while waiting for {token}");

                    continue;
                }

                InvokeCallback<TPayload>(token);
            }
        }

        public void Unregister(string id)
        {
            if (!callbacks.ContainsKey(id))
                return;

            callbacks.Remove(id);
        }
    }
}
