using System.Collections.Generic;
using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 戦闘参加者の種類
    /// </summary>
    public enum ParticipantType
    {
        Player,
        Enemy
    }

    /// <summary>
    /// 戦闘参加者を表すクラス
    /// </summary>
    public class BattleParticipant
    {
        public ParticipantType Type { get; private set; }
        public CharacterStatus CharacterStatus { get; private set; }
        public string Name { get; set; }
        public bool IsAlive => CharacterStatus.CurrentHP > 0;
        
        // 利用可能なスキル
        public List<SkillDefinition> AvailableSkills { get; private set; } = new();
        
        // 戦闘中の一時的な効果
        public bool IsGuarding { get; set; }
        public bool HasActedThisTurn { get; set; }
        
        // AI用（敵キャラクター）
        public IBattleAI AI { get; set; }
        
        public BattleParticipant(ParticipantType type, CharacterStatus status, string name = null)
        {
            Type = type;
            CharacterStatus = status;
            Name = name ?? (type == ParticipantType.Player ? "プレイヤー" : "敵");
        }
        
        /// <summary>
        /// スキルを追加
        /// </summary>
        public void AddSkill(SkillDefinition skill)
        {
            if (!AvailableSkills.Contains(skill))
            {
                AvailableSkills.Add(skill);
            }
        }
        
        /// <summary>
        /// ダメージを受ける
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (IsGuarding)
            {
                damage = Mathf.RoundToInt(damage * 0.5f); // 防御時は半減
            }
            
            CharacterStatus.CurrentHP = Mathf.Max(0, CharacterStatus.CurrentHP - damage);
            Debug.Log($"{Name} が {damage} のダメージを受けた！ (残りHP: {CharacterStatus.CurrentHP})");
        }
        
        /// <summary>
        /// HPを回復
        /// </summary>
        public void Heal(int amount)
        {
            CharacterStatus.CurrentHP = Mathf.Min(CharacterStatus.MaxHP, CharacterStatus.CurrentHP + amount);
            Debug.Log($"{Name} が {amount} HP回復した！ (現在HP: {CharacterStatus.CurrentHP})");
        }
        
        /// <summary>
        /// MPを消費
        /// </summary>
        public bool ConsumeMP(int amount)
        {
            if (CharacterStatus.CurrentMP >= amount)
            {
                CharacterStatus.CurrentMP -= amount;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// ターン開始時の処理
        /// </summary>
        public void OnTurnStart()
        {
            IsGuarding = false;
            HasActedThisTurn = false;
        }
        
        /// <summary>
        /// ターン終了時の処理
        /// </summary>
        public void OnTurnEnd()
        {
            // バフの経過処理
            CharacterStatus.TickBuffs();
        }
    }
    
    /// <summary>
    /// 戦闘AI用インターフェース
    /// </summary>
    public interface IBattleAI
    {
        BattleAction DecideAction(BattleParticipant self, List<BattleParticipant> allies, List<BattleParticipant> enemies);
    }
} 