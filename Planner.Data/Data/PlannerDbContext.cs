using System.Data.Entity;
using Planner.Logic;

namespace Planner.Data
{
    public class PlannerDbContext : DbContext
    {
        public PlannerDbContext()
            : base("name=PlannerDbContext")
        {
        }
        public DbSet<ItemModel> ItemsModel { get; set; }
        public DbSet<UserModel> UsersModel { get; set; }
    }
}
