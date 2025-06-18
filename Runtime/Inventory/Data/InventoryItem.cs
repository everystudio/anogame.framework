// Packages/anogame.framework/Runtime/Data/InventoryItem.cs
using System;

namespace anogame.framework
{
    [Serializable]
    public class InventoryItem
    {
        public ItemDefinition Item;
        public int Amount;

        public InventoryItem(ItemDefinition item, int amount = 1)
        {
            Item = item;
            Amount = amount;
        }
    }
}
