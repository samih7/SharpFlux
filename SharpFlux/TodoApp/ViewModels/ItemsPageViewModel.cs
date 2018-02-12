using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoApp.Flux.Actions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;

namespace TodoApp.ViewModels
{
    public class ItemsPageViewModel : BaseViewModel, INavigationAware
    {
        //Prism without Flux
        //private readonly IEventAggregator eventAggregator;
        //private readonly IItemService itemService;

        private readonly INavigationService navigationService;
        private readonly IPageDialogService dialogService;
        private readonly ItemActionsCreator itemActionsCreator;
        //Commented for testing purposes
        //private readonly ItemStore itemStore;

        public ObservableCollection<ItemViewModel> Items { get; set; }

        private DelegateCommand loadItemsCommand;
        public ICommand LoadItemsCommand => loadItemsCommand ?? (loadItemsCommand = new DelegateCommand(async () =>
        {
            await LoadItemsAsync();
        }));

        private DelegateCommand addItemCommand;
        public ICommand AddItemCommand => addItemCommand ?? (addItemCommand = new DelegateCommand(async () =>
        {
            await navigationService.NavigateAsync("NewItemPage");
        }));

        private DelegateCommand<ItemViewModel> deleteCommand;
        public ICommand DeleteCommand => deleteCommand ?? (deleteCommand = new DelegateCommand<ItemViewModel>(async item =>
        {
            await itemActionsCreator.DeleteAsync(item.Model);
        }));

        private DelegateCommand<ItemViewModel> completeCommand;
        public ICommand CompleteCommand => completeCommand ?? (completeCommand = new DelegateCommand<ItemViewModel>(async item =>
        {
            await itemActionsCreator.CompleteAsync(item.Model);
        }));

        private DelegateCommand<ItemViewModel> showItemDetailCommand;
        public ICommand ShowItemDetailCommand => showItemDetailCommand ?? (showItemDetailCommand = new DelegateCommand<ItemViewModel>(async item => 
        {
            if (item == null)
                return;

            await navigationService.NavigateAsync("ItemDetailPage", new NavigationParameters { { "item", item } }, false, true);
        }));

        public ItemsPageViewModel(INavigationService navigationService, IPageDialogService dialogService, IDependencyService dependencyService,
                                  //Prism without Flux
                                  //IEventAggregator eventAggregator,
                                  //IItemService itemService,

                                  //Commented for testing purposes
                                  //ItemStore itemStore,
                                  ItemActionsCreator itemActionsCreator)
        {   
            //Prism without Flux 
            //this.eventAggregator = eventAggregator;
            //this.itemService = itemService;

            //Commented for testing purposes
            //this.itemStore = itemStore;

            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.itemActionsCreator = itemActionsCreator;

            Items = new ObservableCollection<ItemViewModel>();

            Title = "Browse";

            //Prism without Flux 
            //eventAggregator.GetEvent<AddItemEvent>().Subscribe(async () => 
            //{ 
            //    await LoadItemsAsync();
            //});
            //Items = new ObservableCollection<Item>();
        }

        //Actions can also come from system events
        //public async void Scanner_Scan(object sender, ScanningResult result)
        //{
        //    await itemActionsCreator.AddAsync(new Item
        //    {
        //        Text = result.Symbology,
        //        Description = result.BarcodeData
        //    });
        //}

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await itemActionsCreator.GetItemsAsync();

                //Prism without Flux
                // Items.Clear();
                //        var items = await itemService.GetItemsAsync(true);
                //        foreach (var item in items)
                //        {
                //            Items.Add(item);
                //        }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        //Arriving on the ViewModel
        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("refreshedByEvent") && parameters.GetValue<bool>("refreshedByEvent"))
                return;

            if (parameters.ContainsKey("item"))
            {
                var itemViewModel = parameters.GetValue<ItemViewModel>("item");
                if (itemViewModel != null && await dialogService.DisplayAlertAsync("Save changes", $"Do you want to save: {itemViewModel.Text} - {itemViewModel.Description} ?", "Yes", "No"))
                    await itemActionsCreator.AddAsync(itemViewModel.Model);
            }

            await LoadItemsAsync();
        }

        //Arriving on the ViewModel
        public void OnNavigatingTo(NavigationParameters parameters)
        {
            Subscribe();
        }

        //Leaving the ViewModel
        //Called when user taps the physical & virtual back buttons as well as an in-app back control
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            //itemStore.OnStateChanged += ItemStore_OnChange;
            App.ItemStore.OnStateChanged += ItemStore_OnChange;
        }

        private void Unsubscribe()
        {
            //itemStore.OnStateChanged -= ItemStore_OnChange; 
            App.ItemStore.OnStateChanged -= ItemStore_OnChange;
        }

        //Reflecting changes from our stpre to our view
        public void ItemStore_OnChange(object sender, EventArgs e)
        {
            Debug.WriteLine("Something has changed ! It will be rendered now !");

            Items.Clear();
            foreach (var item in App.ItemStore.Data)
                Items.Add(new ItemViewModel(item));
        }
    }
}