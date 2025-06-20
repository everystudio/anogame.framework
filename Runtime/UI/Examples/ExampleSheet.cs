using UnityEngine;
using UnityEngine.UI;

namespace anogame.framework.UI.Examples
{
    /// <summary>
    /// シートの使用例
    /// </summary>
    public class ExampleSheet : SheetBase
    {
        [Header("UI要素")]
        [SerializeField] private Text _sheetTitle;
        [SerializeField] private Text _sheetContent;
        [SerializeField] private Button _actionButton;
        [SerializeField] private Image _tabIcon;
        
        [Header("設定")]
        [SerializeField] private string _sheetMessage = "これはサンプルシートです";
        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = Color.gray;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // ボタンイベントの設定
            if (_actionButton != null)
            {
                _actionButton.onClick.AddListener(OnActionButtonClicked);
            }
        }
        
        protected override void Start()
        {
            base.Start();
            
            // 初期設定
            UpdateUI();
        }
        
        public override void OnActivate()
        {
            base.OnActivate();
            
            Debug.Log($"[ExampleSheet] シート '{SheetId}' がアクティブになりました");
            UpdateUI();
        }
        
        public override void OnDeactivate()
        {
            Debug.Log($"[ExampleSheet] シート '{SheetId}' が非アクティブになりました");
            UpdateUI();
            base.OnDeactivate();
        }
        
        public override void OnShow()
        {
            Debug.Log($"[ExampleSheet] シート '{SheetId}' を表示しました");
            UpdateUI();
        }
        
        public override void OnHide()
        {
            Debug.Log($"[ExampleSheet] シート '{SheetId}' を非表示にしました");
        }
        
        /// <summary>
        /// UIの更新
        /// </summary>
        private void UpdateUI()
        {
            // シートタイトルの設定
            if (_sheetTitle != null)
            {
                _sheetTitle.text = $"シート: {SheetId}";
            }
            
            // シートコンテンツの設定
            if (_sheetContent != null)
            {
                _sheetContent.text = _sheetMessage;
            }
            
            // アクティブ状態による色の変更
            if (_tabIcon != null)
            {
                _tabIcon.color = IsActive ? _activeColor : _inactiveColor;
            }
            
            // ボタンの有効/無効
            if (_actionButton != null)
            {
                _actionButton.interactable = IsActive;
            }
        }
        
        /// <summary>
        /// アクションボタンが押された時の処理
        /// </summary>
        private void OnActionButtonClicked()
        {
            Debug.Log($"[ExampleSheet] シート '{SheetId}' のアクションボタンが押されました");
            
            // カスタム処理の例
            ShowMessage("アクションが実行されました！");
        }
        
        /// <summary>
        /// カスタムメッセージを設定
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        public void SetMessage(string message)
        {
            _sheetMessage = message;
            if (_sheetContent != null)
            {
                _sheetContent.text = message;
            }
        }
        
        /// <summary>
        /// メッセージを一時的に表示する
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        private void ShowMessage(string message)
        {
            if (_sheetContent != null)
            {
                var originalMessage = _sheetContent.text;
                _sheetContent.text = message;
                
                // 2秒後に元のメッセージに戻す
                Invoke(nameof(RestoreOriginalMessage), 2f);
            }
        }
        
        /// <summary>
        /// 元のメッセージを復元
        /// </summary>
        private void RestoreOriginalMessage()
        {
            if (_sheetContent != null)
            {
                _sheetContent.text = _sheetMessage;
            }
        }
        
        /// <summary>
        /// タブアイコンの色を設定
        /// </summary>
        /// <param name="activeColor">アクティブ時の色</param>
        /// <param name="inactiveColor">非アクティブ時の色</param>
        public void SetTabColors(Color activeColor, Color inactiveColor)
        {
            _activeColor = activeColor;
            _inactiveColor = inactiveColor;
            UpdateUI();
        }
        
        /// <summary>
        /// シートがクリックされた時の処理（タブクリック）
        /// </summary>
        public void OnSheetClicked()
        {
            Debug.Log($"[ExampleSheet] シート '{SheetId}' がクリックされました");
            
            // 親のUIView（PageまたはModal）からグループIDを取得して
            // SheetManagerでアクティブにする必要がある
            // 実際の実装では、親のUIViewがこの処理を行う
        }
        
        protected override void OnCleanup()
        {
            // ボタンイベントのクリーンアップ
            if (_actionButton != null)
            {
                _actionButton.onClick.RemoveListener(OnActionButtonClicked);
            }
            
            // Invoke のキャンセル
            CancelInvoke();
            
            base.OnCleanup();
        }
    }
} 