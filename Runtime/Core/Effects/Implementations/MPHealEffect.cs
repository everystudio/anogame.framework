using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// MPを回復するエフェクト
    /// 魔法薬やマナポーションなどで使用
    /// </summary>
    [CreateAssetMenu(menuName = "ano/effects/MP Heal Effect")]
    public class MPHealEffect : ScriptableObject, IEffect
    {
        [SerializeField] private int amount = 10;
        [SerializeField] private bool canHealAboveMax = false; // 最大値を超えて回復可能かどうか
        
        public void Apply(EffectContext context)
        {
            var healing = Mathf.RoundToInt(amount * context.Multiplier);
            var target = context.Target;
            
            int beforeMP = target.CurrentMP;
            target.CurrentMP += healing;
            
            // 設定に応じてクランプ
            if (!canHealAboveMax)
            {
                target.CurrentMP = Mathf.Min(target.CurrentMP, target.MaxMP);
            }
            
            int actualHealing = target.CurrentMP - beforeMP;
            Debug.Log($"MP回復: {actualHealing} (現在MP: {target.CurrentMP}/{target.MaxMP})");
        }
    }
} 