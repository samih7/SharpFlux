using System;

namespace SharpFlux.Stores
{
    public interface IStore<TData>
    {
        event EventHandler OnChanged;

        TData Data { get; }
        string DispatchToken { get; }
        bool HasChanged { get; }
    }
}