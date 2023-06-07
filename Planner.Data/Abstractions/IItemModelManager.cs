using System.Collections.Generic;
using Planner.Logic;

namespace Planner.Data
{
    public interface IItemModelManager
    {
        IEnumerable<ItemModel> GetItemsByNameAndState(string name = null, bool state = false);
        IEnumerable<ItemModel> GetItemsByUserAndState(UserModel user, bool state);
        void Add(ItemModel newItem);
        void Update(ItemModel updatedItem);
        void Delete(ItemModel itemToDelete);
        int GetCountOfItems(bool state = false);
    }
}
