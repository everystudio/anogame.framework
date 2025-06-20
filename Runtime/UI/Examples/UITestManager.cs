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
        [SerializeField] private Button debugUIButton;
        
        [Header("設定")]
        [SerializeField] private Transform pageParent;
        [SerializeField] private Transform modalParent;
        [SerializeField] private string testPageId = "TestPage";
        [SerializeField] private string testModalId = "TestModal";
        
        private void Start()
        {
            SetupTestButtons();
            SetupTestUI();
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
                
            if (debugUIButton != null)
                debugUIButton.onClick.AddListener(DebugUI);
        }
        
        /// <summary>
        /// テスト用UIの準備
        /// </summary>
        private void SetupTestUI()
        {
            // UIManagerが存在することを確認
            if (UIManager.Instance == null)
            {
                Debug.LogError("UIManagerが見つかりません。UIManagerをシーンに配置してください。");
                return;
            }
            
            // ページの作成と登録
            if (examplePagePrefab != null)
            {
                CreateAndRegisterPage();
            }
            
            // モーダルの作成と登録
            if (exampleModalPrefab != null)
            {
                CreateAndRegisterModal();
            }
            
            Debug.Log("[UITestManager] テスト用UIの準備完了");
        }
        
        /// <summary>
        /// テスト用ページの作成と登録
        /// </summary>
        private void CreateAndRegisterPage()
        {
            Transform parent = pageParent != null ? pageParent : transform;
            GameObject pageObj = Instantiate(examplePagePrefab, parent);
            
            ExamplePage page = pageObj.GetComponent<ExamplePage>();
            if (page != null)
            {
                // PageIDを設定
                page.SetPageId(testPageId);
                
                // PageManagerに登録
                PageManager.Instance.RegisterPage(page);
                
                Debug.Log($"[UITestManager] ページ '{testPageId}' を登録しました");
            }
            else
            {
                Debug.LogError("ExamplePageコンポーネントが見つかりません");
            }
        }
        
        /// <summary>
        /// テスト用モーダルの作成と登録
        /// </summary>
        private void CreateAndRegisterModal()
        {
            Transform parent = modalParent != null ? modalParent : transform;
            GameObject modalObj = Instantiate(exampleModalPrefab, parent);
            
            ExampleModal modal = modalObj.GetComponent<ExampleModal>();
            if (modal != null)
            {
                // ModalIDを設定
                modal.SetModalId(testModalId);
                
                // ModalManagerに登録
                ModalManager.Instance.RegisterModal(modal);
                
                Debug.Log($"[UITestManager] モーダル '{testModalId}' を登録しました");
            }
            else
            {
                Debug.LogError("ExampleModalコンポーネントが見つかりません");
            }
        }
        
        /// <summary>
        /// テスト用ページを表示
        /// </summary>
        [ContextMenu("Show Test Page")]
        public void ShowTestPage()
        {
            if (PageManager.Instance != null)
            {
                PageManager.Instance.NavigateToPage(testPageId);
                Debug.Log($"[UITestManager] ページ '{testPageId}' を表示しました");
            }
            else
            {
                Debug.LogError("PageManagerが見つかりません");
            }
        }
        
        /// <summary>
        /// テスト用モーダルを表示
        /// </summary>
        [ContextMenu("Show Test Modal")]
        public void ShowTestModal()
        {
            if (ModalManager.Instance != null)
            {
                var modal = ModalManager.Instance.OpenModal(testModalId);
                Debug.Log($"[UITestManager] モーダル '{testModalId}' を表示しました");
            }
            else
            {
                Debug.LogError("ModalManagerが見つかりません");
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
                var modal = ModalManager.Instance.OpenModal(testModalId, (m) => {
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
                    var modal = ModalManager.Instance.OpenModal(testModalId, (m) => {
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
        /// UI状態のデバッグ情報を表示
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
        /// すべてのUIをリセット
        /// </summary>
        [ContextMenu("Reset All UI")]
        public void ResetAllUI()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ResetAllUI();
                Debug.Log("[UITestManager] すべてのUIをリセットしました");
            }
        }
    }
} 