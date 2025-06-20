using UnityEngine;

namespace anogame.framework
{
    [CreateAssetMenu(menuName = "ano/inventory/Consumable Item")]
    public class ConsumableItem : ItemDefinition, IEffectSource
    {
        [Header("消費アイテム設定")]
        [SerializeField] private IEffect[] effects;
        
        [Header("使用制限")]
        [SerializeField] private bool canUseInBattle = true;
        [SerializeField] private bool canUseOutOfBattle = true;
        
        /// <summary>
        /// このアイテムが持つエフェクトを取得
        /// </summary>
        /// <returns>エフェクトの配列</returns>
        public IEffect[] GetEffects() => effects;
        
        /// <summary>
        /// バトル中に使用可能かどうか
        /// </summary>
        public bool CanUseInBattle => canUseInBattle;
        
        /// <summary>
        /// バトル外で使用可能かどうか
        /// </summary>
        public bool CanUseOutOfBattle => canUseOutOfBattle;
        
        /// <summary>
        /// アイテムを使用する
        /// </summary>
        /// <param name="target">対象キャラクター</param>
        /// <param name="source">使用者</param>
        /// <returns>使用に成功したかどうか</returns>
        public bool Use(CharacterStatus target, CharacterStatus source = null)
        {
            if (effects == null || effects.Length == 0)
            {
                Debug.LogWarning($"アイテム '{DisplayName}' にエフェクトが設定されていません");
                return false;
            }
            
            // エフェクトを適用
            EffectUtils.ApplyEffects(effects, target, source);
            return true;
        }
    }
}
