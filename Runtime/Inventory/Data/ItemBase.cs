// Packages/anogame.framework/Runtime/Data/ItemBase.cs
using UnityEngine;

namespace anogame.framework
{
    public abstract class ItemBase : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;
        public bool IsStackable;
    }
}
