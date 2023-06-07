using Planner.Data.Abstractions;
using Planner.Logic;
using System.Linq;

namespace Planner.Data
{
    public class SqlUserModelManager : IUserModelManager
    {
        private readonly PlannerDbContext db;

        public SqlUserModelManager(PlannerDbContext db)
        {
            this.db = db;
        }

        public UserModel GetUserByName(string userName)
        {
            var user = db.UsersModel.SingleOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                // Если пользователь не найден, создаем нового
                user = new UserModel
                {
                    UserName = userName
                };

                db.UsersModel.Add(user);
                db.SaveChanges();
            }

            return user;
        }
    }
}
