using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 割合ベースでステータスを変更するエフェクト
    /// 現在値に対する割合で変更を適用
    /// </summary>
    [CreateAssetMenu(menuName = "ano/effects/Percentage Stat Effect")]
    public class PercentageStatEffect : ScriptableObject, IEffect
    {
        [Header("割合変更設定 (%)")]
        [SerializeField, Range(-100f, 500f)] private float hpPercentage = 0f;
        [SerializeField, Range(-100f, 500f)] private float mpPercentage = 0f;
        [SerializeField, Range(-100f, 500f)] private float attackPercentage = 0f;
        [SerializeField, Range(-100f, 500f)] private float defensePercentage = 0f;
        [SerializeField, Range(-100f, 500f)] private float speedPercentage = 0f;
        
        [Header("適用方法")]
        [SerializeField] private bool affectCurrentHP = true; // 現在HPに影響するか
        [SerializeField] private bool affectCurrentMP = true; // 現在MPに影響するか
        [SerializeField] private bool affectBaseStats = false; // 基本ステータスに影響するか
        
        public void Apply(EffectContext context)
        {
            var target = context.Target;
            var multiplier = context.Multiplier;
            
            // HP変更
            if (hpPercentage != 0f)
            {
                if (affectCurrentHP)
                {
                    float change = target.MaxHP * (hpPercentage / 100f) * multiplier;
                    target.CurrentHP += Mathf.RoundToInt(change);
                    target.CurrentHP = Mathf.Clamp(target.CurrentHP, 0, target.MaxHP);
                    Debug.Log($"HP {hpPercentage:+0;-0}%: {change:+0;-0} (現在: {target.CurrentHP}/{target.MaxHP})");
                }
                
                if (affectBaseStats)
                {
                    float change = target.BaseMaxHP * (hpPercentage / 100f) * multiplier;
                    target.BaseMaxHP += Mathf.RoundToInt(change);
                    Debug.Log($"最大HP {hpPercentage:+0;-0}%: {change:+0;-0} (現在: {target.MaxHP})");
                }
            }
            
            // MP変更
            if (mpPercentage != 0f)
            {
                if (affectCurrentMP)
                {
                    float change = target.MaxMP * (mpPercentage / 100f) * multiplier;
                    target.CurrentMP += Mathf.RoundToInt(change);
                    target.CurrentMP = Mathf.Clamp(target.CurrentMP, 0, target.MaxMP);
                    Debug.Log($"MP {mpPercentage:+0;-0}%: {change:+0;-0} (現在: {target.CurrentMP}/{target.MaxMP})");
                }
                
                if (affectBaseStats)
                {
                    float change = target.BaseMaxMP * (mpPercentage / 100f) * multiplier;
                    target.BaseMaxMP += Mathf.RoundToInt(change);
                    Debug.Log($"最大MP {mpPercentage:+0;-0}%: {change:+0;-0} (現在: {target.MaxMP})");
                }
            }
            
            // 攻撃力変更
            if (attackPercentage != 0f && affectBaseStats)
            {
                float change = target.BaseAttack * (attackPercentage / 100f) * multiplier;
                target.BaseAttack += Mathf.RoundToInt(change);
                Debug.Log($"攻撃力 {attackPercentage:+0;-0}%: {change:+0;-0} (現在: {target.TotalAttack})");
            }
            
            // 防御力変更
            if (defensePercentage != 0f && affectBaseStats)
            {
                float change = target.BaseDefense * (defensePercentage / 100f) * multiplier;
                target.BaseDefense += Mathf.RoundToInt(change);
                Debug.Log($"防御力 {defensePercentage:+0;-0}%: {change:+0;-0} (現在: {target.TotalDefense})");
            }
            
            // 素早さ変更
            if (speedPercentage != 0f && affectBaseStats)
            {
                float change = target.BaseSpeed * (speedPercentage / 100f) * multiplier;
                target.BaseSpeed += Mathf.RoundToInt(change);
                Debug.Log($"素早さ {speedPercentage:+0;-0}%: {change:+0;-0} (現在: {target.TotalSpeed})");
            }
        }
    }
} 