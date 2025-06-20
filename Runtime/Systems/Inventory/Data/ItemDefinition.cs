// Packages/anogame.framework/Runtime/Data/ItemDefinition.cs
using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/inventory/Item Definition")]
    public class ItemDefinition : ItemBase
    {
        public int MaxStack = 99;
        public ItemType ItemType = ItemType.Consumable;
        // 今後、カテゴリや効果なども追加可能
    }
}
