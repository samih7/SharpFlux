using TodoApp.Models;
using Prism.Navigation;

namespace TodoApp.ViewModels
{
    public class ItemDetailPageViewModel : BaseViewModel, INavigationAware
	{
        private readonly INavigationService navigationService;

        private ItemViewModel item;
        public ItemViewModel Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }
        
        public ItemDetailPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("item"))
            {
                Item = parameters.GetValue<ItemViewModel>("item");
                Title = item?.Text;
            }
        }
    }
}
