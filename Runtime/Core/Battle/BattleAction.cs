using System;

namespace anogame.framework
{
    /// <summary>
    /// 戦闘行動の種類
    /// </summary>
    public enum BattleActionType
    {
        Attack,      // 通常攻撃
        Skill,       // スキル使用
        Item,        // アイテム使用
        Guard,       // 防御
        Escape       // 逃走
    }

    /// <summary>
    /// 戦闘での行動を表すクラス
    /// </summary>
    [Serializable]
    public class BattleAction
    {
        public BattleActionType ActionType { get; set; }
        public BattleParticipant Actor { get; set; }
        public BattleParticipant Target { get; set; }
        public BattleParticipant[] Targets { get; set; } // 複数対象用
        
        // スキル使用時
        public SkillDefinition Skill { get; set; }
        
        // アイテム使用時
        public ConsumableItem Item { get; set; }
        
        // 行動の優先度（速度ベース）
        public int Priority { get; set; }
        
        public BattleAction(BattleActionType actionType, BattleParticipant actor)
        {
            ActionType = actionType;
            Actor = actor;
            Priority = actor.CharacterStatus.TotalSpeed;
        }
        
        public BattleAction(BattleActionType actionType, BattleParticipant actor, BattleParticipant target) 
            : this(actionType, actor)
        {
            Target = target;
        }
        
        /// <summary>
        /// 複数対象への行動
        /// </summary>
        public BattleAction(BattleActionType actionType, BattleParticipant actor, BattleParticipant[] targets)
            : this(actionType, actor)
        {
            Targets = targets;
        }
    }
} 