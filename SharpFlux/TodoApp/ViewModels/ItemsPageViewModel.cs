using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoApp.Flux.Actions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SharpFlux;
using TodoApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.ViewModels
{
    public class ItemsPageViewModel : BaseViewModel, INavigationAware
    {
        private readonly INavigationService navigationService;
        private readonly IPageDialogService dialogService;
        private readonly ItemActionsCreator itemActionsCreator;
        //Commented for testing purposes
        //private readonly ItemStore itemStore;

        private ObservableCollection<ItemViewModel> items;
        public ObservableCollection<ItemViewModel> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }


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

        public ItemsPageViewModel(INavigationService navigationService, IPageDialogService dialogService,
                                  //Commented for testing purposes
                                  //ItemStore itemStore,
                                  ItemActionsCreator itemActionsCreator)
        {   
            //Commented for testing purposes
            //this.itemStore = itemStore;

            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.itemActionsCreator = itemActionsCreator;

            Items = new ObservableCollection<ItemViewModel>();

            Title = "Browse";
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
            App.ItemStore.ItemUpserted += ItemStore_ItemUpserted;
            App.ItemStore.ItemRemoved += ItemStore_ItemRemoved;
            App.ItemStore.ItemsFetched += ItemStore_ItemsFetched;
        }

        private void Unsubscribe()
        {
            App.ItemStore.ItemUpserted -= ItemStore_ItemUpserted;
            App.ItemStore.ItemRemoved -= ItemStore_ItemRemoved;
            App.ItemStore.ItemsFetched -= ItemStore_ItemsFetched;
        }

        //Reflecting changes from our stpre to our view
        public void ItemStore_ItemUpserted(object sender, EventArgs e)
        {
            var item = (e as DataEventArgs<Item>)?.Data;
            if (item == null)
                return;
            var existingItem = Items.FirstOrDefault(t => t.Id == item.Id);
            if (existingItem == null)
            {
                Items.Add(new ItemViewModel(item));
                return;
            }
            var itemIndex = Items.IndexOf(existingItem);

            Items.RemoveAt(itemIndex);
            Items.Insert(itemIndex, new ItemViewModel(item));
        }
        public void ItemStore_ItemRemoved(object sender, EventArgs e)
        {
            var item = (e as DataEventArgs<Item>)?.Data;
            if (item == null)
                return;
            var existingItem = Items.FirstOrDefault(t => t.Id == item.Id);
            if (existingItem == null)
                return;
            Items.Remove(existingItem);
        }
        public void ItemStore_ItemsFetched(object sender, EventArgs e)
        {
            Items = new ObservableCollection<ItemViewModel>(App.ItemStore.Data.Select(x => new ItemViewModel(x)));
        }
    }
}