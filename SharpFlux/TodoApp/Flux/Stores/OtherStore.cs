using System.Collections.Generic;
using TodoApp.Flux.Actions;
using TodoApp.Models;
using SharpFlux;
using SharpFlux.Stores;
using SharpFlux.Dispatching;

namespace TodoApp.Flux.Stores
{
    public class OtherStore : Store<Payload<ItemActionTypes>, IList<Item>>
    {
        public OtherStore(IDispatcher dispatcher) : base(dispatcher, new List<Item>()) 
        {
        }

        protected override void OnDispatch(Payload<ItemActionTypes> payload)
        {
            switch (payload.ActionType)
            {
                case ItemActionTypes.AddItem:
                    //Test raise circular dependency error
                    //WaitFor(new List<string> { App.ItemStore.DispatchToken });
                    Data.Add(new Item { Id = "Test1", Description = "Should be inserted first", Text = "Test" });
                    EmitChange();
                    break;
                default:
                    break;
            }
        }
    }
}
