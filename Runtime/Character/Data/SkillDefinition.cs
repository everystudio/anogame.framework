// Packages/anogame.framework/Runtime/Data/SkillDefinition.cs
using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/character/Skill Definition")]
    public class SkillDefinition : ScriptableObject, ISkillEffectSource
    {
        public string SkillName;
        public ISkillEffect[] Effects;

        public ISkillEffect[] GetEffects() => Effects;
    }
}
