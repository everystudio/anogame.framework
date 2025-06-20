using UnityEngine;
using UnityEngine.UI;

namespace anogame.framework.UI
{
    /// <summary>
    /// シートの使用例
    /// </summary>
    public class ExampleSheet : SheetBase
    {
        [Header("UI要素")]
        [SerializeField] private Text sheetTitle;
        [SerializeField] private Text sheetContent;
        [SerializeField] private Button actionButton;
        [SerializeField] private Image tabIcon;
        
        [Header("設定")]
        [SerializeField] private string sheetMessage = "これはサンプルシートです";
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.gray;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // UIの初期設定
            if (sheetTitle != null)
            {
                sheetTitle.text = $"Sheet: {SheetId}";
            }
            
            if (sheetContent != null)
            {
                sheetContent.text = sheetMessage;
            }
            
            // ボタンイベントの設定
            if (actionButton != null)
            {
                actionButton.onClick.AddListener(OnActionButtonClicked);
            }
            
            // 初期状態を設定
            UpdateVisualState();
        }
        
        public override void OnActivate()
        {
            Debug.Log($"[ExampleSheet] '{SheetId}' がアクティブになりました");
            
            // アクティブになった時の処理をここに実装
            UpdateVisualState();
        }
        
        public override void OnDeactivate()
        {
            Debug.Log($"[ExampleSheet] '{SheetId}' が非アクティブになりました");
            
            // 非アクティブになった時の処理をここに実装
            UpdateVisualState();
        }
        
        /// <summary>
        /// アクション実行ボタンが押された時の処理
        /// </summary>
        private void OnActionButtonClicked()
        {
            Debug.Log($"[ExampleSheet] '{SheetId}' でアクションが実行されました");
            
            // シート固有のアクション処理をここに実装
            // 例：データの更新、他のシートへの切り替えなど
        }
        
        /// <summary>
        /// 表示状態を更新
        /// </summary>
        private void UpdateVisualState()
        {
            if (tabIcon != null)
            {
                tabIcon.color = IsActive ? activeColor : inactiveColor;
            }
            
            // アクティブ状態に応じてUIを更新
            if (actionButton != null)
            {
                actionButton.interactable = IsActive;
            }
        }
        
        /// <summary>
        /// カスタムメッセージを設定
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        public void SetMessage(string message)
        {
            sheetMessage = message;
            if (sheetContent != null)
            {
                sheetContent.text = message;
            }
        }
        
        protected override void OnCleanup()
        {
            // ボタンイベントのクリーンアップ
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(OnActionButtonClicked);
            }
            
            base.OnCleanup();
        }
    }
} 