using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/effects/Buff Effect")]
    public class BuffEffect : ScriptableObject, IEffect
    {
        [SerializeField] private BuffDefinition buff;
        
        public void Apply(EffectContext context)
        {
            if (buff != null)
            {
                context.Target.AddBuff(buff);
            }
        }
    }
} 