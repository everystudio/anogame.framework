// Packages/anogame.framework/Runtime/Data/BuffDefinition.cs
using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/character/Buff Definition")]
    public class BuffDefinition : ScriptableObject
    {
        [Header("基本情報")]
        public string Id;
        public StatusType TargetStat;
        public int Amount;
        public int DurationTurns;
        public Sprite IconSprite;

        [Header("重複時の挙動")]
        public BuffMergePolicy MergePolicy = BuffMergePolicy.Override;

    }
}
