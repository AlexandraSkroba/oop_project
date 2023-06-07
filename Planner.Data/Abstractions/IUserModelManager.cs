
using Planner.Logic;

namespace Planner.Data.Abstractions
{
    public interface IUserModelManager
    {
        UserModel GetUserByName(string userName);
    }
}
