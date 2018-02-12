using TodoApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp.Services.Items
{
    public interface IItemService
    {
        Task<Item> GetItemAsync(string id);
        Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false);
        Task<bool> AddAsync(Item item);
        Task<bool> UpdateAsync(Item item);
        Task<bool> RemoveAsync(Item item);
        Task<bool> CompleteAsync(Item item);
    }
}
