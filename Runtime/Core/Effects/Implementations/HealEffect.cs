using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/effects/Heal Effect")]
    public class HealEffect : ScriptableObject, IEffect
    {
        [SerializeField] private int amount = 20;
        
        public void Apply(EffectContext context)
        {
            var healing = Mathf.RoundToInt(amount * context.Multiplier);
            context.Target.CurrentHP += healing;
            
            // MaxHPを超えないようにクランプ
            context.Target.CurrentHP = Mathf.Min(context.Target.CurrentHP, context.Target.MaxHP);
        }
    }
} 