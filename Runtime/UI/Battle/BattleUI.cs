using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace anogame.framework
{
    /// <summary>
    /// 戦闘UIのメインクラス
    /// </summary>
    public class BattleUI : MonoBehaviour, IBattleUI
    {
        [Header("キャラクター表示")]
        [SerializeField] private Transform playerStatusParent;
        [SerializeField] private Transform enemyStatusParent;
        [SerializeField] private GameObject hpMpBarPrefab;

        [Header("行動選択UI")]
        [SerializeField] private GameObject actionSelectionPanel;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button skillButton;
        [SerializeField] private Button itemButton;
        [SerializeField] private Button guardButton;

        [Header("対象選択UI")]
        [SerializeField] private GameObject targetSelectionPanel;
        [SerializeField] private Transform targetButtonParent;
        [SerializeField] private GameObject targetButtonPrefab;

        [Header("スキル選択UI")]
        [SerializeField] private GameObject skillSelectionPanel;
        [SerializeField] private Transform skillButtonParent;
        [SerializeField] private GameObject skillButtonPrefab;

        [Header("戦闘情報")]
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI logText;

        // 現在の選択状態
        private BattleParticipant currentActor;
        private List<BattleParticipant> validTargets;
        private BattleAction selectedAction;
        private bool hasSelectedAction;

        // UI要素の管理
        private Dictionary<BattleParticipant, HPMPBar> statusBars = new();

        private void Start()
        {
            SetupButtonEvents();
            HideAllPanels();
        }

        /// <summary>
        /// ボタンイベントを設定
        /// </summary>
        private void SetupButtonEvents()
        {
            if (attackButton != null)
                attackButton.onClick.AddListener(() => SelectActionType(BattleActionType.Attack));
            
            if (skillButton != null)
                skillButton.onClick.AddListener(() => SelectActionType(BattleActionType.Skill));
            
            if (itemButton != null)
                itemButton.onClick.AddListener(() => SelectActionType(BattleActionType.Item));
            
            if (guardButton != null)
                guardButton.onClick.AddListener(() => SelectActionType(BattleActionType.Guard));
        }

        /// <summary>
        /// 戦闘参加者のUI表示を初期化
        /// </summary>
        public void InitializeParticipants(List<BattleParticipant> players, List<BattleParticipant> enemies)
        {
            // プレイヤーのステータスバーを作成
            foreach (var player in players)
            {
                CreateStatusBar(player, playerStatusParent);
            }

            // 敵のステータスバーを作成
            foreach (var enemy in enemies)
            {
                CreateStatusBar(enemy, enemyStatusParent);
            }
        }

        /// <summary>
        /// ステータスバーを作成
        /// </summary>
        private void CreateStatusBar(BattleParticipant participant, Transform parent)
        {
            if (hpMpBarPrefab == null || parent == null) return;

            GameObject barObject = Instantiate(hpMpBarPrefab, parent);
            HPMPBar statusBar = barObject.GetComponent<HPMPBar>();
            
            if (statusBar != null)
            {
                statusBar.SetTarget(participant.CharacterStatus);
                statusBars[participant] = statusBar;
                
                // 名前表示
                var nameText = barObject.GetComponentInChildren<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = participant.Name;
                }
            }
        }

        /// <summary>
        /// 行動選択UIを表示
        /// </summary>
        public void ShowActionSelection(BattleParticipant participant, List<BattleParticipant> validTargets)
        {
            currentActor = participant;
            this.validTargets = validTargets;
            selectedAction = null;
            hasSelectedAction = false;

            // 使用可能な行動を確認してボタンを有効/無効化
            UpdateActionButtons();
            
            if (actionSelectionPanel != null)
            {
                actionSelectionPanel.SetActive(true);
            }

            UpdateStateText($"{participant.Name} の行動を選択してください");
        }

        /// <summary>
        /// 行動ボタンの有効/無効を更新
        /// </summary>
        private void UpdateActionButtons()
        {
            if (currentActor == null) return;

            // 攻撃は常に可能
            if (attackButton != null)
                attackButton.interactable = validTargets.Count > 0;

            // スキルは利用可能なスキルがある場合のみ
            if (skillButton != null)
                skillButton.interactable = currentActor.AvailableSkills.Count > 0;

            // アイテムは所持アイテムがある場合のみ（今後実装）
            if (itemButton != null)
                itemButton.interactable = false; // 仮実装

            // 防御は常に可能
            if (guardButton != null)
                guardButton.interactable = true;
        }

        /// <summary>
        /// 行動タイプを選択
        /// </summary>
        private void SelectActionType(BattleActionType actionType)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    if (validTargets.Count == 1)
                    {
                        // 対象が1体の場合は即座に決定
                        CompleteAction(new BattleAction(actionType, currentActor, validTargets[0]));
                    }
                    else
                    {
                        ShowTargetSelection(actionType);
                    }
                    break;
                    
                case BattleActionType.Skill:
                    ShowSkillSelection();
                    break;
                    
                case BattleActionType.Item:
                    // アイテム選択UI（今後実装）
                    break;
                    
                case BattleActionType.Guard:
                    CompleteAction(new BattleAction(actionType, currentActor));
                    break;
            }
        }

        /// <summary>
        /// 対象選択UIを表示
        /// </summary>
        private void ShowTargetSelection(BattleActionType actionType)
        {
            HideAllPanels();
            
            if (targetSelectionPanel == null || targetButtonParent == null) return;
            
            // 既存のボタンを削除
            foreach (Transform child in targetButtonParent)
            {
                Destroy(child.gameObject);
            }

            // 対象ボタンを作成
            foreach (var target in validTargets)
            {
                if (targetButtonPrefab != null)
                {
                    GameObject buttonObj = Instantiate(targetButtonPrefab, targetButtonParent);
                    Button button = buttonObj.GetComponent<Button>();
                    TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (text != null)
                    {
                        text.text = $"{target.Name} (HP: {target.CharacterStatus.CurrentHP}/{target.CharacterStatus.MaxHP})";
                    }
                    
                    if (button != null)
                    {
                        button.onClick.AddListener(() => 
                        {
                            CompleteAction(new BattleAction(actionType, currentActor, target));
                        });
                    }
                }
            }

            targetSelectionPanel.SetActive(true);
        }

        /// <summary>
        /// スキル選択UIを表示
        /// </summary>
        private void ShowSkillSelection()
        {
            HideAllPanels();
            
            if (skillSelectionPanel == null || skillButtonParent == null) return;
            
            // 既存のボタンを削除
            foreach (Transform child in skillButtonParent)
            {
                Destroy(child.gameObject);
            }

            // スキルボタンを作成
            foreach (var skill in currentActor.AvailableSkills)
            {
                if (skillButtonPrefab != null)
                {
                    GameObject buttonObj = Instantiate(skillButtonPrefab, skillButtonParent);
                    Button button = buttonObj.GetComponent<Button>();
                    TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (text != null)
                    {
                        text.text = skill.SkillName;
                    }
                    
                    if (button != null)
                    {
                        button.onClick.AddListener(() => 
                        {
                            SelectSkill(skill);
                        });
                    }
                }
            }

            skillSelectionPanel.SetActive(true);
        }

        /// <summary>
        /// スキルを選択
        /// </summary>
        private void SelectSkill(SkillDefinition skill)
        {
            var action = new BattleAction(BattleActionType.Skill, currentActor)
            {
                Skill = skill
            };

            // 対象選択が必要かチェック
            if (validTargets.Count == 1)
            {
                action.Target = validTargets[0];
                CompleteAction(action);
            }
            else
            {
                // 複数対象の場合は対象選択へ
                ShowTargetSelectionForSkill(action);
            }
        }

        /// <summary>
        /// スキル用の対象選択
        /// </summary>
        private void ShowTargetSelectionForSkill(BattleAction skillAction)
        {
            HideAllPanels();
            
            if (targetSelectionPanel == null || targetButtonParent == null) return;
            
            // 既存のボタンを削除
            foreach (Transform child in targetButtonParent)
            {
                Destroy(child.gameObject);
            }

            // 対象ボタンを作成
            foreach (var target in validTargets)
            {
                if (targetButtonPrefab != null)
                {
                    GameObject buttonObj = Instantiate(targetButtonPrefab, targetButtonParent);
                    Button button = buttonObj.GetComponent<Button>();
                    TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    
                    if (text != null)
                    {
                        text.text = $"{target.Name} (HP: {target.CharacterStatus.CurrentHP}/{target.CharacterStatus.MaxHP})";
                    }
                    
                    if (button != null)
                    {
                        button.onClick.AddListener(() => 
                        {
                            skillAction.Target = target;
                            CompleteAction(skillAction);
                        });
                    }
                }
            }

            targetSelectionPanel.SetActive(true);
        }

        /// <summary>
        /// 行動選択を完了
        /// </summary>
        private void CompleteAction(BattleAction action)
        {
            selectedAction = action;
            hasSelectedAction = true;
            
            HideAllPanels();
            UpdateStateText($"{currentActor.Name} の行動が決定しました");
        }

        /// <summary>
        /// すべてのパネルを非表示
        /// </summary>
        private void HideAllPanels()
        {
            if (actionSelectionPanel != null)
                actionSelectionPanel.SetActive(false);
            if (targetSelectionPanel != null)
                targetSelectionPanel.SetActive(false);
            if (skillSelectionPanel != null)
                skillSelectionPanel.SetActive(false);
        }

        /// <summary>
        /// ターン情報を更新
        /// </summary>
        public void UpdateTurnInfo(int turnNumber)
        {
            if (turnText != null)
            {
                turnText.text = $"ターン {turnNumber}";
            }
        }

        /// <summary>
        /// 状態テキストを更新
        /// </summary>
        private void UpdateStateText(string text)
        {
            if (stateText != null)
            {
                stateText.text = text;
            }
        }

        /// <summary>
        /// ログを追加
        /// </summary>
        public void AddLog(string message)
        {
            if (logText != null)
            {
                logText.text += message + "\n";
                
                // ログが長くなりすぎた場合は古いものを削除
                string[] lines = logText.text.Split('\n');
                if (lines.Length > 10)
                {
                    logText.text = string.Join("\n", lines.Skip(lines.Length - 10));
                }
            }
        }

        // IBattleUI インターフェースの実装
        public bool HasSelectedAction() => hasSelectedAction;
        public BattleAction GetSelectedAction() => selectedAction;
    }
} 