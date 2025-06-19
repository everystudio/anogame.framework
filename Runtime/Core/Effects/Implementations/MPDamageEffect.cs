using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// MPにダメージを与えるエフェクト
    /// 魔法封じや精神攻撃などで使用
    /// </summary>
    [CreateAssetMenu(menuName = "ano/effects/MP Damage Effect")]
    public class MPDamageEffect : ScriptableObject, IEffect
    {
        [SerializeField] private int amount = 5;
        [SerializeField] private bool canReduceBelowZero = false; // マイナス値になるかどうか
        
        public void Apply(EffectContext context)
        {
            var damage = Mathf.RoundToInt(amount * context.Multiplier);
            var target = context.Target;
            
            int beforeMP = target.CurrentMP;
            target.CurrentMP -= damage;
            
            // 設定に応じてクランプ
            if (!canReduceBelowZero)
            {
                target.CurrentMP = Mathf.Max(0, target.CurrentMP);
            }
            
            int actualDamage = beforeMP - target.CurrentMP;
            Debug.Log($"MPダメージ: {actualDamage} (現在MP: {target.CurrentMP}/{target.MaxMP})");
        }
    }
} 