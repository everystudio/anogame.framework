using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 複数のステータスを同時に変更する複合エフェクト
    /// バランス調整や特殊アイテムで使用
    /// </summary>
    [CreateAssetMenu(menuName = "ano/effects/Composite Stat Effect")]
    public class CompositeStatEffect : ScriptableObject, IEffect
    {
        [System.Serializable]
        public struct StatChange
        {
            public StatusType statType;
            public int changeAmount;
            public bool isPercentage; // 割合ベースかどうか
        }
        
        [Header("ステータス変更設定")]
        [SerializeField] private StatChange[] statChanges;
        
        [Header("HP/MP変更設定")]
        [SerializeField] private int currentHPChange = 0;
        [SerializeField] private int currentMPChange = 0;
        
        [Header("説明")]
        [SerializeField, TextArea(2, 4)] private string description = "複数のステータスを同時に変更します";
        
        public void Apply(EffectContext context)
        {
            var target = context.Target;
            var multiplier = context.Multiplier;
            
            Debug.Log($"複合ステータスエフェクト適用: {description}");
            
            // ステータス変更を適用
            foreach (var change in statChanges)
            {
                ApplyStatChange(target, change, multiplier);
            }
            
            // 現在HP/MP変更
            if (currentHPChange != 0)
            {
                int change = Mathf.RoundToInt(currentHPChange * multiplier);
                target.CurrentHP += change;
                target.CurrentHP = Mathf.Clamp(target.CurrentHP, 0, target.MaxHP);
                Debug.Log($"現在HP: {change:+0;-0} (現在: {target.CurrentHP}/{target.MaxHP})");
            }
            
            if (currentMPChange != 0)
            {
                int change = Mathf.RoundToInt(currentMPChange * multiplier);
                target.CurrentMP += change;
                target.CurrentMP = Mathf.Clamp(target.CurrentMP, 0, target.MaxMP);
                Debug.Log($"現在MP: {change:+0;-0} (現在: {target.CurrentMP}/{target.MaxMP})");
            }
        }
        
        private void ApplyStatChange(CharacterStatus target, StatChange change, float multiplier)
        {
            if (change.isPercentage)
            {
                // 割合ベースの変更
                ApplyPercentageChange(target, change, multiplier);
            }
            else
            {
                // 固定値ベースの変更
                ApplyFixedChange(target, change, multiplier);
            }
        }
        
        private void ApplyFixedChange(CharacterStatus target, StatChange change, float multiplier)
        {
            int changeAmount = Mathf.RoundToInt(change.changeAmount * multiplier);
            
            switch (change.statType)
            {
                case StatusType.HP:
                    target.BaseMaxHP += changeAmount;
                    if (changeAmount > 0) target.CurrentHP += changeAmount;
                    Debug.Log($"最大HP: {changeAmount:+0;-0} (現在: {target.MaxHP})");
                    break;
                    
                case StatusType.MP:
                    target.BaseMaxMP += changeAmount;
                    if (changeAmount > 0) target.CurrentMP += changeAmount;
                    Debug.Log($"最大MP: {changeAmount:+0;-0} (現在: {target.MaxMP})");
                    break;
                    
                case StatusType.Attack:
                    target.BaseAttack += changeAmount;
                    Debug.Log($"攻撃力: {changeAmount:+0;-0} (現在: {target.TotalAttack})");
                    break;
                    
                case StatusType.Defense:
                    target.BaseDefense += changeAmount;
                    Debug.Log($"防御力: {changeAmount:+0;-0} (現在: {target.TotalDefense})");
                    break;
                    
                case StatusType.Speed:
                    target.BaseSpeed += changeAmount;
                    Debug.Log($"素早さ: {changeAmount:+0;-0} (現在: {target.TotalSpeed})");
                    break;
            }
        }
        
        private void ApplyPercentageChange(CharacterStatus target, StatChange change, float multiplier)
        {
            float percentage = change.changeAmount * multiplier;
            
            switch (change.statType)
            {
                case StatusType.HP:
                    int hpChange = Mathf.RoundToInt(target.BaseMaxHP * (percentage / 100f));
                    target.BaseMaxHP += hpChange;
                    if (hpChange > 0) target.CurrentHP += hpChange;
                    Debug.Log($"最大HP {percentage:+0;-0}%: {hpChange:+0;-0} (現在: {target.MaxHP})");
                    break;
                    
                case StatusType.MP:
                    int mpChange = Mathf.RoundToInt(target.BaseMaxMP * (percentage / 100f));
                    target.BaseMaxMP += mpChange;
                    if (mpChange > 0) target.CurrentMP += mpChange;
                    Debug.Log($"最大MP {percentage:+0;-0}%: {mpChange:+0;-0} (現在: {target.MaxMP})");
                    break;
                    
                case StatusType.Attack:
                    int attackChange = Mathf.RoundToInt(target.BaseAttack * (percentage / 100f));
                    target.BaseAttack += attackChange;
                    Debug.Log($"攻撃力 {percentage:+0;-0}%: {attackChange:+0;-0} (現在: {target.TotalAttack})");
                    break;
                    
                case StatusType.Defense:
                    int defenseChange = Mathf.RoundToInt(target.BaseDefense * (percentage / 100f));
                    target.BaseDefense += defenseChange;
                    Debug.Log($"防御力 {percentage:+0;-0}%: {defenseChange:+0;-0} (現在: {target.TotalDefense})");
                    break;
                    
                case StatusType.Speed:
                    int speedChange = Mathf.RoundToInt(target.BaseSpeed * (percentage / 100f));
                    target.BaseSpeed += speedChange;
                    Debug.Log($"素早さ {percentage:+0;-0}%: {speedChange:+0;-0} (現在: {target.TotalSpeed})");
                    break;
            }
        }
    }
} 