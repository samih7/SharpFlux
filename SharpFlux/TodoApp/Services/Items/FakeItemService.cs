using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services.Items
{
    public class FakeItemService : IItemService
    {
        private readonly IList<Item> items;

        public FakeItemService()
        {
            items = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." },
            };
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<bool> AddAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync(Item item)
        {
            var itemToUpdate = items.FirstOrDefault((Item arg) => arg.Id == item.Id);
            items.Remove(itemToUpdate);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> CompleteAsync(Item item)
        {
            var itemToSwitch = items.FirstOrDefault((Item arg) => arg.Id == item.Id);
            items.Remove(itemToSwitch);
            item.IsComplete = !item.IsComplete;
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveAsync(Item item)
        {
            var itemRetrieved = items.FirstOrDefault((Item arg) => arg.Id == item.Id);
            items.Remove(itemRetrieved);

            return await Task.FromResult(true);
        }
    }
}
