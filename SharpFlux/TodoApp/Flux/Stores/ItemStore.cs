using System.Collections.Generic;
using System.Linq;
using TodoApp.Flux.Actions;
using TodoApp.Models;
using SharpFlux;

namespace TodoApp.Flux.Stores
{
    public class ItemStore : Store<Payload<ItemActionTypes>, IList<Item>>
    {
        //Commented dependency injection for testing purposes
        //private readonly OtherStore otherStore;
        //public ItemStore(Dispatcher dispatcher, OtherStore otherStore) : base(dispatcher, new List<Item>())
        //{
        //    this.otherStore = otherStore;
        //}
        public ItemStore(Dispatcher dispatcher) : base(dispatcher, new List<Item>()) 
        {
        }

        //Only place where we supposed to be mutating our data
        protected override void OnDispatch(Payload<ItemActionTypes> payload)
        {
            switch (payload.ActionType)
            {
                case ItemActionTypes.AddItem:
                    //We will wait for OtherStore's OnDispatch method to be executed before continuing here
                    WaitFor(new string[] { App.OtherStore.DispatchToken });
                    //We also can test its state
                    if (!App.OtherStore.HasChanged)
                    {
                        break;
                    }
                    var toAddItem = payload.Data as Item;
                    Data.Add(toAddItem);

                    //Emit changes to the ViewModels that are subscribing
                    EmitChange();
                    break;
                case ItemActionTypes.CompleteItem:
                    var itemCompleted = payload.Data as Item;
                    var itemToToggle = Data.FirstOrDefault(t => t.Id == itemCompleted?.Id);
                    if (itemToToggle == null)
                        return;

                    if (itemToToggle.IsComplete != itemCompleted.IsComplete)
                    {
                        itemToToggle.IsComplete = itemCompleted.IsComplete;
                        EmitChange();
                    }
                    break;
                case ItemActionTypes.GetItems:
                    var newItems = payload.Data as IList<Item>;
                    Data.Clear();
                    foreach (var item in newItems)
                        Data.Add(item);

                    EmitChange();
                    break;
                case ItemActionTypes.RemoveItem:
                    var toRemoveItem = payload.Data as Item;
                    Data.Remove(toRemoveItem);

                    EmitChange();
                    break;
                case ItemActionTypes.Failure:
                    break;
            }
        }
    }
}
