using System;

namespace SharpFlux
{
    public abstract class Store<TPayload, TData>
    {
        private readonly object syncRoot = new object();
        private readonly Dispatcher dispatcher;
        public event EventHandler OnStateChanged;

        //Property that stores mutate and that is exposed to ViewModels for them to update 
        public TData Data { get; }

        //Returns the dispatch token that the dispatcher recognizes this store by
        //Can be used to WaitFor() this store
        public string DispatchToken { get; private set; }

        //Returns whether the store has changed during the most recent dispatch
        public bool HasChanged { get; set; }

        protected Store(Dispatcher dispatcher, TData initData)
        {
            this.dispatcher = dispatcher;
            Data = initData;

            DispatchToken = this.dispatcher.Register<TPayload>(InvokeOnDispatch);
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
            if (HasChanged)
                OnStateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void EmitChange()
        {
            if (!dispatcher.IsDispatching)
                throw new InvalidOperationException("Must be invoked while dispatching.");
            HasChanged = true;
        }

        //The callback that will be registered with the dispatcher during instanciation.
        //Subclasses must override this method.
        //This callback is the only way the store receives new data.
        protected abstract void OnDispatch(TPayload payload);

        //Dispatcher-forwarded methods so the API users do not have to care about the Dispatcher
        protected void Unsubscribe()
        {
            dispatcher.Unregister(DispatchToken);
        }

        protected void WaitFor(string[] dispatchTokens)
        {
            dispatcher.WaitFor<TPayload>(dispatchTokens);
        }
    }
}
