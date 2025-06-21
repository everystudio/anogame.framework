using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// ページ管理システム
    /// </summary>
    public class PageManager : MonoSingleton<PageManager>
    {
        // ページテンプレート管理（PageId -> プレハブ）
        private readonly Dictionary<string, GameObject> pageTemplates = new Dictionary<string, GameObject>();
        // アクティブなページインスタンス管理（InstanceId -> インスタンス）
        private readonly Dictionary<string, IPage> activePages = new Dictionary<string, IPage>();
        private readonly Stack<IPage> pageStack = new Stack<IPage>();

        [Header("ページ管理設定")]
        [SerializeField] private Transform pageContainer; // オプション：ページの親コンテナ
        [SerializeField] private bool moveToContainer = false; // 親を移動するかどうか
        [SerializeField] private bool keepInactivePages = true; // 非アクティブページを保持するか

        /// <summary>
        /// 現在アクティブなページ
        /// </summary>
        public IPage CurrentPage => pageStack.Count > 0 ? pageStack.Peek() : null;

        /// <summary>
        /// ページ変更イベント（新しいページ, 前のページ）
        /// </summary>
        public event Action<IPage, IPage> OnPageChanged;

        /// <summary>
        /// ページテンプレート（プレハブ）を登録する
        /// </summary>
        /// <param name="pageId">ページID</param>
        /// <param name="prefab">ページのプレハブ</param>
        public void RegisterPageTemplate(string pageId, GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("[PageManager] 登録するPrefabがnullです");
                return;
            }

            if (pageTemplates.ContainsKey(pageId))
            {
                Debug.LogWarning($"[PageManager] Page template '{pageId}' は既に登録されています");
                return;
            }

            pageTemplates[pageId] = prefab;
            Debug.Log($"[PageManager] Page template '{pageId}' を登録しました");
        }

        /// <summary>
        /// ページテンプレートの登録を解除する
        /// </summary>
        /// <param name="pageId">解除するページID</param>
        public void UnregisterPageTemplate(string pageId)
        {
            if (pageTemplates.ContainsKey(pageId))
            {
                pageTemplates.Remove(pageId);

                // このPageIdのアクティブなインスタンスをすべて閉じる
                var instancesToClose = activePages.Values
                    .Where(p => p.PageId == pageId)
                    .ToList();

                foreach (var instance in instancesToClose)
                {
                    DestroyPageInstance(instance);
                }

                Debug.Log($"[PageManager] Page template '{pageId}' の登録を解除しました");
            }
        }

        /// <summary>
        /// 指定ページに遷移する（スタックを置き換え）
        /// </summary>
        /// <param name="pageId">遷移先ページID</param>
        /// <param name="setupAction">ページ初期化アクション</param>
        public void NavigateToPage(string pageId, Action<IPage> setupAction = null)
        {
            IPage page = GetOrCreatePage(pageId, setupAction);
            if (page == null)
            {
                Debug.LogError($"[PageManager] Page '{pageId}' の取得または作成に失敗しました");
                return;
            }

            var previousPage = CurrentPage;

            // 現在のページをすべて終了
            while (pageStack.Count > 0)
            {
                var currentPage = pageStack.Pop();
                currentPage.Exit();
                currentPage.Hide();

                // 履歴として保持しない場合は破棄
                if (!keepInactivePages)
                {
                    DestroyPageInstance(currentPage);
                }
            }

            // 新しいページを開始
            pageStack.Push(page);
            page.Show();
            page.Enter();

            OnPageChanged?.Invoke(page, previousPage);
            Debug.Log($"[PageManager] Page '{pageId}' に遷移しました");
        }

        /// <summary>
        /// 現在のページの上に新しいページを積む
        /// </summary>
        /// <param name="pageId">追加するページID</param>
        /// <param name="setupAction">ページ初期化アクション</param>
        public void PushPage(string pageId, Action<IPage> setupAction = null)
        {
            var page = GetOrCreatePage(pageId, setupAction);
            if (page == null) return;

            var previousPage = CurrentPage;

            // 現在のページを一時停止
            if (previousPage != null)
            {
                previousPage.Hide();
            }

            // 新しいページを開始
            pageStack.Push(page);
            page.Show();
            page.Enter();

            OnPageChanged?.Invoke(page, previousPage);
            Debug.Log($"[PageManager] Page '{pageId}' をプッシュしました（深度: {pageStack.Count}）");
        }

        /// <summary>
        /// Prefabから直接ページを開く
        /// </summary>
        /// <param name="pagePrefab">ページのプレハブ</param>
        /// <param name="setupAction">ページ初期化アクション</param>
        /// <param name="pushMode">プッシュモードかどうか（falseの場合はNavigate）</param>
        public IPage OpenPageFromPrefab(GameObject pagePrefab, Action<IPage> setupAction = null, bool pushMode = false)
        {
            if (pagePrefab == null)
            {
                Debug.LogError("[PageManager] Prefabがnullです");
                return null;
            }

            var page = CreatePageInstance(pagePrefab);
            if (page == null) return null;

            // セットアップアクションを実行
            setupAction?.Invoke(page);

            var previousPage = CurrentPage;

            if (pushMode)
            {
                // プッシュモード
                if (previousPage != null)
                {
                    previousPage.Hide();
                }
                pageStack.Push(page);
            }
            else
            {
                // ナビゲートモード
                while (pageStack.Count > 0)
                {
                    var currentPage = pageStack.Pop();
                    currentPage.Exit();
                    currentPage.Hide();

                    if (!keepInactivePages)
                    {
                        DestroyPageInstance(currentPage);
                    }
                }
                pageStack.Push(page);
            }

            page.Show();
            page.Enter();

            OnPageChanged?.Invoke(page, previousPage);
            Debug.Log($"[PageManager] Prefabから Page '{page.PageId}' を開きました");

            return page;
        }

        /// <summary>
        /// ページインスタンスを取得または作成
        /// </summary>
        /// <param name="pageId">ページID</param>
        /// <param name="setupAction">初期化アクション</param>
        /// <returns>ページインスタンス</returns>
        private IPage GetOrCreatePage(string pageId, Action<IPage> setupAction)
        {
            // 既存のアクティブなページがあるかチェック（履歴として保持されている場合）
            if (keepInactivePages)
            {
                var existingPage = activePages.Values.FirstOrDefault(p => p.PageId == pageId);
                if (existingPage != null)
                {
                    // セットアップアクションを実行
                    setupAction?.Invoke(existingPage);
                    return existingPage;
                }
            }

            // テンプレートから新しいインスタンスを作成
            if (!pageTemplates.TryGetValue(pageId, out var template))
            {
                Debug.LogError($"[PageManager] Page template '{pageId}' が見つかりません");
                return null;
            }

            var page = CreatePageInstance(template, pageId);
            if (page == null) return null;

            // セットアップアクションを実行
            setupAction?.Invoke(page);

            return page;
        }

        /// <summary>
        /// ページインスタンスを作成
        /// </summary>
        /// <param name="template">テンプレート</param>
        /// <param name="pageId">設定するページID（nullの場合はプレファブのPageIdを使用）</param>
        /// <returns>作成されたインスタンス</returns>
        private IPage CreatePageInstance(GameObject template, string pageId = null)
        {
            if (template == null)
            {
                Debug.LogError("[PageManager] Template prefabがnullです");
                return null;
            }

            // 親を決定
            Transform parent = null;
            if (moveToContainer && pageContainer != null)
            {
                parent = pageContainer;
            }
            else
            {
                // pageContainerが設定されていない場合、適切なCanvasを探す
                parent = FindPageCanvas();
            }

            // プレハブをインスタンス化
            var instanceObj = Instantiate(template, parent);
            var pageInstance = instanceObj.GetComponent<IPage>();

            if (pageInstance == null)
            {
                Debug.LogError("[PageManager] インスタンス化されたオブジェクトにIPageコンポーネントがありません");
                Destroy(instanceObj);
                return null;
            }

            // UI要素として適切に配置
            SetupPageTransform(instanceObj, parent);

            // 新しいインスタンスIDを生成
            if (pageInstance is PageBase pageBase)
            {
                pageBase.GenerateNewInstanceId();
                
                // PageIdが指定されている場合は設定
                if (!string.IsNullOrEmpty(pageId))
                {
                    pageBase.SetPageId(pageId);
                    Debug.Log($"[PageManager] Page '{pageId}' のPageIdを設定しました");
                }
                
                // GameObjectの名前を更新
                UpdateGameObjectName(instanceObj, pageInstance.PageId, pageInstance.InstanceId);
            }

            // アクティブなページとして管理
            activePages[pageInstance.InstanceId] = pageInstance;

            // ページ遷移要求イベントを購読
            pageInstance.OnPageTransition += OnPageTransitionRequested;

            Debug.Log($"[PageManager] Page '{pageInstance.PageId}' インスタンス '{pageInstance.InstanceId[..8]}' を作成しました（親: {parent?.name ?? "None"}）");

            return pageInstance;
        }

        /// <summary>
        /// ページ用のCanvasを探す
        /// </summary>
        /// <returns>見つかったCanvas、またはnull</returns>
        private Transform FindPageCanvas()
        {
            // UIManagerのPageCanvasを優先的に使用
            if (UIManager.Instance != null)
            {
                var pageCanvas = UIManager.Instance.GetPageCanvas();
                if (pageCanvas != null)
                {
                    Debug.Log($"[PageManager] UIManagerのPageCanvas '{pageCanvas.name}' を使用します");
                    return pageCanvas.transform;
                }
            }

            // シーン内のCanvasを探す
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"[PageManager] シーン内のCanvas '{canvas.name}' を使用します");
                return canvas.transform;
            }

            Debug.LogWarning("[PageManager] 適切なCanvasが見つかりません。ページが正しく表示されない可能性があります");
            return null;
        }

        /// <summary>
        /// ページのTransformを適切に設定
        /// </summary>
        /// <param name="pageObj">ページのGameObject</param>
        /// <param name="parent">親Transform</param>
        private void SetupPageTransform(GameObject pageObj, Transform parent)
        {
            var rectTransform = pageObj.GetComponent<RectTransform>();
            if (rectTransform != null && parent != null)
            {
                // RectTransformの場合、UI要素として適切に設定
                rectTransform.SetParent(parent, false);
                
                // フルスクリーンに設定（必要に応じて）
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
                
                Debug.Log($"[PageManager] ページのRectTransformを設定しました（親: {parent.name}）");
            }
            else if (parent != null)
            {
                // 通常のTransformの場合
                pageObj.transform.SetParent(parent, false);
                Debug.Log($"[PageManager] ページのTransformを設定しました（親: {parent.name}）");
            }
        }
        
        /// <summary>
        /// GameObjectの名前を更新
        /// </summary>
        /// <param name="gameObj">対象のGameObject</param>
        /// <param name="pageId">ページID</param>
        /// <param name="instanceId">インスタンスID</param>
        private void UpdateGameObjectName(GameObject gameObj, string pageId, string instanceId)
        {
            if (gameObj != null)
            {
                // インスタンスIDの短縮版（最初の8文字）
                var shortInstanceId = instanceId?.Length > 8 ? instanceId.Substring(0, 8) : instanceId;
                
                // 新しい名前を設定
                gameObj.name = $"Page_{pageId}_{shortInstanceId}";
                
                Debug.Log($"[PageManager] GameObjectの名前を '{gameObj.name}' に更新しました");
            }
        }

        /// <summary>
        /// 現在のページを終了して前のページに戻る
        /// </summary>
        /// <returns>戻ることができたかどうか</returns>
        public bool PopPage()
        {
            if (pageStack.Count <= 1)
            {
                Debug.LogWarning("[PageManager] 戻るページがありません");
                return false;
            }

            var currentPage = pageStack.Pop();
            var previousPage = CurrentPage;

            // 現在のページを終了
            currentPage.Exit();
            currentPage.Hide();

            // 履歴として保持しない場合は破棄
            if (!keepInactivePages)
            {
                DestroyPageInstance(currentPage);
            }

            // 前のページを再開
            if (previousPage != null)
            {
                previousPage.Show();
            }

            OnPageChanged?.Invoke(previousPage, currentPage);
            Debug.Log($"[PageManager] Page '{currentPage.PageId}' から '{previousPage?.PageId}' に戻りました");
            return true;
        }

        /// <summary>
        /// ページインスタンスを破棄
        /// </summary>
        /// <param name="page">破棄するページ</param>
        private void DestroyPageInstance(IPage page)
        {
            if (page == null) return;

            // イベント購読解除
            page.OnPageTransition -= OnPageTransitionRequested;

            // アクティブリストから削除
            activePages.Remove(page.InstanceId);

            // GameObjectを破棄
            if (page.GameObject != null)
            {
                Destroy(page.GameObject);
            }
        }

        /// <summary>
        /// 戻るボタンが押された時の処理
        /// </summary>
        public void OnBackButtonPressed()
        {
            if (CurrentPage != null && CurrentPage.CanGoBack())
            {
                PopPage();
            }
        }

        /// <summary>
        /// ページからの遷移要求を処理
        /// </summary>
        /// <param name="page">遷移要求を出したページ</param>
        private void OnPageTransitionRequested(IPage page)
        {
            // ページの遷移ロジックが必要な場合はここに実装
            Debug.Log($"[PageManager] Page '{page.PageId}' から遷移要求を受信しました");
        }

        /// <summary>
        /// ページスタックの状態をログ出力
        /// </summary>
        [ContextMenu("Debug Page Stack")]
        public void DebugPageStack()
        {
            var pages = new List<string>();
            foreach (var page in pageStack)
            {
                pages.Add($"{page.PageId}({page.InstanceId[..8]})");
            }
            Debug.Log($"[PageManager] Page Stack: {string.Join(" -> ", pages)}");
            Debug.Log($"[PageManager] Active Pages: {activePages.Count}");
        }

        /// <summary>
        /// ページスタックの深度を取得
        /// </summary>
        public int PageDepth => pageStack.Count;

        /// <summary>
        /// 全ページの登録を解除
        /// </summary>
        public void UnregisterAllPages()
        {
            foreach (var page in activePages.Values.ToList())
            {
                DestroyPageInstance(page);
            }
            pageTemplates.Clear();
            pageStack.Clear();
            Debug.Log("[PageManager] 全てのページの登録を解除しました");
        }

        /// <summary>
        /// 指定したPageIdのアクティブなページ一覧を取得
        /// </summary>
        /// <param name="pageId">ページID</param>
        /// <returns>該当するページ一覧</returns>
        public IReadOnlyList<IPage> GetActivePagesOfType(string pageId)
        {
            return activePages.Values
                .Where(p => p.PageId == pageId)
                .ToList();
        }

        /// <summary>
        /// ページスタックをクリア（全てのページを閉じる）
        /// </summary>
        public void ClearPageStack()
        {
            while (pageStack.Count > 0)
            {
                var page = pageStack.Pop();
                page.Exit();
                page.Hide();

                // 履歴として保持しない場合は破棄
                if (!keepInactivePages)
                {
                    DestroyPageInstance(page);
                }
            }

            Debug.Log("[PageManager] ページスタックをクリアしました");
        }
    }
}