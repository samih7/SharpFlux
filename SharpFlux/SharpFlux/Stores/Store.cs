﻿using System;
using System.Collections.Generic;
using SharpFlux.Dispatching;

namespace SharpFlux.Stores
{
    public abstract class Store<TPayload, TData> : IStore<TData>
    {
        private readonly object syncRoot = new object();
        private readonly IDispatcher dispatcher;
        public event EventHandler OnChanged;

        //Property that stores mutate and that is exposed to ViewModels for them to update 
        public TData Data { get; protected set; }

        //Returns the dispatch token that the dispatcher recognizes this store by
        //Can be used to WaitFor() this store
        public string DispatchToken { get; private set; }

        //Returns whether the store has changed during the most recent dispatch
        public bool HasChanged { get; set; }

        protected Store(IDispatcher dispatcher, TData initData)
        {
            this.dispatcher = dispatcher;
            Data = initData;

            DispatchToken = Subscribe();
        }

        //Dispatcher-forwarded methods so the API users do not have to care about the Dispatcher
        protected string Subscribe()
        {
            return dispatcher.Register<TPayload>(InvokeOnDispatch);
        }

        protected void Unsubscribe(string dispatchToken)
        {
            dispatcher.Unregister(dispatchToken);
        }

        protected void WaitFor(IEnumerable<string> dispatchTokens)
        {
            dispatcher.WaitFor<TPayload>(dispatchTokens);
        }

        //This is the store's registered callback method and all the logic that will be executed is contained here
        //Only place where state's mutation should happen
        private void InvokeOnDispatch(TPayload payload)
        {
            HasChanged = false;

            lock (syncRoot)
            {
                OnDispatch(payload);
            }

            //If a change is emitted (store implementation has called 'EmitChange'), we notify our ViewModels that subscribed
            //They will update the View through our getters (Data)
            if (!HasChanged)
                return;

            OnChanged?.Invoke(this, EventArgs.Empty);
        }
        //The callback that will be registered with the dispatcher during instanciation.
        //Subclasses must override this method.
        //This callback is the only way the store receives new data.
        protected abstract void OnDispatch(TPayload payload);

        protected void EmitChange()
        {
            if (!dispatcher.IsDispatching)
                throw new InvalidOperationException("Must be invoked while dispatching.");
            
            HasChanged = true;
        }
    }
}
