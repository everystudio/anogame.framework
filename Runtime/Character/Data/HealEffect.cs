using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/character/SkillEffect/Heal")]
    public class HealEffect : ScriptableObject, ISkillEffect
    {
        public int amount;

        public void Apply(CharacterStatus target)
        {
            target.CurrentHP = Mathf.Min(target.CurrentHP + amount, target.MaxHP);
        }
    }
}
