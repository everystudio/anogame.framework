using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace anogame.framework.UI
{
    /// <summary>
    /// ページの使用例
    /// </summary>
    public class ExamplePage : PageBase
    {
        [Header("UI要素")]
        [SerializeField] private Button nextPageButton;
        [SerializeField] private Button openModalButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI pageTitle;
        
        [Header("設定")]
        [SerializeField] private string nextPageId;
        [SerializeField] private string modalId;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // UIの初期設定
            if (pageTitle != null)
            {
                pageTitle.text = $"Page: {PageId}";
            }
            
            // ボタンイベントの設定
            if (nextPageButton != null)
            {
                nextPageButton.onClick.AddListener(GoToNextPage);
            }
            
            if (openModalButton != null)
            {
                openModalButton.onClick.AddListener(OpenModal);
            }
            
            if (backButton != null)
            {
                backButton.onClick.AddListener(GoBack);
            }
        }
        
        public override void OnEnter()
        {
            Debug.Log($"[ExamplePage] '{PageId}' に入りました");
            
            // ページに入った時の処理をここに実装
            // アニメーションや初期化など
        }
        
        public override void OnExit()
        {
            Debug.Log($"[ExamplePage] '{PageId}' から出ました");
            
            // ページから出る時の処理をここに実装
            // 状態保存やクリーンアップなど
        }
        
        public override bool CanGoBack()
        {
            // 戻ることができるかどうかの判定
            // 例：未保存のデータがある場合は false を返す
            return true;
        }
        
        private void GoToNextPage()
        {
            if (!string.IsNullOrEmpty(nextPageId))
            {
                PageManager.Instance.PushPage(nextPageId);
            }
        }
        
        private void OpenModal()
        {
            if (!string.IsNullOrEmpty(modalId))
            {
                ModalManager.Instance.OpenModal(modalId);
            }
        }
        
        private void GoBack()
        {
            PageManager.Instance.PopPage();
        }
        
        /// <summary>
        /// PageIDを動的に設定する（テスト用）
        /// </summary>
        /// <param name="pageId">設定するページID</param>
        public new void SetPageId(string pageId)
        {
            base.SetPageId(pageId);
            
            // タイトルも更新
            if (pageTitle != null)
            {
                pageTitle.text = $"Page: {PageId}";
            }
        }
        
        protected override void OnCleanup()
        {
            // ボタンイベントのクリーンアップ
            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveListener(GoToNextPage);
            }
            
            if (openModalButton != null)
            {
                openModalButton.onClick.RemoveListener(OpenModal);
            }
            
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(GoBack);
            }
            
            base.OnCleanup();
        }
    }
} 