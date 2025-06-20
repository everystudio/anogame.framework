using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 戦闘行動を実行するクラス
    /// </summary>
    public static class BattleActionExecutor
    {
        /// <summary>
        /// 行動を実行
        /// </summary>
        public static void ExecuteAction(BattleAction action)
        {
            if (action?.Actor == null || !action.Actor.IsAlive)
            {
                Debug.LogWarning("無効な行動または行動者が戦闘不能です");
                return;
            }

            switch (action.ActionType)
            {
                case BattleActionType.Attack:
                    ExecuteAttack(action);
                    break;
                case BattleActionType.Skill:
                    ExecuteSkill(action);
                    break;
                case BattleActionType.Item:
                    ExecuteItem(action);
                    break;
                case BattleActionType.Guard:
                    ExecuteGuard(action);
                    break;
                case BattleActionType.Escape:
                    ExecuteEscape(action);
                    break;
            }

            action.Actor.HasActedThisTurn = true;
        }

        /// <summary>
        /// 通常攻撃を実行
        /// </summary>
        private static void ExecuteAttack(BattleAction action)
        {
            if (action.Target == null || !action.Target.IsAlive)
            {
                Debug.Log($"{action.Actor.Name} の攻撃は対象がいない！");
                return;
            }

            int damage = CalculateAttackDamage(action.Actor, action.Target);
            action.Target.TakeDamage(damage);
            
            Debug.Log($"{action.Actor.Name} が {action.Target.Name} を攻撃！ {damage} のダメージ！");
        }

        /// <summary>
        /// スキルを実行
        /// </summary>
        private static void ExecuteSkill(BattleAction action)
        {
            if (action.Skill == null)
            {
                Debug.LogWarning("スキルが設定されていません");
                return;
            }

            // MP消費チェック（今後実装予定）
            // if (!action.Actor.ConsumeMP(action.Skill.MPCost))
            // {
            //     Debug.Log($"{action.Actor.Name} のMPが足りない！");
            //     return;
            // }

            Debug.Log($"{action.Actor.Name} が {action.Skill.SkillName} を使用！");

            // 単体対象
            if (action.Target != null)
            {
                var context = new EffectContext(action.Target.CharacterStatus, action.Actor.CharacterStatus);
                EffectUtils.ApplyEffects(action.Skill, context);
            }
            // 複数対象
            else if (action.Targets != null)
            {
                foreach (var target in action.Targets)
                {
                    if (target != null && target.IsAlive)
                    {
                        var context = new EffectContext(target.CharacterStatus, action.Actor.CharacterStatus);
                        EffectUtils.ApplyEffects(action.Skill, context);
                    }
                }
            }
        }

        /// <summary>
        /// アイテムを使用
        /// </summary>
        private static void ExecuteItem(BattleAction action)
        {
            if (action.Item == null)
            {
                Debug.LogWarning("アイテムが設定されていません");
                return;
            }

            if (!action.Item.CanUseInBattle)
            {
                Debug.Log($"{action.Item.DisplayName} は戦闘中に使用できません！");
                return;
            }

            var target = action.Target ?? action.Actor; // 対象が指定されていなければ自分
            bool success = action.Item.Use(target.CharacterStatus, action.Actor.CharacterStatus);
            
            if (success)
            {
                Debug.Log($"{action.Actor.Name} が {action.Item.DisplayName} を使用！");
            }
            else
            {
                Debug.Log($"{action.Item.DisplayName} の使用に失敗！");
            }
        }

        /// <summary>
        /// 防御を実行
        /// </summary>
        private static void ExecuteGuard(BattleAction action)
        {
            action.Actor.IsGuarding = true;
            Debug.Log($"{action.Actor.Name} は身を守っている！");
        }

        /// <summary>
        /// 逃走を実行
        /// </summary>
        private static void ExecuteEscape(BattleAction action)
        {
            // 逃走成功率の計算（今後実装）
            bool success = Random.value > 0.3f; // 仮実装：70%の確率で成功
            
            if (success)
            {
                Debug.Log($"{action.Actor.Name} は逃走した！");
                // 戦闘終了の処理は BattleManager で行う
            }
            else
            {
                Debug.Log($"{action.Actor.Name} は逃げられなかった！");
            }
        }

        /// <summary>
        /// 攻撃ダメージを計算
        /// </summary>
        private static int CalculateAttackDamage(BattleParticipant attacker, BattleParticipant target)
        {
            int attackPower = attacker.CharacterStatus.TotalAttack;
            int defense = target.CharacterStatus.TotalDefense;
            
            // 基本ダメージ = 攻撃力 - 防御力の半分
            int baseDamage = Mathf.Max(1, attackPower - defense / 2);
            
            // ランダム要素（±20%）
            float randomFactor = Random.Range(0.8f, 1.2f);
            int finalDamage = Mathf.RoundToInt(baseDamage * randomFactor);
            
            return Mathf.Max(1, finalDamage); // 最低1ダメージ
        }
    }
} 