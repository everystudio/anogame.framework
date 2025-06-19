using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 基本ステータスを直接変更するエフェクト
    /// レベルアップや成長アイテムなどで使用
    /// </summary>
    [CreateAssetMenu(menuName = "ano/effects/Stat Modifier Effect")]
    public class StatModifierEffect : ScriptableObject, IEffect
    {
        [Header("ステータス変更設定")]
        [SerializeField] private int maxHPChange = 0;
        [SerializeField] private int maxMPChange = 0;
        [SerializeField] private int attackChange = 0;
        [SerializeField] private int defenseChange = 0;
        [SerializeField] private int speedChange = 0;
        
        [Header("変更タイプ")]
        [SerializeField] private bool isPermanent = true; // 永続的な変更かどうか
        
        public void Apply(EffectContext context)
        {
            var target = context.Target;
            var multiplier = context.Multiplier;
            
            // 各ステータスを変更
            if (maxHPChange != 0)
            {
                int change = Mathf.RoundToInt(maxHPChange * multiplier);
                target.BaseMaxHP += change;
                
                // 現在HPも比例して調整（最大HPが増えた場合）
                if (change > 0)
                {
                    target.CurrentHP += change;
                }
                
                Debug.Log($"最大HP: {change:+0;-0} (現在: {target.MaxHP})");
            }
            
            if (maxMPChange != 0)
            {
                int change = Mathf.RoundToInt(maxMPChange * multiplier);
                target.BaseMaxMP += change;
                
                if (change > 0)
                {
                    target.CurrentMP += change;
                }
                
                Debug.Log($"最大MP: {change:+0;-0} (現在: {target.MaxMP})");
            }
            
            if (attackChange != 0)
            {
                int change = Mathf.RoundToInt(attackChange * multiplier);
                target.BaseAttack += change;
                Debug.Log($"攻撃力: {change:+0;-0} (現在: {target.TotalAttack})");
            }
            
            if (defenseChange != 0)
            {
                int change = Mathf.RoundToInt(defenseChange * multiplier);
                target.BaseDefense += change;
                Debug.Log($"防御力: {change:+0;-0} (現在: {target.TotalDefense})");
            }
            
            if (speedChange != 0)
            {
                int change = Mathf.RoundToInt(speedChange * multiplier);
                target.BaseSpeed += change;
                Debug.Log($"素早さ: {change:+0;-0} (現在: {target.TotalSpeed})");
            }
        }
    }
} 