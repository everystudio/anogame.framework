using System.Collections.Generic;
using System.Linq;

namespace anogame.framework
{
    /// <summary>
    /// 行動順序を管理するクラス
    /// </summary>
    public class ActionQueue
    {
        private List<BattleAction> actions = new();
        private int currentIndex = 0;

        /// <summary>
        /// アクションをキューに追加
        /// </summary>
        public void AddAction(BattleAction action)
        {
            if (action == null) return;
            
            actions.Add(action);
        }

        /// <summary>
        /// アクションをソートして実行順序を決定
        /// </summary>
        public void SortActions()
        {
            // 速度順（降順）でソート
            actions = actions
                .OrderByDescending(action => action.Actor.CharacterStatus.TotalSpeed)
                .ToList();
            
            currentIndex = 0;
        }

        /// <summary>
        /// 次のアクションを取得
        /// </summary>
        public BattleAction GetNextAction()
        {
            if (currentIndex >= actions.Count)
                return null;

            return actions[currentIndex++];
        }

        /// <summary>
        /// まだ実行されていないアクションがあるか
        /// </summary>
        public bool HasNextAction()
        {
            return currentIndex < actions.Count;
        }

        /// <summary>
        /// 全てのアクションを取得（読み取り専用）
        /// </summary>
        public IReadOnlyList<BattleAction> GetAllActions()
        {
            return actions.AsReadOnly();
        }

        /// <summary>
        /// キューをクリア
        /// </summary>
        public void Clear()
        {
            actions.Clear();
            currentIndex = 0;
        }

        /// <summary>
        /// 指定した参加者のアクションを削除
        /// </summary>
        public void RemoveActionsBy(BattleParticipant participant)
        {
            actions.RemoveAll(action => action.Actor == participant);
            
            // インデックスを調整
            if (currentIndex > actions.Count)
            {
                currentIndex = actions.Count;
            }
        }
    }
} 