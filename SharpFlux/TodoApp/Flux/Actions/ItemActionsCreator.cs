using System.Threading.Tasks;
using SharpFlux;
using TodoApp.Models;
using TodoApp.Services.Items;

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
                //dispatcher.Dispatch(new Payload<ActionTypes>(ActionTypes.UpsertItem, item));
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.UpsertItem, item));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.Failure, ex.Message));
            }
        }

        public async Task DeleteAsync(Item item)
        {
            try
            {
                await itemService.RemoveAsync(item);
                //dispatcher.Dispatch(new Payload<ActionTypes>(ActionTypes.RemoveItem, item));
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.RemoveItem, item));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.Failure, ex.Message));
            }
        }

        public async Task CompleteAsync(Item item)
        {
            try
            {
                await itemService.CompleteAsync(item);
                //dispatcher.Dispatch(new Payload<ActionTypes>(ActionTypes.UpsertItem, item));
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.UpsertItem, item));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.Failure, ex.Message));
            }
        }

        public async Task GetItemsAsync()
        {
            try
            {
                var items = await itemService.GetItemsAsync();

                //dispatcher.Dispatch(new Payload<ActionTypes>(ActionTypes.GetItems, items));
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.GetItems, items));
            }
            catch (System.Exception ex)
            {
                App.Dispatcher.Dispatch(Payload<ActionTypes>.From(ActionTypes.Failure, ex.Message));
            }
        }
    }
}
