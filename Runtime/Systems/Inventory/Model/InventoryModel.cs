// Packages/anogame.framework/Runtime/Model/InventoryModel.cs
using System.Collections.Generic;
using System.Linq;

namespace anogame.framework
{
    public class InventoryModel
    {
        private readonly List<InventoryItem> items = new();

        public IReadOnlyList<InventoryItem> Items => items;

        public void AddItem(ItemDefinition item, int amount = 1)
        {
            if (item == null) return;

            var existing = items.FirstOrDefault(i => i.Item == item);
            if (existing != null)
            {
                // 既存アイテムの数量を増加
                existing.Amount += amount;
            }
            else
            {
                // 新しいアイテムを追加
                items.Add(new InventoryItem(item, amount));
            }
        }

        public bool RemoveItem(ItemDefinition item, int amount = 1)
        {
            var entry = items.FirstOrDefault(i => i.Item == item);
            if (entry == null || entry.Amount < amount)
                return false;

            entry.Amount -= amount;
            if (entry.Amount <= 0)
            {
                items.Remove(entry);
            }
            else
            {
                items.Remove(entry);
            }

            return true;
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}
