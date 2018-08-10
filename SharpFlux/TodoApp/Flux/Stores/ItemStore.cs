using System.Collections.Generic;
using System.Linq;
using TodoApp.Flux.Actions;
using TodoApp.Models;
using SharpFlux;
using SharpFlux.Stores;
using SharpFlux.Dispatching;
using System;

namespace TodoApp.Flux.Stores
{
    public class ItemStore : Store<Payload<ActionTypes>, IList<Item>>
    {
        public event EventHandler ItemUpserted;
        public event EventHandler ItemsFetched;
        public event EventHandler ItemRemoved;
        //Commented dependency injection for testing purposes
        //private readonly OtherStore otherStore;
        //public ItemStore(Dispatcher dispatcher, OtherStore otherStore) : base(dispatcher, new List<Item>())
        //{
        //    this.otherStore = otherStore;
        //}
        public ItemStore(IDispatcher dispatcher) : base(dispatcher, new List<Item>()) 
        {
        }

        //Only place where we supposed to be mutating our data
        protected override void OnDispatch(Payload<ActionTypes> payload)
        {
            switch (payload.ActionType)
            {
                case ActionTypes.UpsertItem:
                    //We will wait for OtherStore's OnDispatch method to be executed before continuing here
                    WaitFor(new string[] { App.OtherStore.DispatchToken });
                    //We also can test its state
                    if (!App.OtherStore.HasChanged)
                    {
                        break;
                    }
                    var toUpsertItem = payload.Data as Item;
                    var existingItem = Data.FirstOrDefault(t => t.Id == toUpsertItem.Id);
                    if (existingItem == null)
                    {
                        Data.Add(toUpsertItem);
                        HasChanged = true;
                        ItemUpserted?.Invoke(this, DataEventArgs<Item>.From(toUpsertItem));
                        return;
                    }
                    Data.Remove(existingItem);
                    Data.Add(toUpsertItem);

                    HasChanged = true;
                    ItemUpserted?.Invoke(this, DataEventArgs<Item>.From(toUpsertItem));
                    //Emit changes to the ViewModels that are subscribing
                    break;
                case ActionTypes.GetItems:
                    var newItems = payload.Data as IList<Item>;
                    Data.Clear();
                    foreach (var item in newItems)
                        Data.Add(item);

                    HasChanged = true;
                    ItemsFetched?.Invoke(this, DataEventArgs<IList<Item>>.From(newItems));
                    break;
                case ActionTypes.RemoveItem:
                    var toRemoveItem = payload.Data as Item;
                    Data.Remove(toRemoveItem);

                    HasChanged = true;
                    ItemRemoved?.Invoke(this, DataEventArgs<Item>.From(toRemoveItem));
                    break;
                case ActionTypes.Failure:
                    break;
            }
        }
    }
}
