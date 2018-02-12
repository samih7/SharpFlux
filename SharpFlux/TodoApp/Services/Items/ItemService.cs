using TodoApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp.Services.Items
{
    public class ItemService : IItemService
    {
        public Task<Item> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddAsync(Item item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(Item item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Item item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CompleteAsync(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
