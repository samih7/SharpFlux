using System.Collections.Generic;
using TodoApp.Flux.Actions;
using TodoApp.Models;
using SharpFlux;
using SharpFlux.Stores;
using SharpFlux.Dispatching;
using System;

namespace TodoApp.Flux.Stores
{
    public class OtherStore : Store<Payload<ActionTypes>, IList<Item>>
    {
        public event EventHandler ItemUpserted;

        public OtherStore(IDispatcher dispatcher) : base(dispatcher, new List<Item>()) 
        {
        }

        protected override void OnDispatch(Payload<ActionTypes> payload)
        {
            switch (payload.ActionType)
            {
                case ActionTypes.UpsertItem:
                    //Test raise circular dependency error
                    //WaitFor(new List<string> { App.ItemStore.DispatchToken });
                    var item = new Item { Id = "Test1", Description = "Should be inserted first", Text = "Test" };
                    Data.Add(item);
                    HasChanged = true;
                    ItemUpserted?.Invoke(this, DataEventArgs<Item>.From(item));
                    break;
                default:
                    break;
            }
        }
    }
}
