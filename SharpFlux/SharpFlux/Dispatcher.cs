using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpFlux
{
    public class Dispatcher
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

        //Waits for the callbacks specified to be invoked before continuing execution
        //of the current callback.This method should only be used by a callback in
        //response to a dispatched payload.
        public void WaitFor<TPayload>(string[] storesDispatchTokens)
        {
            if (!IsDispatching)
                throw new InvalidOperationException("Must be handled when dispatching");
            
            foreach (var storeToken in storesDispatchTokens)
            {
                if (isPendingCallbacks[storeToken])
                {
                    if (!isHandledCallbacks[storeToken])
                        throw new InvalidOperationException($"Dispatcher WaitFor: circular dependency detected while waiting for {storeToken}");

                    continue;
                }

                InvokeCallback<TPayload>(storeToken);
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
