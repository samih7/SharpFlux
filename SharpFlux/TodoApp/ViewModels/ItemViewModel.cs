using TodoApp.Models;
using Prism.Mvvm;

namespace TodoApp.ViewModels
{
    public class ItemViewModel : BindableBase
    {
        private string id;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private bool isComplete;
        public bool IsComplete
        {
            get { return isComplete; }
            set { SetProperty(ref isComplete, value); }
        }

        //Get model current state by taking out ViewModel's bindable data (changed by user)
        //Allows to persist model's data
        public Item Model => new Item { Id = Id, Text = Text, Description = Description, IsComplete = IsComplete };

        public ItemViewModel() { }

        //Allows ViewModel to be set from Model so it can be rendered on the View when a change comes from Stores
        public ItemViewModel(Item item)
        {
            Id = item.Id;
            Text = item.Text;
            Description = item.Description;
            IsComplete = item.IsComplete;
        }
    }
}
