using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// UI全体を統合管理するメインマネージャー
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("設定")]
        [SerializeField] private bool _enableBackButton = true;
        [SerializeField] private bool _enableEscapeKey = true;
        
        /// <summary>
        /// PageManagerの参照
        /// </summary>
        public PageManager PageManager => PageManager.Instance;
        
        /// <summary>
        /// SheetManagerの参照
        /// </summary>
        public SheetManager SheetManager => SheetManager.Instance;
        
        /// <summary>
        /// ModalManagerの参照
        /// </summary>
        public ModalManager ModalManager => ModalManager.Instance;
        
        /// <summary>
        /// UIの初期化イベント
        /// </summary>
        public event Action OnUIInitialized;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            InitializeUI();
        }
        
        /// <summary>
        /// UIシステムの初期化
        /// </summary>
        private void InitializeUI()
        {
            Debug.Log("[UIManager] UIシステムを初期化中...");
            
            // 各マネージャーの初期化を確認
            if (PageManager != null)
            {
                Debug.Log("[UIManager] PageManager初期化完了");
            }
            
            if (SheetManager != null)
            {
                Debug.Log("[UIManager] SheetManager初期化完了");
            }
            
            if (ModalManager != null)
            {
                Debug.Log("[UIManager] ModalManager初期化完了");
            }
            
            // イベントの購読
            SubscribeToEvents();
            
            OnUIInitialized?.Invoke();
            Debug.Log("[UIManager] UIシステム初期化完了");
        }
        
        /// <summary>
        /// イベントの購読
        /// </summary>
        private void SubscribeToEvents()
        {
            if (PageManager != null)
            {
                PageManager.OnPageChanged += OnPageChanged;
            }
            
            if (SheetManager != null)
            {
                SheetManager.OnSheetChanged += OnSheetChanged;
            }
            
            if (ModalManager != null)
            {
                ModalManager.OnModalStateChanged += OnModalStateChanged;
            }
        }
        
        /// <summary>
        /// イベントの購読解除
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (PageManager != null)
            {
                PageManager.OnPageChanged -= OnPageChanged;
            }
            
            if (SheetManager != null)
            {
                SheetManager.OnSheetChanged -= OnSheetChanged;
            }
            
            if (ModalManager != null)
            {
                ModalManager.OnModalStateChanged -= OnModalStateChanged;
            }
        }
        
        /// <summary>
        /// ページ変更時の処理
        /// </summary>
        /// <param name="previousPage">前のページ</param>
        /// <param name="currentPage">現在のページ</param>
        private void OnPageChanged(IPage previousPage, IPage currentPage)
        {
            Debug.Log($"[UIManager] ページ変更: {previousPage?.PageId ?? "None"} -> {currentPage?.PageId ?? "None"}");
        }
        
        /// <summary>
        /// シート変更時の処理
        /// </summary>
        /// <param name="groupId">グループID</param>
        /// <param name="previousSheet">前のシート</param>
        /// <param name="currentSheet">現在のシート</param>
        private void OnSheetChanged(string groupId, ISheet previousSheet, ISheet currentSheet)
        {
            Debug.Log($"[UIManager] シート変更 [{groupId}]: {previousSheet?.SheetId ?? "None"} -> {currentSheet?.SheetId ?? "None"}");
        }
        
        /// <summary>
        /// モーダル状態変更時の処理
        /// </summary>
        /// <param name="modal">対象のモーダル</param>
        /// <param name="isOpen">開いているかどうか</param>
        private void OnModalStateChanged(IModal modal, bool isOpen)
        {
            Debug.Log($"[UIManager] モーダル{(isOpen ? "開" : "閉")}く: {modal.ModalId}");
        }
        
        /// <summary>
        /// 入力処理
        /// </summary>
        private void Update()
        {
            HandleInput();
        }
        
        /// <summary>
        /// 入力処理
        /// </summary>
        private void HandleInput()
        {
            if (!_enableEscapeKey) return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
            
            // Android向けの戻るボタン処理
            if (_enableBackButton && Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackButton();
            }
        }
        
        /// <summary>
        /// ESCキーが押された時の処理
        /// </summary>
        private void HandleEscapeKey()
        {
            // 優先順位: Modal > Page
            if (ModalManager != null && ModalManager.OpenModalCount > 0)
            {
                ModalManager.OnEscapeKeyPressed();
            }
            else if (PageManager != null)
            {
                PageManager.OnBackButtonPressed();
            }
        }
        
        /// <summary>
        /// 戻るボタンが押された時の処理
        /// </summary>
        private void HandleBackButton()
        {
            // ESCキーと同じ処理
            HandleEscapeKey();
        }
        
        /// <summary>
        /// UI要素の自動登録
        /// </summary>
        /// <param name="uiView">登録するUI要素</param>
        public void RegisterUIView(IUIView uiView)
        {
            switch (uiView)
            {
                case IPage page:
                    PageManager?.RegisterPage(page);
                    break;
                case IModal modal:
                    ModalManager?.RegisterModal(modal);
                    break;
                case ISheet sheet:
                    // Sheetの場合はグループIDが必要なので、手動で登録する必要がある
                    Debug.LogWarning($"[UIManager] Sheet '{sheet.SheetId}' の登録にはグループIDが必要です。SheetManager.RegisterSheet()を直接呼び出してください。");
                    break;
            }
        }
        
        /// <summary>
        /// UI要素の自動登録解除
        /// </summary>
        /// <param name="uiView">登録解除するUI要素</param>
        public void UnregisterUIView(IUIView uiView)
        {
            switch (uiView)
            {
                case IPage page:
                    PageManager?.UnregisterPage(page.PageId);
                    break;
                case IModal modal:
                    ModalManager?.UnregisterModal(modal.ModalId);
                    break;
                case ISheet sheet:
                    Debug.LogWarning($"[UIManager] Sheet '{sheet.SheetId}' の登録解除には手動でSheetManager.UnregisterSheet()を呼び出してください。");
                    break;
            }
        }
        
        /// <summary>
        /// 全UIの状態をリセット
        /// </summary>
        public void ResetAllUI()
        {
            Debug.Log("[UIManager] 全UIの状態をリセット中...");
            
            ModalManager?.CloseAllModals();
            SheetManager?.DeactivateAllSheets();
            
            Debug.Log("[UIManager] 全UIの状態をリセット完了");
        }
        
        /// <summary>
        /// デバッグ情報の出力
        /// </summary>
        [ContextMenu("Debug UI State")]
        public void DebugUIState()
        {
            Debug.Log($"[UIManager] === UI状態デバッグ ===");
            Debug.Log($"現在のページ: {PageManager?.CurrentPage?.PageId ?? "None"}");
            Debug.Log($"開いているモーダル数: {ModalManager?.OpenModalCount ?? 0}");
            Debug.Log($"トップモーダル: {ModalManager?.TopModal?.ModalId ?? "None"}");
            
            PageManager?.DebugPageStack();
            ModalManager?.DebugModalStack();
            SheetManager?.DebugSheetGroups();
        }
        
        /// <summary>
        /// UIManagerのクリーンアップ処理
        /// </summary>
        protected virtual void OnCleanup()
        {
            UnsubscribeFromEvents();
        }
        
        /// <summary>
        /// MonoSingletonのOnDestroyをオーバーライド
        /// </summary>
        protected override void OnDestroy()
        {
            OnCleanup();
            base.OnDestroy(); // MonoSingletonの処理も呼び出す
        }
    }
} 