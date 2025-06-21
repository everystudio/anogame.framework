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
        
        // カスタムメッセージが設定されたかどうかのフラグ
        private bool hasCustomMessage = false;

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
            
            // カスタムメッセージが設定されていない場合のみ、デフォルトセットアップを実行
            if (!hasCustomMessage)
            {
                SetupForPageType(PageId);
            }
            
            // ページに入った時の処理をここに実装
            // アニメーションや初期化など
        }

                public override void OnExit()
        {
            Debug.Log($"[ExamplePage] '{PageId}' から出ました");
            
            // カスタムメッセージフラグをリセット
            hasCustomMessage = false;
            
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
            else
            {
                // nextPageIdが設定されていない場合は、ページの種類に応じて自動的に次のページを決定
                var nextPage = GetNextPageId();
                if (!string.IsNullOrEmpty(nextPage))
                {
                    PageManager.Instance.PushPage(nextPage, (page) =>
                    {
                        if (page is ExamplePage examplePage)
                        {
                            examplePage.SetupForPageType(nextPage);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 現在のページIDに基づいて次のページIDを決定
        /// </summary>
        /// <returns>次のページID</returns>
        private string GetNextPageId()
        {
            return PageId switch
            {
                "HomePage" => "SettingsPage",
                "SettingsPage" => "ProfilePage",
                "ProfilePage" => "GamePage",
                "GamePage" => "HomePage", // 循環
                _ => "HomePage" // デフォルト
            };
        }

                /// <summary>
        /// ページタイプに応じたセットアップ
        /// </summary>
        /// <param name="pageType">ページタイプ</param>
        public void SetupForPageType(string pageType)
        {
            var message = pageType switch
            {
                "HomePage" => "ホームページ - メインメニュー",
                "SettingsPage" => "設定ページ - ゲーム設定",
                "ProfilePage" => "プロフィールページ - ユーザー情報",
                "GamePage" => "ゲームページ - ゲーム画面",
                _ => $"ページ: {pageType}"
            };
            
            // デフォルトメッセージを設定（カスタムメッセージフラグはリセットしない）
            SetDefaultPageMessage(message);
            
            // ページタイプに応じてボタンのテキストも変更
            UpdateButtonText(pageType);
        }
        
        /// <summary>
        /// デフォルトページメッセージを設定（カスタムメッセージフラグに影響しない）
        /// </summary>
        /// <param name="message">設定するメッセージ</param>
        private void SetDefaultPageMessage(string message)
        {
            // タイトルを更新
            if (pageTitle != null)
            {
                pageTitle.text = $"Page: {PageId} - {message}";
            }
            
            Debug.Log($"[ExamplePage] デフォルトメッセージを設定: '{message}'");
        }

        /// <summary>
        /// ページタイプに応じてボタンのテキストを更新
        /// </summary>
        /// <param name="pageType">ページタイプ</param>
        private void UpdateButtonText(string pageType)
        {
            if (nextPageButton != null)
            {
                var buttonText = nextPageButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    var nextPage = GetNextPageId();
                    var nextPageName = nextPage switch
                    {
                        "HomePage" => "ホーム",
                        "SettingsPage" => "設定",
                        "ProfilePage" => "プロフィール",
                        "GamePage" => "ゲーム",
                        _ => "次へ"
                    };
                    buttonText.text = $"{nextPageName}へ";
                }
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
                pageTitle.text = $"PageId: {PageId}";
            }
        }

        /// <summary>
        /// ページメッセージを動的に設定する
        /// </summary>
        /// <param name="message">設定するメッセージ</param>
        public void SetPageMessage(string message)
        {
            // カスタムメッセージが設定されたことを記録
            hasCustomMessage = true;
            
            // タイトルを更新
            if (pageTitle != null)
            {
                pageTitle.text = $"Page: {PageId} - {message}";
            }
            
            Debug.Log($"[ExamplePage] カスタムメッセージを設定: '{message}'");
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