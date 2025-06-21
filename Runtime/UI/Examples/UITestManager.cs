using UnityEngine;
using UnityEngine.UI;

namespace anogame.framework.UI
{
    /// <summary>
    /// UIテスト用のマネージャー
    /// シーンでのUI表示テストを簡単に行うためのヘルパークラス
    /// </summary>
    public class UITestManager : MonoBehaviour
    {
        [Header("テスト対象のプレハブ")]
        [SerializeField] private GameObject examplePagePrefab;
        [SerializeField] private GameObject exampleModalPrefab;
        [SerializeField] private GameObject exampleSheetPrefab;

        [Header("テスト用ボタン")]
        [SerializeField] private Button showPageButton;
        [SerializeField] private Button showModalButton;
        [SerializeField] private Button showPrefabPageButton;
        [SerializeField] private Button showPrefabModalButton;
        [SerializeField] private Button debugUIButton;

        [Header("設定")]
        [SerializeField] private Transform pageParent;
        [SerializeField] private Transform modalParent;
        [SerializeField] private string testPageId = "TestPage";
        [SerializeField] private string testModalId = "TestModal";

        // ページ間移動テスト用
        [Header("ページ間移動テスト")]
        [SerializeField] private Button testPageNavigationButton;
        [SerializeField] private Button goBackButton;

        private void Start()
        {
            SetupTestButtons();
            SetupTestTemplates();
        }

        /// <summary>
        /// テスト用ボタンのイベント設定
        /// </summary>
        private void SetupTestButtons()
        {
            if (showPageButton != null)
                showPageButton.onClick.AddListener(ShowTestPage);

            if (showModalButton != null)
                showModalButton.onClick.AddListener(ShowTestModal);

            if (showPrefabPageButton != null)
                showPrefabPageButton.onClick.AddListener(ShowPageFromPrefab);

            if (showPrefabModalButton != null)
                showPrefabModalButton.onClick.AddListener(ShowModalFromPrefab);

            if (debugUIButton != null)
                debugUIButton.onClick.AddListener(DebugUI);

            // ページ間移動テスト用ボタン
            if (testPageNavigationButton != null)
                testPageNavigationButton.onClick.AddListener(TestPageNavigation);

            if (goBackButton != null)
                goBackButton.onClick.AddListener(GoBackPage);
        }

        /// <summary>
        /// テスト用テンプレートの準備
        /// </summary>
        private void SetupTestTemplates()
        {
            // UIManagerが存在することを確認
            if (UIManager.Instance == null)
            {
                Debug.LogError("UIManagerが見つかりません。UIManagerをシーンに配置してください。");
                return;
            }

            // 複数のページテンプレートを登録
            if (examplePagePrefab != null)
            {
                PageManager.Instance.RegisterPageTemplate("HomePage", examplePagePrefab);
                PageManager.Instance.RegisterPageTemplate("SettingsPage", examplePagePrefab);
                PageManager.Instance.RegisterPageTemplate("ProfilePage", examplePagePrefab);
                PageManager.Instance.RegisterPageTemplate("GamePage", examplePagePrefab);
                PageManager.Instance.RegisterPageTemplate(testPageId, examplePagePrefab);
                Debug.Log("[UITestManager] 複数のページテンプレートを登録しました");
            }

            // モーダルテンプレートの登録
            if (exampleModalPrefab != null)
            {
                // テンプレート用のインスタンスを作成（非アクティブ）
                var templateObj = Instantiate(exampleModalPrefab);
                templateObj.SetActive(false);
                var modal = templateObj.GetComponent<IModal>();
                if (modal != null)
                {
                    ModalManager.Instance.RegisterModal(modal);
                    Debug.Log($"[UITestManager] モーダルテンプレート '{testModalId}' を登録しました");
                }
            }

            Debug.Log("[UITestManager] テスト用UIの準備完了");
        }

        /// <summary>
        /// テスト用ページを表示（テンプレート使用）
        /// </summary>
        [ContextMenu("Show Test Page")]
        public void ShowTestPage()
        {
            if (PageManager.Instance != null)
            {
                PageManager.Instance.NavigateToPage(testPageId, (page) =>
                {
                    if (page is ExamplePage examplePage)
                    {
                        examplePage.SetPageMessage($"テンプレートページ - 時刻: {System.DateTime.Now:HH:mm:ss}");
                    }
                });
                Debug.Log($"[UITestManager] ページ '{testPageId}' を表示しました");
            }
            else
            {
                Debug.LogError("PageManagerが見つかりません");
            }
        }

        /// <summary>
        /// Prefabから直接ページを表示
        /// </summary>
        [ContextMenu("Show Page From Prefab")]
        public void ShowPageFromPrefab()
        {
            if (PageManager.Instance != null && examplePagePrefab != null)
            {
                Debug.LogError($"[UITestManager] Prefabからページを表示します: {examplePagePrefab.name}");
                var page = PageManager.Instance.OpenPageFromPrefab(examplePagePrefab, (p) =>
                {
                    if (p is ExamplePage examplePage)
                    {
                        examplePage.SetPageMessage($"Prefabページ - ID: {p.InstanceId[..8]}");
                    }
                });
                Debug.Log($"[UITestManager] Prefabからページを表示しました");
            }
            else
            {
                Debug.LogError("PageManagerまたはPrefabが見つかりません");
            }
        }

        /// <summary>
        /// テスト用モーダルを表示（テンプレート使用）
        /// </summary>
        [ContextMenu("Show Test Modal")]
        public void ShowTestModal()
        {
            if (ModalManager.Instance != null)
            {
                var modal = ModalManager.Instance.OpenModal(testModalId, (m) =>
                {
                    if (m is ExampleModal exampleModal)
                    {
                        exampleModal.SetModalMessage($"テンプレートモーダル - 時刻: {System.DateTime.Now:HH:mm:ss}");
                    }
                });
                Debug.Log($"[UITestManager] モーダル '{testModalId}' を表示しました");
            }
            else
            {
                Debug.LogError("ModalManagerが見つかりません");
            }
        }

        /// <summary>
        /// Prefabから直接モーダルを表示
        /// </summary>
        [ContextMenu("Show Modal From Prefab")]
        public void ShowModalFromPrefab()
        {
            if (ModalManager.Instance != null && exampleModalPrefab != null)
            {
                var modal = ModalManager.Instance.OpenModalFromPrefab(exampleModalPrefab, (m) =>
                {
                    if (m is ExampleModal exampleModal)
                    {
                        exampleModal.SetModalMessage($"Prefabモーダル - ID: {m.InstanceId[..8]}");
                    }
                });
                Debug.Log($"[UITestManager] Prefabからモーダルを表示しました");
            }
            else
            {
                Debug.LogError("ModalManagerまたはPrefabが見つかりません");
            }
        }

        /// <summary>
        /// パラメータ付きテストモーダルを表示
        /// </summary>
        [ContextMenu("Show Parameterized Modal")]
        public void ShowParameterizedModal()
        {
            if (ModalManager.Instance != null)
            {
                var modal = ModalManager.Instance.OpenModal(testModalId, (m) =>
                {
                    // モーダルの初期化パラメータを設定
                    if (m is ExampleModal exampleModal)
                    {
                        exampleModal.SetModalMessage($"パラメータ付きモーダル - 時刻: {System.DateTime.Now:HH:mm:ss}");
                    }
                });
                Debug.Log($"[UITestManager] パラメータ付きモーダル '{testModalId}' を表示しました");
            }
            else
            {
                Debug.LogError("ModalManagerが見つかりません");
            }
        }

        /// <summary>
        /// 複数の同じモーダルを開く
        /// </summary>
        [ContextMenu("Show Multiple Modals")]
        public void ShowMultipleModals()
        {
            if (ModalManager.Instance != null)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var modal = ModalManager.Instance.OpenModal(testModalId, (m) =>
                    {
                        if (m is ExampleModal exampleModal)
                        {
                            exampleModal.SetModalMessage($"モーダル #{i} - ID: {m.InstanceId[..8]}");
                        }
                    });
                }
                Debug.Log($"[UITestManager] 複数の '{testModalId}' モーダルを表示しました");
            }
            else
            {
                Debug.LogError("ModalManagerが見つかりません");
            }
        }

        /// <summary>
        /// 複数のPrefabモーダルを開く
        /// </summary>
        [ContextMenu("Show Multiple Prefab Modals")]
        public void ShowMultiplePrefabModals()
        {
            if (ModalManager.Instance != null && exampleModalPrefab != null)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var modal = ModalManager.Instance.OpenModalFromPrefab(exampleModalPrefab, (m) =>
                    {
                        if (m is ExampleModal exampleModal)
                        {
                            exampleModal.SetModalMessage($"Prefabモーダル #{i} - ID: {m.InstanceId[..8]}");
                        }
                    });
                }
                Debug.Log($"[UITestManager] 複数のPrefabモーダルを表示しました");
            }
            else
            {
                Debug.LogError("ModalManagerまたはPrefabが見つかりません");
            }
        }

        /// <summary>
        /// 指定タイプのモーダルをすべて閉じる
        /// </summary>
        [ContextMenu("Close All Test Modals")]
        public void CloseAllTestModals()
        {
            if (ModalManager.Instance != null)
            {
                ModalManager.Instance.CloseAllModalsOfType(testModalId);
                Debug.Log($"[UITestManager] すべての '{testModalId}' モーダルを閉じました");
            }
        }

        /// <summary>
        /// UIの状態をデバッグ出力
        /// </summary>
        [ContextMenu("Debug UI State")]
        public void DebugUI()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.DebugUIState();
            }
            else
            {
                Debug.LogError("UIManagerが見つかりません");
            }
        }

        /// <summary>
        /// ページ間移動のテスト
        /// </summary>
        [ContextMenu("Test Page Navigation")]
        public void TestPageNavigation()
        {
            if (PageManager.Instance == null)
            {
                Debug.LogError("PageManagerが見つかりません");
                return;
            }

            // 現在のページ数を確認
            var currentPageCount = PageManager.Instance.PageDepth;

            if (currentPageCount == 0)
            {
                // 最初のページを開く
                PageManager.Instance.NavigateToPage("HomePage", (page) =>
                {
                    if (page is ExamplePage examplePage)
                    {
                        examplePage.SetPageMessage("ホームページ");
                        // 次のページへのボタンを有効化（ここでは設定で行う）
                    }
                });
                Debug.Log("[UITestManager] ホームページを開きました");
            }
            else if (currentPageCount == 1)
            {
                // 2番目のページに移動
                PageManager.Instance.PushPage("SettingsPage", (page) =>
                {
                    if (page is ExamplePage examplePage)
                    {
                        examplePage.SetPageMessage("設定ページ");
                    }
                });
                Debug.Log("[UITestManager] 設定ページに移動しました");
            }
            else if (currentPageCount == 2)
            {
                // 3番目のページに移動
                PageManager.Instance.PushPage("ProfilePage", (page) =>
                {
                    if (page is ExamplePage examplePage)
                    {
                        examplePage.SetPageMessage("プロフィールページ");
                    }
                });
                Debug.Log("[UITestManager] プロフィールページに移動しました");
            }
            else
            {
                // 4番目のページに移動
                PageManager.Instance.PushPage("GamePage", (page) =>
                {
                    if (page is ExamplePage examplePage)
                    {
                        examplePage.SetPageMessage("ゲームページ");
                    }
                });
                Debug.Log("[UITestManager] ゲームページに移動しました");
            }
        }

        /// <summary>
        /// 前のページに戻る
        /// </summary>
        [ContextMenu("Go Back Page")]
        public void GoBackPage()
        {
            if (PageManager.Instance != null)
            {
                var success = PageManager.Instance.PopPage();
                if (success)
                {
                    Debug.Log("[UITestManager] 前のページに戻りました");
                }
                else
                {
                    Debug.Log("[UITestManager] 戻るページがありません");
                }
            }
            else
            {
                Debug.LogError("PageManagerが見つかりません");
            }
        }

        /// <summary>
        /// ページ履歴のリセット
        /// </summary>
        [ContextMenu("Reset Page History")]
        public void ResetPageHistory()
        {
            if (PageManager.Instance != null)
            {
                PageManager.Instance.ClearPageStack();
                Debug.Log("[UITestManager] ページ履歴をリセットしました");
            }
        }
    }
}