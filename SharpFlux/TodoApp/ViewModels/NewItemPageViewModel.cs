using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoApp.Flux.Actions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace TodoApp.ViewModels
{
    public class NewItemPageViewModel : BindableBase, INavigationAware, IConfirmNavigationAsync, IDestructible
    {
        //Prism without Flux
        //private IEventAggregator eventAggregator;
        //private IItemService itemService;
        private INavigationService navigationService;
        private IPageDialogService dialogService;
        private readonly ItemActionsCreator itemActions;

        public ItemViewModel Item { get; set; }

        private DelegateCommand saveItemCommand;
        public ICommand SaveItemCommand => saveItemCommand ?? (saveItemCommand = new DelegateCommand(async () => {

            if (await navigationService.GoBackAsync(new NavigationParameters { { "refreshedByEvent", true } }))
            {
                var itemViewModel = new ItemViewModel { Id = Guid.NewGuid().ToString(), Text = "New item", Description = "New description" };
                await itemActions.AddAsync(itemViewModel.Model);

                //Prism without Flux - bidirectional data flow (a ViewModel talks to another one)
                //await itemService.AddAsync(Item);
                //eventAggregator.GetEvent<AddItemEvent>().Publish();
            }
        }));


        public NewItemPageViewModel(INavigationService navigationService, IPageDialogService dialogService, 
                                    ItemActionsCreator itemActionsCreator
                                    //Prism without Flux
                                    //IEventAggregator eventAggregator,
                                    //IItemService itemService
                                    )
        {
            //Prism without Flux
            //this.eventAggregator = eventAggregator;
            //this.itemService = itemService;

            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.itemActions = itemActionsCreator;

            Item = new ItemViewModel
            {
                Text = "Item name",
                Description = "This is an item description."
            };
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            parameters.Add("item", Item);
        }
        
        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public Task<bool> CanNavigateAsync(NavigationParameters parameters)
        {
            return dialogService.DisplayAlertAsync("You're about to quit", "Are you sure you want to leave?", "Yes", "No");
        }

        //Called when physical/virtual back button is tapped
        public void Destroy()
        {
            //Cleanup anything you want
        }
    }
}
