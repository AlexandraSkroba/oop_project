using System.Collections.Generic;

namespace Planner.Logic
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ICollection<ItemModel> Items { get; set; }
    }
}
