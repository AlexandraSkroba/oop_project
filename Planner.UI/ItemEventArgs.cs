using System;
using Planner.Logic;

namespace Planner.UI
{
    public class ItemEventArgs : EventArgs
    {
        public ItemModel Item { get; }

        public ItemEventArgs(ItemModel item)
        {
            Item = item;
        }
    }
}
