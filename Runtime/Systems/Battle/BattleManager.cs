using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 戦闘システム全体を管理するクラス
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("戦闘設定")]
        [SerializeField] private float actionDelay = 1.0f; // 行動間の待機時間
        [SerializeField] private int maxTurns = 50; // 最大ターン数（無限戦闘防止）

        // 戦闘状態
        public BattleState CurrentState { get; private set; } = BattleState.None;
        public int CurrentTurn { get; private set; } = 0;

        // 戦闘参加者
        private List<BattleParticipant> _playerParty = new();
        private List<BattleParticipant> _enemyParty = new();
        private ActionQueue _actionQueue = new();

        // イベント
        public event Action<BattleState> OnStateChanged;
        public event Action<BattleParticipant> OnParticipantDefeated;
        public event Action<BattleResult> OnBattleEnded;
        public event Action<BattleAction> OnActionExecuted;
        public event Action<int> OnTurnStarted;

        // UI参照
        private IBattleUI _battleUI;

        private void Start()
        {
            _battleUI = FindObjectOfType<BattleUI>();
        }

        /// <summary>
        /// 戦闘を開始
        /// </summary>
        public void StartBattle(List<BattleParticipant> playerParty, List<BattleParticipant> enemyParty)
        {
            if (playerParty == null || enemyParty == null || 
                playerParty.Count == 0 || enemyParty.Count == 0)
            {
                Debug.LogError("無効な戦闘参加者です");
                return;
            }

            _playerParty = new List<BattleParticipant>(playerParty);
            _enemyParty = new List<BattleParticipant>(enemyParty);
            CurrentTurn = 0;

            ChangeState(BattleState.Starting);
            StartCoroutine(BattleLoop());
        }

        /// <summary>
        /// 戦闘のメインループ
        /// </summary>
        private IEnumerator BattleLoop()
        {
            Debug.Log("=== 戦闘開始！ ===");

            while (CurrentState != BattleState.Ended)
            {
                CurrentTurn++;
                OnTurnStarted?.Invoke(CurrentTurn);
                
                if (CurrentTurn > maxTurns)
                {
                    Debug.Log("最大ターン数に達しました。戦闘を終了します。");
                    EndBattle(BattleResult.Draw);
                    yield break;
                }

                Debug.Log($"--- ターン {CurrentTurn} ---");
                
                // ターン開始処理
                yield return StartCoroutine(ProcessTurnStart());
                
                // 行動収集
                yield return StartCoroutine(CollectActions());
                
                // 行動実行
                yield return StartCoroutine(ExecuteActions());
                
                // ターン終了処理
                yield return StartCoroutine(ProcessTurnEnd());
                
                // 勝敗判定
                var result = CheckBattleResult();
                if (result != BattleResult.None)
                {
                    EndBattle(result);
                    yield break;
                }
            }
        }

        /// <summary>
        /// ターン開始処理
        /// </summary>
        private IEnumerator ProcessTurnStart()
        {
            foreach (var participant in GetAllParticipants())
            {
                if (participant.IsAlive)
                {
                    participant.OnTurnStart();
                }
            }
            yield return null;
        }

        /// <summary>
        /// 行動を収集
        /// </summary>
        private IEnumerator CollectActions()
        {
            _actionQueue.Clear();
            ChangeState(BattleState.DeterminingOrder);

            // プレイヤーの行動選択
            foreach (var player in _playerParty.Where(p => p.IsAlive))
            {
                ChangeState(BattleState.WaitingForPlayerAction);
                yield return StartCoroutine(WaitForPlayerAction(player));
            }

            // 敵の行動決定
            ChangeState(BattleState.EnemyActionDecision);
            foreach (var enemy in _enemyParty.Where(e => e.IsAlive))
            {
                var action = DecideEnemyAction(enemy);
                if (action != null)
                {
                    _actionQueue.AddAction(action);
                }
            }

            // 行動順序決定
            _actionQueue.DetermineOrder();
            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// プレイヤーの行動選択を待機
        /// </summary>
        private IEnumerator WaitForPlayerAction(BattleParticipant player)
        {
            Debug.Log($"{player.Name} の行動を選択してください");
            
            BattleAction selectedAction = null;
            
            // UIからの入力待ち（仮実装）
            if (_battleUI != null)
            {
                _battleUI.ShowActionSelection(player, GetValidTargets(player));
                
                // 入力完了まで待機
                yield return new WaitUntil(() => _battleUI.HasSelectedAction());
                selectedAction = _battleUI.GetSelectedAction();
            }
            else
            {
                // UI がない場合の自動行動（デバッグ用）
                selectedAction = CreateDefaultPlayerAction(player);
            }

            if (selectedAction != null)
            {
                _actionQueue.AddAction(selectedAction);
            }
        }

        /// <summary>
        /// 敵の行動を決定
        /// </summary>
        private BattleAction DecideEnemyAction(BattleParticipant enemy)
        {
            if (enemy.AI != null)
            {
                return enemy.AI.DecideAction(enemy, _enemyParty, _playerParty);
            }
            
            // 基本AI: ランダムにプレイヤーを攻撃
            var aliveEnemies = _playerParty.Where(p => p.IsAlive).ToList();
            if (aliveEnemies.Count > 0)
            {
                var target = aliveEnemies[UnityEngine.Random.Range(0, aliveEnemies.Count)];
                return new BattleAction(BattleActionType.Attack, enemy, target);
            }
            
            return null;
        }

        /// <summary>
        /// 行動を実行
        /// </summary>
        private IEnumerator ExecuteActions()
        {
            ChangeState(BattleState.ExecutingAction);

            while (_actionQueue.HasNextAction())
            {
                var action = _actionQueue.GetNextAction();
                
                if (action?.Actor != null && action.Actor.IsAlive)
                {
                    Debug.Log($"{action.Actor.Name} の行動: {action.ActionType}");
                    BattleActionExecutor.ExecuteAction(action);
                    OnActionExecuted?.Invoke(action);
                    
                    // 戦闘不能チェック
                    CheckForDefeatedParticipants();
                    
                    yield return new WaitForSeconds(actionDelay);
                }
            }
        }

        /// <summary>
        /// ターン終了処理
        /// </summary>
        private IEnumerator ProcessTurnEnd()
        {
            ChangeState(BattleState.TurnEnd);
            
            foreach (var participant in GetAllParticipants())
            {
                if (participant.IsAlive)
                {
                    participant.OnTurnEnd();
                }
            }
            
            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// 戦闘不能者をチェック
        /// </summary>
        private void CheckForDefeatedParticipants()
        {
            var allParticipants = GetAllParticipants();
            
            foreach (var participant in allParticipants)
            {
                if (!participant.IsAlive && participant.HasActedThisTurn)
                {
                    OnParticipantDefeated?.Invoke(participant);
                    _actionQueue.RemoveActionsBy(participant);
                }
            }
        }

        /// <summary>
        /// 勝敗判定
        /// </summary>
        private BattleResult CheckBattleResult()
        {
            bool playersAlive = _playerParty.Any(p => p.IsAlive);
            bool enemiesAlive = _enemyParty.Any(e => e.IsAlive);

            if (!playersAlive && !enemiesAlive)
                return BattleResult.Draw;
            if (!playersAlive)
                return BattleResult.Defeat;
            if (!enemiesAlive)
                return BattleResult.Victory;

            return BattleResult.None;
        }

        /// <summary>
        /// 戦闘終了
        /// </summary>
        private void EndBattle(BattleResult result)
        {
            ChangeState(result == BattleResult.Victory ? BattleState.Victory : 
                       result == BattleResult.Defeat ? BattleState.Defeat : BattleState.Ended);
            
            Debug.Log($"=== 戦闘終了: {result} ===");
            OnBattleEnded?.Invoke(result);
            
            ChangeState(BattleState.Ended);
        }

        /// <summary>
        /// 状態変更
        /// </summary>
        private void ChangeState(BattleState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// すべての戦闘参加者を取得
        /// </summary>
        private List<BattleParticipant> GetAllParticipants()
        {
            var all = new List<BattleParticipant>();
            all.AddRange(_playerParty);
            all.AddRange(_enemyParty);
            return all;
        }

        /// <summary>
        /// 有効な対象を取得
        /// </summary>
        private List<BattleParticipant> GetValidTargets(BattleParticipant actor)
        {
            // 基本的には敵を対象とする
            return actor.Type == ParticipantType.Player ? 
                _enemyParty.Where(e => e.IsAlive).ToList() :
                _playerParty.Where(p => p.IsAlive).ToList();
        }

        /// <summary>
        /// デフォルトのプレイヤー行動を作成（UI未実装時の代替）
        /// </summary>
        private BattleAction CreateDefaultPlayerAction(BattleParticipant player)
        {
            var enemies = _enemyParty.Where(e => e.IsAlive).ToList();
            if (enemies.Count > 0)
            {
                var target = enemies[UnityEngine.Random.Range(0, enemies.Count)];
                return new BattleAction(BattleActionType.Attack, player, target);
            }
            
            return null;
        }
    }

    /// <summary>
    /// 戦闘結果
    /// </summary>
    public enum BattleResult
    {
        None,
        Victory,
        Defeat,
        Draw
    }

    /// <summary>
    /// 戦闘UI用インターフェース
    /// </summary>
    public interface IBattleUI
    {
        void ShowActionSelection(BattleParticipant participant, List<BattleParticipant> validTargets);
        bool HasSelectedAction();
        BattleAction GetSelectedAction();
    }
} 