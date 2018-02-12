using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace TodoApp.ViewModels
{
    public class AboutPageViewModel : BaseViewModel, INavigationAware
    {
        public AboutPageViewModel()
        {
            Title = "About";

            OpenWebCommand = new DelegateCommand(() => 
            {
                Device.OpenUri(new Uri("https://xamarin.com/platform"));
            });
        }

        public ICommand OpenWebCommand { get; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}