using TodoApp.Flux.Stores;
using TodoApp.Services.Items;
using TodoApp.Views;
using Prism.Ioc;
using Prism.Unity;
using SharpFlux.Dispatching;
using SharpFlux.Stores;
using TodoApp.Models;
using System.Collections.Generic;

namespace TodoApp
{
    public partial class App : PrismApplication
    {
        public bool UseMock { get; set; }

        private static IDispatcher dispatcher = null;
        public static IDispatcher Dispatcher => dispatcher ?? (dispatcher = new Dispatcher());

        private static IStore<IList<Item>> itemStore = null;
        public static IStore<IList<Item>> ItemStore => itemStore ?? (itemStore = new ItemStore(Dispatcher));

        private static IStore<IList<Item>> otherStore = null;
        public static IStore<IList<Item>> OtherStore => otherStore ?? (otherStore = new OtherStore(Dispatcher));

        protected override async void OnInitialized()
        {
            InitializeComponent();

            //Not lazy for testing Dispatcher.WaitFor() method
            //We want to make sure ItemStore is subscribed first to the Dispatcher callback's list
            //So should be executed first but has to WaitFor() OtherStore to finish
            itemStore = new ItemStore(Dispatcher);
            otherStore = new OtherStore(Dispatcher);

            await NavigationService.NavigateAsync("MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<ItemsPage>();
            containerRegistry.RegisterForNavigation<AboutPage>();
            containerRegistry.RegisterForNavigation<ItemDetailPage>();
            containerRegistry.RegisterForNavigation<NewItemPage>();

            //Commented for testing purposes
            //containerRegistry.RegisterSingleton<Dispatcher>();
            //containerRegistry.RegisterSingleton<ItemStore>();
            //containerRegistry.RegisterSingleton<OtherStore>();

#if DEBUG
            UseMock = true;
#endif

            if (UseMock)
            {
                containerRegistry.RegisterSingleton<IItemService, FakeItemService>();
            }
            else
            {
                containerRegistry.RegisterSingleton<IItemService, ItemService>();
            }
        }

        //Uncomment & adapt if your ViewModels are in a separate assembly
        //protected override void ConfigureViewModelLocator()
        //{
        //    base.ConfigureViewModelLocator();

        //    ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(
        //        viewType =>
        //        {
        //            var viewName = viewType.FullName;
        //            viewName = viewName.Replace(".Views.", ".ViewModels.").Replace(".Clients.", ".Logic.");
        //            var viewAssemblyName = viewType.Assembly.FullName.Replace(".Clients", ".Logic");
        //            var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
        //            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
        //            var type = Type.GetType(viewModelName);
        //            return type;
        //        });
        //}
    }
}