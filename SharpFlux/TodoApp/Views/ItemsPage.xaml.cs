﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
        public ItemsPage()
        {
            InitializeComponent();
        }
    }
}