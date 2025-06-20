using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 新しいエフェクトシステムの使用例
    /// このクラスは例示用のみで、実際のゲームでは削除してください
    /// </summary>
    public class EffectSystemExample : MonoBehaviour
    {
        [Header("基本エフェクト")]
        [SerializeField] private DamageEffect damageEffect;
        [SerializeField] private HealEffect healEffect;
        [SerializeField] private BuffEffect buffEffect;
        
        [Header("新しいステータス変更エフェクト")]
        [SerializeField] private StatModifierEffect statModifierEffect;
        [SerializeField] private PercentageStatEffect percentageStatEffect;
        [SerializeField] private CompositeStatEffect compositeStatEffect;
        
        [Header("MP関連エフェクト")]
        [SerializeField] private MPDamageEffect mpDamageEffect;
        [SerializeField] private MPHealEffect mpHealEffect;
        
        [Header("テスト用スキル")]
        [SerializeField] private SkillDefinition testSkill;
        
        [Header("テスト用アイテム")]
        [SerializeField] private ConsumableItem healingPotion;
        
        void Start()
        {
            // テスト用キャラクターステータスを作成
            var playerStatus = new CharacterStatus();
            var enemyStatus = new CharacterStatus();
            
            Debug.Log("=== エフェクトシステムテスト開始 ===");
            
            // 1. 直接エフェクトを適用
            TestDirectEffects(playerStatus, enemyStatus);
            
            // 2. スキルからエフェクトを適用
            TestSkillEffects(playerStatus, enemyStatus);
            
            // 3. アイテムからエフェクトを適用
            TestItemEffects(playerStatus);
            
            // 4. 新しいステータス変更エフェクトのテスト
            TestStatModificationEffects(playerStatus);
            
            // 5. MP関連エフェクトのテスト
            TestMPEffects(playerStatus);
            
            Debug.Log("=== エフェクトシステムテスト完了 ===");
        }
        
        void TestDirectEffects(CharacterStatus player, CharacterStatus enemy)
        {
            Debug.Log("--- 直接エフェクト適用テスト ---");
            
            // ダメージエフェクトを敵に適用
            if (damageEffect != null)
            {
                var initialHP = enemy.CurrentHP;
                EffectUtils.ApplyEffect(damageEffect, enemy, player);
                Debug.Log($"ダメージエフェクト: {initialHP} → {enemy.CurrentHP}");
            }
            
            // ヒールエフェクトをプレイヤーに適用
            if (healEffect != null)
            {
                player.CurrentHP = 50; // 一度HPを減らす
                var initialHP = player.CurrentHP;
                EffectUtils.ApplyEffect(healEffect, player);
                Debug.Log($"ヒールエフェクト: {initialHP} → {player.CurrentHP}");
            }
        }
        
        void TestSkillEffects(CharacterStatus player, CharacterStatus enemy)
        {
            Debug.Log("--- スキルエフェクト適用テスト ---");
            
            if (testSkill != null)
            {
                var context = new EffectContext(enemy, player, 1.5f); // 1.5倍のダメージ
                EffectUtils.ApplyEffects(testSkill, context);
                Debug.Log($"スキル '{testSkill.SkillName}' を使用");
            }
        }
        
        void TestItemEffects(CharacterStatus player)
        {
            Debug.Log("--- アイテムエフェクト適用テスト ---");
            
            if (healingPotion != null)
            {
                player.CurrentHP = 30; // HPを減らす
                var initialHP = player.CurrentHP;
                healingPotion.Use(player);
                Debug.Log($"アイテム '{healingPotion.DisplayName}' 使用: {initialHP} → {player.CurrentHP}");
            }
        }
        
        void TestStatModificationEffects(CharacterStatus player)
        {
            Debug.Log("--- ステータス変更エフェクトテスト ---");
            
            // 基本ステータス変更
            if (statModifierEffect != null)
            {
                Debug.Log($"変更前 - HP: {player.MaxHP}, 攻撃: {player.TotalAttack}, 防御: {player.TotalDefense}");
                EffectUtils.ApplyEffect(statModifierEffect, player);
                Debug.Log($"変更後 - HP: {player.MaxHP}, 攻撃: {player.TotalAttack}, 防御: {player.TotalDefense}");
            }
            
            // 割合ベースステータス変更
            if (percentageStatEffect != null)
            {
                Debug.Log($"割合変更前 - HP: {player.CurrentHP}/{player.MaxHP}");
                EffectUtils.ApplyEffect(percentageStatEffect, player);
                Debug.Log($"割合変更後 - HP: {player.CurrentHP}/{player.MaxHP}");
            }
            
            // 複合ステータス変更
            if (compositeStatEffect != null)
            {
                Debug.Log($"複合変更前 - 攻撃: {player.TotalAttack}, 防御: {player.TotalDefense}");
                EffectUtils.ApplyEffect(compositeStatEffect, player);
                Debug.Log($"複合変更後 - 攻撃: {player.TotalAttack}, 防御: {player.TotalDefense}");
            }
        }
        
        void TestMPEffects(CharacterStatus player)
        {
            Debug.Log("--- MP関連エフェクトテスト ---");
            
            // MPダメージ
            if (mpDamageEffect != null)
            {
                Debug.Log($"MPダメージ前: {player.CurrentMP}/{player.MaxMP}");
                EffectUtils.ApplyEffect(mpDamageEffect, player);
                Debug.Log($"MPダメージ後: {player.CurrentMP}/{player.MaxMP}");
            }
            
            // MP回復
            if (mpHealEffect != null)
            {
                Debug.Log($"MP回復前: {player.CurrentMP}/{player.MaxMP}");
                EffectUtils.ApplyEffect(mpHealEffect, player);
                Debug.Log($"MP回復後: {player.CurrentMP}/{player.MaxMP}");
            }
        }
    }
} 