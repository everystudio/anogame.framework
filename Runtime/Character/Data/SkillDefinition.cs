// Packages/anogame.framework/Runtime/Data/SkillDefinition.cs
using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/character/Skill Definition")]
    public class SkillDefinition : ScriptableObject, IEffectSource
    {
        public string SkillName;
        public IEffect[] Effects;

        public IEffect[] GetEffects() => Effects;
    }
}
