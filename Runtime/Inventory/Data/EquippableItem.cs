using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/equipment/Equippable Item")]
    public class EquippableItem : ItemDefinition
    {
        public EquipmentSlotType SlotType;
        public int AttackBonus;
        public int DefenseBonus;
        public int SpeedBonus;
    }
}
