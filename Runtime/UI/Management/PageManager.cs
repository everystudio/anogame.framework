using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// ページ遷移管理マネージャー
    /// </summary>
    public class PageManager : MonoSingleton<PageManager>
    {
        private readonly Stack<IPage> pageStack = new Stack<IPage>();
        private readonly Dictionary<string, IPage> registeredPages = new Dictionary<string, IPage>();
        
        [Header("ページ管理設定")]
        [SerializeField] private Transform pageContainer; // オプション：ページの親コンテナ
        [SerializeField] private bool moveToContainer = false; // 親を移動するかどうか
        
        /// <summary>
        /// 現在のアクティブページ
        /// </summary>
        public IPage CurrentPage => pageStack.Count > 0 ? pageStack.Peek() : null;
        
        /// <summary>
        /// ページ遷移イベント（遷移先ページ, 遷移元ページ）
        /// </summary>
        public event Action<IPage, IPage> OnPageChanged;
        
        /// <summary>
        /// ページを登録する
        /// </summary>
        /// <param name="page">登録するページ</param>
        public void RegisterPage(IPage page)
        {
            if (page == null)
            {
                Debug.LogError("[PageManager] 登録するPageがnullです");
                return;
            }
            
            if (registeredPages.ContainsKey(page.PageId))
            {
                Debug.LogWarning($"[PageManager] Page '{page.PageId}' は既に登録されています");
                return;
            }
            
            registeredPages[page.PageId] = page;
            
            // ページ遷移要求イベントを購読
            page.OnPageTransition += OnPageTransitionRequested;
            
            // オプション：ページを統一コンテナに移動
            if (moveToContainer && pageContainer != null && page.GameObject != null)
            {
                page.GameObject.transform.SetParent(pageContainer, false);
                Debug.Log($"[PageManager] Page '{page.PageId}' を統一コンテナに移動しました");
            }
            
            Debug.Log($"[PageManager] Page '{page.PageId}' を登録しました");
        }
        
        /// <summary>
        /// ページの登録を解除する
        /// </summary>
        /// <param name="pageId">解除するページID</param>
        public void UnregisterPage(string pageId)
        {
            if (registeredPages.TryGetValue(pageId, out var page))
            {
                page.OnPageTransition -= OnPageTransitionRequested;
                registeredPages.Remove(pageId);
                
                // スタックからも削除
                if (pageStack.Contains(page))
                {
                    var tempStack = new Stack<IPage>();
                    while (pageStack.Count > 0)
                    {
                        var p = pageStack.Pop();
                        if (p != page)
                        {
                            tempStack.Push(p);
                        }
                    }
                    while (tempStack.Count > 0)
                    {
                        pageStack.Push(tempStack.Pop());
                    }
                }
                
                Debug.Log($"[PageManager] Page '{pageId}' の登録を解除しました");
            }
        }
        
        /// <summary>
        /// 指定ページに遷移する（スタックを置き換え）
        /// </summary>
        /// <param name="pageId">遷移先ページID</param>
        public void NavigateToPage(string pageId)
        {
            var page = GetPage(pageId);
            if (page == null) return;
            
            var previousPage = CurrentPage;
            
            // 現在のページをすべて終了
            while (pageStack.Count > 0)
            {
                var currentPage = pageStack.Pop();
                currentPage.Exit();
                currentPage.Hide();
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
        public void PushPage(string pageId)
        {
            var page = GetPage(pageId);
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
        /// 登録されているページを取得する
        /// </summary>
        /// <param name="pageId">ページID</param>
        /// <returns>ページインスタンス</returns>
        private IPage GetPage(string pageId)
        {
            if (!registeredPages.TryGetValue(pageId, out var page))
            {
                Debug.LogError($"[PageManager] Page '{pageId}' が見つかりません");
                return null;
            }
            return page;
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
                pages.Add(page.PageId);
            }
            Debug.Log($"[PageManager] Page Stack: {string.Join(" -> ", pages)}");
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
            foreach (var page in registeredPages.Values)
            {
                page.OnPageTransition -= OnPageTransitionRequested;
            }
            registeredPages.Clear();
            pageStack.Clear();
            Debug.Log("[PageManager] 全てのページの登録を解除しました");
        }
    }
} 