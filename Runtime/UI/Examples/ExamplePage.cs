using UnityEngine;
using UnityEngine.UI;

namespace anogame.framework.UI.Examples
{
    /// <summary>
    /// ページの使用例
    /// </summary>
    public class ExamplePage : PageBase
    {
        [Header("UI要素")]
        [SerializeField] private Button _nextPageButton;
        [SerializeField] private Button _openModalButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _pageTitle;
        
        [Header("設定")]
        [SerializeField] private string _nextPageId;
        [SerializeField] private string _modalId;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // ボタンイベントの設定
            if (_nextPageButton != null)
            {
                _nextPageButton.onClick.AddListener(OnNextPageButtonClicked);
            }
            
            if (_openModalButton != null)
            {
                _openModalButton.onClick.AddListener(OnOpenModalButtonClicked);
            }
            
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            // ページタイトルの設定
            if (_pageTitle != null)
            {
                _pageTitle.text = $"ページ: {PageId}";
            }
            
            Debug.Log($"[ExamplePage] ページ '{PageId}' に入りました");
        }
        
        public override void OnExit()
        {
            Debug.Log($"[ExamplePage] ページ '{PageId}' から出ます");
            base.OnExit();
        }
        
        public override void OnShow()
        {
            Debug.Log($"[ExamplePage] ページ '{PageId}' を表示しました");
        }
        
        public override void OnHide()
        {
            Debug.Log($"[ExamplePage] ページ '{PageId}' を非表示にしました");
        }
        
        /// <summary>
        /// 次のページボタンが押された時の処理
        /// </summary>
        private void OnNextPageButtonClicked()
        {
            if (!string.IsNullOrEmpty(_nextPageId))
            {
                PageManager.Instance.PushPage(_nextPageId);
            }
        }
        
        /// <summary>
        /// モーダルを開くボタンが押された時の処理
        /// </summary>
        private void OnOpenModalButtonClicked()
        {
            if (!string.IsNullOrEmpty(_modalId))
            {
                ModalManager.Instance.OpenModal(_modalId);
            }
        }
        
        /// <summary>
        /// 戻るボタンが押された時の処理
        /// </summary>
        private void OnBackButtonClicked()
        {
            PageManager.Instance.PopPage();
        }
        
        /// <summary>
        /// 戻る処理のカスタマイズ例
        /// </summary>
        /// <returns>戻る処理を実行するかどうか</returns>
        public override bool OnBackPressed()
        {
            // 例：確認ダイアログを表示してから戻る
            Debug.Log($"[ExamplePage] ページ '{PageId}' で戻る処理が要求されました");
            
            // 通常は true を返して戻る処理を許可
            // false を返すと戻る処理をキャンセルできる
            return true;
        }
        
        protected override void OnCleanup()
        {
            // ボタンイベントのクリーンアップ
            if (_nextPageButton != null)
            {
                _nextPageButton.onClick.RemoveListener(OnNextPageButtonClicked);
            }
            
            if (_openModalButton != null)
            {
                _openModalButton.onClick.RemoveListener(OnOpenModalButtonClicked);
            }
            
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
            }
            
            base.OnCleanup();
        }
    }
} 