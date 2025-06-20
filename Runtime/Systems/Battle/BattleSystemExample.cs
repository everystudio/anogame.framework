using System.Collections.Generic;
using UnityEngine;

namespace anogame.framework
{
    /// <summary>
    /// 戦闘システムの使用例とテスト用クラス
    /// </summary>
    public class BattleSystemExample : MonoBehaviour
    {
        [Header("テスト設定")]
        [SerializeField] private bool startBattleOnStart = true;
        [SerializeField] private int playerCount = 1;
        [SerializeField] private int enemyCount = 2;

        [Header("スキル設定")]
        [SerializeField] private SkillDefinition[] testSkills;

        private BattleManager _battleManager;
        private BattleUI _battleUI;

        private void Start()
        {
            _battleManager = FindFirstObjectByType<BattleManager>();
            _battleUI = FindFirstObjectByType<BattleUI>();

            if (_battleManager == null)
            {
                Debug.LogError("BattleManager が見つかりません");
                return;
            }

            // イベント登録
            RegisterBattleEvents();

            if (startBattleOnStart)
            {
                StartTestBattle();
            }
        }

        /// <summary>
        /// 戦闘イベントを登録
        /// </summary>
        private void RegisterBattleEvents()
        {
            _battleManager.OnStateChanged += OnBattleStateChanged;
            _battleManager.OnActionExecuted += OnActionExecuted;
            _battleManager.OnParticipantDefeated += OnParticipantDefeated;
            _battleManager.OnBattleEnded += OnBattleEnded;
            _battleManager.OnTurnStarted += OnTurnStarted;
        }

        /// <summary>
        /// テスト戦闘を開始
        /// </summary>
        [ContextMenu("Start Test Battle")]
        public void StartTestBattle()
        {
            var players = CreateTestPlayers();
            var enemies = CreateTestEnemies();

            // UIに戦闘参加者を設定
            if (_battleUI != null)
            {
                _battleUI.InitializeParticipants(players, enemies);
            }

            Debug.Log("=== テスト戦闘を開始します ===");
            _battleManager.StartBattle(players, enemies);
        }

        /// <summary>
        /// テスト用プレイヤーを作成
        /// </summary>
        private List<BattleParticipant> CreateTestPlayers()
        {
            var players = new List<BattleParticipant>();

            for (int i = 0; i < playerCount; i++)
            {
                var status = new CharacterStatus
                {
                    BaseMaxHP = 100,
                    BaseMaxMP = 30,
                    BaseAttack = 15,
                    BaseDefense = 8,
                    BaseSpeed = 10
                };
                status.CurrentHP = status.MaxHP;
                status.CurrentMP = status.MaxMP;

                var player = new BattleParticipant(ParticipantType.Player, status, $"プレイヤー{i + 1}");

                // テスト用スキルを追加
                if (testSkills != null)
                {
                    foreach (var skill in testSkills)
                    {
                        if (skill != null)
                        {
                            player.AddSkill(skill);
                        }
                    }
                }

                players.Add(player);
            }

            return players;
        }

        /// <summary>
        /// テスト用敵を作成
        /// </summary>
        private List<BattleParticipant> CreateTestEnemies()
        {
            var enemies = new List<BattleParticipant>();

            for (int i = 0; i < enemyCount; i++)
            {
                var status = new CharacterStatus
                {
                    BaseMaxHP = 80,
                    BaseMaxMP = 20,
                    BaseAttack = 12,
                    BaseDefense = 5,
                    BaseSpeed = 8
                };
                status.CurrentHP = status.MaxHP;
                status.CurrentMP = status.MaxMP;

                var enemy = new BattleParticipant(ParticipantType.Enemy, status, $"敵{i + 1}")
                {
                    AI = new SimpleAI() // 簡単なAIを設定
                };

                enemies.Add(enemy);
            }

            return enemies;
        }

        // イベントハンドラー
        private void OnBattleStateChanged(BattleState newState)
        {
            Debug.Log($"戦闘状態変更: {newState}");

            if (_battleUI != null)
            {
                _battleUI.AddLog($"状態: {GetBattleStateText(newState)}");
            }
        }

        private void OnActionExecuted(BattleAction action)
        {
            string message = $"{action.Actor.Name} が {GetActionTypeText(action.ActionType)}";

            if (action.Target != null)
            {
                message += $" → {action.Target.Name}";
            }

            Debug.Log(message);

            if (_battleUI != null)
            {
                _battleUI.AddLog(message);
            }
        }

        private void OnParticipantDefeated(BattleParticipant participant)
        {
            string message = $"{participant.Name} が倒れた！";
            Debug.Log(message);

            if (_battleUI != null)
            {
                _battleUI.AddLog(message);
            }
        }

        private void OnBattleEnded(BattleResult result)
        {
            string message = $"戦闘終了: {GetBattleResultText(result)}";
            Debug.Log(message);

            if (_battleUI != null)
            {
                _battleUI.AddLog(message);
            }
        }

        private void OnTurnStarted(int turnNumber)
        {
            Debug.Log($"ターン {turnNumber} 開始");

            if (_battleUI != null)
            {
                _battleUI.UpdateTurnInfo(turnNumber);
                _battleUI.AddLog($"--- ターン {turnNumber} ---");
            }
        }

        // ヘルパーメソッド
        private string GetBattleStateText(BattleState state)
        {
            return state switch
            {
                BattleState.Starting => "戦闘開始",
                BattleState.DeterminingOrder => "行動順決定中",
                BattleState.WaitingForPlayerAction => "プレイヤー行動選択中",
                BattleState.EnemyActionDecision => "敵行動決定中",
                BattleState.ExecutingAction => "行動実行中",
                BattleState.TurnEnd => "ターン終了",
                BattleState.Victory => "勝利！",
                BattleState.Defeat => "敗北...",
                BattleState.Ended => "戦闘終了",
                _ => state.ToString()
            };
        }

        private string GetActionTypeText(BattleActionType actionType)
        {
            return actionType switch
            {
                BattleActionType.Attack => "攻撃",
                BattleActionType.Skill => "スキル使用",
                BattleActionType.Item => "アイテム使用",
                BattleActionType.Guard => "防御",
                BattleActionType.Escape => "逃走",
                _ => actionType.ToString()
            };
        }

        private string GetBattleResultText(BattleResult result)
        {
            return result switch
            {
                BattleResult.Victory => "勝利！",
                BattleResult.Defeat => "敗北...",
                BattleResult.Draw => "引き分け",
                _ => result.ToString()
            };
        }

        private void OnDestroy()
        {
            // イベント登録解除
            if (_battleManager != null)
            {
                _battleManager.OnStateChanged -= OnBattleStateChanged;
                _battleManager.OnActionExecuted -= OnActionExecuted;
                _battleManager.OnParticipantDefeated -= OnParticipantDefeated;
                _battleManager.OnBattleEnded -= OnBattleEnded;
                _battleManager.OnTurnStarted -= OnTurnStarted;
            }
        }
    }

    /// <summary>
    /// 簡単な敵AI実装例
    /// </summary>
    public class SimpleAI : IBattleAI
    {
        public BattleAction DecideAction(BattleParticipant self, List<BattleParticipant> allies, List<BattleParticipant> enemies)
        {
            // 生きている敵（プレイヤー）をランダムに攻撃
            var aliveEnemies = enemies.FindAll(e => e.IsAlive);

            if (aliveEnemies.Count > 0)
            {
                var target = aliveEnemies[Random.Range(0, aliveEnemies.Count)];

                // 70%の確率で攻撃、30%の確率で防御
                if (Random.value > 0.3f)
                {
                    return new BattleAction(BattleActionType.Attack, self, target);
                }
                else
                {
                    return new BattleAction(BattleActionType.Guard, self);
                }
            }

            return new BattleAction(BattleActionType.Guard, self);
        }
    }
}