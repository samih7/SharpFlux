using System.Windows.Input;
using Xamarin.Forms;

namespace TodoApp.Controls
{
    //Item selected cleared when coming back on the ListView page (from having navigated to another page).
    public class SelectedItemClearedListView : ListView
    {
        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(object), typeof(SelectedItemClearedListView), null, BindingMode.Default);
        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        public SelectedItemClearedListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            ItemTapped += OnItemTapped;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || ItemSelectedCommand == null)
                return;

            ItemSelectedCommand.Execute(e.Item);
            SelectedItem = null;
        }
    }
}
