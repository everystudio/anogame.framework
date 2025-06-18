using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/character/SkillEffect/Buff")]
    public class BuffEffect : ScriptableObject, ISkillEffect
    {
        public BuffDefinition buff;

        public void Apply(CharacterStatus target)
        {
            if (buff != null)
            {
                target.AddBuff(buff);
            }
        }
    }
}
