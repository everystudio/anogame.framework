// Packages/anogame.framework/Runtime/Model/InventoryModel.cs
using System.Collections.Generic;
using System.Linq;

namespace anogame.framework
{
    public class InventoryModel
    {
        private readonly List<InventoryItem> _items = new();

        public IReadOnlyList<InventoryItem> Items => _items;

        public void Add(ItemDefinition item, int amount = 1)
        {
            if (item.IsStackable)
            {
                var existing = _items.FirstOrDefault(i => i.Item == item);
                if (existing != null)
                {
                    existing.Amount += amount;
                    return;
                }
            }

            _items.Add(new InventoryItem(item, amount));
        }

        public bool Remove(ItemDefinition item, int amount = 1)
        {
            var entry = _items.FirstOrDefault(i => i.Item == item);
            if (entry == null)
                return false;

            if (entry.Item.IsStackable)
            {
                entry.Amount -= amount;
                if (entry.Amount <= 0)
                    _items.Remove(entry);
            }
            else
            {
                _items.Remove(entry);
            }

            return true;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
