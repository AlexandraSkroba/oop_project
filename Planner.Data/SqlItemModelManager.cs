using Planner.Logic;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Planner.Data
{
    public class SqlItemModelManager : IItemModelManager
    {
        private readonly PlannerDbContext db;
        private readonly UserModel user;
        public SqlItemModelManager(PlannerDbContext db, UserModel user)
        {
            this.db = db;
            this.user = user;
        }
        public void Add(ItemModel newItem)
        {
            newItem.User = user;
            db.ItemsModel.Add(newItem);
            db.SaveChanges();
        }

        public void Delete(ItemModel itemToDelete)
        {
            var item = db.ItemsModel.SingleOrDefault(i => i.Id == itemToDelete.Id && i.User == user);

            if (item != null)
            {
                db.ItemsModel.Remove(item);
                db.SaveChanges();
            }
        }

        public int GetCountOfItems(bool state = false)
        {
            var query = (from i in db.ItemsModel
                         where i.UserId == user.Id && i.IsCompleted == state
                         select i)
                        .Count();

            return query;
        }

        public IEnumerable<ItemModel> GetItemsByNameAndState(string name = null, bool state = false)
        {
            var query = from i in db.ItemsModel
                        where i.User == user &&
                              (string.IsNullOrEmpty(name) || i.ItemName.Contains(name)) &&
                              i.IsCompleted == state
                        orderby i.DueDate.HasValue descending,  // Display items with DueDate first
                                i.DueDate                       // Display items with DueDate == null after
                        select i;

            return query;
        }

        public void Update(ItemModel updatedItem)
        {
            var userId = updatedItem.User?.Id;
            var result = db.ItemsModel.SingleOrDefault(i => i.Id == updatedItem.Id && i.UserId == userId);

            if (result != null)
            {
                result.ItemName = updatedItem.ItemName;
                result.DueDate = updatedItem.DueDate;
                result.CreatedDate = updatedItem.CreatedDate;
                result.Priority = updatedItem.Priority;
                result.IsCompleted = updatedItem.IsCompleted;

                db.SaveChanges();
            }
        }


        public IEnumerable<ItemModel> GetItemsByUserAndState(UserModel user, bool state)
        {
            IQueryable<ItemModel> query = db.ItemsModel.Where(item => item.UserId == user.Id);

            if (state)
            {
                // Фильтрация только завершенных элементов
                query = query.Where(item => item.IsCompleted);
            }
            else
            {
                // Фильтрация только активных элементов
                query = query.Where(item => !item.IsCompleted);
            }

            return query.ToList();
        }


    }
}
