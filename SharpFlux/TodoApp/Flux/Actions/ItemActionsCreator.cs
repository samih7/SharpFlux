using System.Threading.Tasks;
using TodoApp.Models;
using TodoApp.Services.Items;
using SharpFlux;

namespace TodoApp.Flux.Actions
{
    public class ItemActionsCreator
    {
        private readonly IItemService itemService;

        //Commented dependency injection for testing purposes
        //private readonly Dispatcher dispatcher;
        //public ItemActionsCreator(Dispatcher dispatcher, IItemService itemService)
        //{
        //    this.dispatcher = dispatcher;
        //    this.itemService = itemService;
        //}
        public ItemActionsCreator(IItemService itemService)
        {
            this.itemService = itemService;
        }

        //Action
        public async Task AddAsync(Item item)
        {
            try
            {
                //Some business logic
                await itemService.AddAsync(item);

                //Dispatching payload
                //dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.AddItem, item));
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.AddItem, item));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.Failure, ex.Message));
            }
        }

        public async Task DeleteAsync(Item item)
        {
            try
            {
                await itemService.RemoveAsync(item);
                //dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.RemoveItem, item));
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.RemoveItem, item));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.Failure, ex.Message));
            }
        }

        public async Task CompleteAsync(Item item)
        {
            try
            {
                await itemService.CompleteAsync(item);
                //dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.CompleteItem, item));
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.CompleteItem, item));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.Failure, ex.Message));
            }
        }

        public async Task GetItemsAsync()
        {
            try
            {
                var items = await itemService.GetItemsAsync();

                //dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.GetItems, items));
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.GetItems, items));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(new Payload<ItemActionTypes>(ItemActionTypes.Failure, ex.Message));
            }
        }
    }

    public enum ItemActionTypes
    {
        AddItem,
        CompleteItem,
        GetItems,
        RemoveItem,
        Failure
    }
}
