using System.Collections.Generic;

namespace anogame.framework
{
    public class EquipmentModel
    {
        private readonly Dictionary<EquipmentSlotType, EquippableItem> equipped = new();

        public IReadOnlyDictionary<EquipmentSlotType, EquippableItem> Equipped => equipped;

        public void Equip(EquippableItem item)
        {
            equipped[item.SlotType] = item;
        }

        public EquippableItem GetEquippedItem(EquipmentSlotType slot)
        {
            equipped.TryGetValue(slot, out var item);
            return item;
        }

        public void Unequip(EquipmentSlotType slot)
        {
            equipped.Remove(slot);
        }

        public void ClearAll()
        {
            equipped.Clear();
        }
    }
}
