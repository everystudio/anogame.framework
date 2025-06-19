using System.Collections.Generic;
using System.Linq;

namespace anogame.framework
{
    /// <summary>
    /// 行動順序を管理するクラス
    /// </summary>
    public class ActionQueue
    {
        private List<BattleAction> _actions = new();
        private int _currentIndex = 0;

        /// <summary>
        /// 行動を追加
        /// </summary>
        public void AddAction(BattleAction action)
        {
            _actions.Add(action);
        }

        /// <summary>
        /// 行動順序を決定（速度順でソート）
        /// </summary>
        public void DetermineOrder()
        {
            // 速度が高い順 → 同じ速度の場合はランダム
            _actions = _actions
                .OrderByDescending(a => a.Priority)
                .ThenBy(a => UnityEngine.Random.value)
                .ToList();
            
            _currentIndex = 0;
        }

        /// <summary>
        /// 次の行動を取得
        /// </summary>
        public BattleAction GetNextAction()
        {
            if (_currentIndex >= _actions.Count)
                return null;

            return _actions[_currentIndex++];
        }

        /// <summary>
        /// まだ実行されていない行動があるか
        /// </summary>
        public bool HasNextAction()
        {
            return _currentIndex < _actions.Count;
        }

        /// <summary>
        /// 現在のターンの行動をすべて取得（デバッグ用）
        /// </summary>
        public IReadOnlyList<BattleAction> GetAllActions()
        {
            return _actions.AsReadOnly();
        }

        /// <summary>
        /// キューをクリア（新しいターン開始時）
        /// </summary>
        public void Clear()
        {
            _actions.Clear();
            _currentIndex = 0;
        }

        /// <summary>
        /// 指定した参加者の行動を削除（戦闘不能時など）
        /// </summary>
        public void RemoveActionsBy(BattleParticipant participant)
        {
            _actions.RemoveAll(action => action.Actor == participant);
            
            // インデックスの調整
            if (_currentIndex > _actions.Count)
            {
                _currentIndex = _actions.Count;
            }
        }
    }
} 