using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Logic
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }
        public UserModel User { get; set; }
        public int? UserId { get; set; }
        public IEnumerable<string> States { get; }
        public IEnumerable<string> Priorities { get; }

        public ItemModel()
        {
            States = new List<string>()
            {
                "Show Active",
                "Show Completed"
            };
            Priorities = new List<string>()
            {
                "High",
                "Medium",
                "Low"
            };
        }
    }
}
