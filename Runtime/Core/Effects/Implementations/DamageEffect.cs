using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/effects/Damage Effect")]
    public class DamageEffect : ScriptableObject, IEffect
    {
        [SerializeField] private int amount = 10;
        
        public void Apply(EffectContext context)
        {
            var damage = Mathf.RoundToInt(amount * context.Multiplier);
            context.Target.CurrentHP -= damage;
            
            // HPが0未満にならないようにクランプ
            context.Target.CurrentHP = Mathf.Max(0, context.Target.CurrentHP);
        }
    }
} 