using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/character/SkillEffect/Damage")]
    public class DamageEffect : ScriptableObject, ISkillEffect
    {
        public int amount;

        public void Apply(CharacterStatus target)
        {
            target.CurrentHP -= amount;
        }
    }
}
