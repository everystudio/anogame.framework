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
        private readonly Stack<IPage> _pageStack = new Stack<IPage>();
        private readonly Dictionary<string, IPage> _registeredPages = new Dictionary<string, IPage>();
        
        /// <summary>
        /// 現在のページ
        /// </summary>
        public IPage CurrentPage => _pageStack.Count > 0 ? _pageStack.Peek() : null;
        
        /// <summary>
        /// ページ変更イベント
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
            
            if (_registeredPages.ContainsKey(page.PageId))
            {
                Debug.LogWarning($"[PageManager] Page '{page.PageId}' は既に登録されています");
                return;
            }
            
            _registeredPages[page.PageId] = page;
            
            // ページ遷移イベントを購読
            page.OnPageTransition += OnPageTransitionRequested;
            
            Debug.Log($"[PageManager] Page '{page.PageId}' を登録しました");
        }
        
        /// <summary>
        /// ページの登録を解除する
        /// </summary>
        /// <param name="pageId">解除するページID</param>
        public void UnregisterPage(string pageId)
        {
            if (_registeredPages.TryGetValue(pageId, out var page))
            {
                page.OnPageTransition -= OnPageTransitionRequested;
                _registeredPages.Remove(pageId);
                Debug.Log($"[PageManager] Page '{pageId}' の登録を解除しました");
            }
        }
        
        /// <summary>
        /// ページに遷移する（スタックをクリア）
        /// </summary>
        /// <param name="pageId">遷移先のページID</param>
        public void NavigateToPage(string pageId)
        {
            var targetPage = GetPage(pageId);
            if (targetPage == null) return;
            
            var currentPage = CurrentPage;
            
            // 現在のページを終了
            currentPage?.Exit();
            
            // スタックをクリア
            _pageStack.Clear();
            
            // 新しいページを開始
            _pageStack.Push(targetPage);
            targetPage.Enter();
            
            OnPageChanged?.Invoke(currentPage, targetPage);
            Debug.Log($"[PageManager] Page '{pageId}' に遷移しました");
        }
        
        /// <summary>
        /// ページをプッシュする（現在のページの上に重ねる）
        /// </summary>
        /// <param name="pageId">プッシュするページID</param>
        public void PushPage(string pageId)
        {
            var targetPage = GetPage(pageId);
            if (targetPage == null) return;
            
            var currentPage = CurrentPage;
            
            // 現在のページは隠すだけ（終了はしない）
            currentPage?.Hide();
            
            // 新しいページをスタックにプッシュ
            _pageStack.Push(targetPage);
            targetPage.Enter();
            
            OnPageChanged?.Invoke(currentPage, targetPage);
            Debug.Log($"[PageManager] Page '{pageId}' をプッシュしました");
        }
        
        /// <summary>
        /// 現在のページをポップする（前のページに戻る）
        /// </summary>
        /// <returns>戻ることができたかどうか</returns>
        public bool PopPage()
        {
            if (_pageStack.Count <= 1)
            {
                Debug.LogWarning("[PageManager] 戻るページがありません");
                return false;
            }
            
            var currentPage = _pageStack.Pop();
            var previousPage = CurrentPage;
            
            // 現在のページに戻る処理を実行してもらう
            if (!currentPage.OnBackPressed())
            {
                // 戻る処理をキャンセルされた場合は元に戻す
                _pageStack.Push(currentPage);
                return false;
            }
            
            // 現在のページを終了
            currentPage.Exit();
            
            // 前のページを再表示
            previousPage?.Show();
            
            OnPageChanged?.Invoke(currentPage, previousPage);
            Debug.Log($"[PageManager] Page '{currentPage.PageId}' から戻りました");
            return true;
        }
        
        /// <summary>
        /// 戻るボタンが押された時の処理
        /// </summary>
        public void OnBackButtonPressed()
        {
            PopPage();
        }
        
        /// <summary>
        /// 登録されているページを取得する
        /// </summary>
        /// <param name="pageId">ページID</param>
        /// <returns>ページインスタンス</returns>
        private IPage GetPage(string pageId)
        {
            if (!_registeredPages.TryGetValue(pageId, out var page))
            {
                Debug.LogError($"[PageManager] Page '{pageId}' が見つかりません");
                return null;
            }
            return page;
        }
        
        /// <summary>
        /// ページからの遷移要求を処理
        /// </summary>
        /// <param name="targetPage">遷移先のページ</param>
        private void OnPageTransitionRequested(IPage targetPage)
        {
            if (targetPage != null)
            {
                PushPage(targetPage.PageId);
            }
        }
        
        /// <summary>
        /// 全ページの一覧を取得
        /// </summary>
        /// <returns>登録されているページの一覧</returns>
        public IReadOnlyList<IPage> GetAllPages()
        {
            return _registeredPages.Values.ToList();
        }
        
        /// <summary>
        /// ページスタックの状態をログ出力
        /// </summary>
        [ContextMenu("Debug Page Stack")]
        public void DebugPageStack()
        {
            Debug.Log($"[PageManager] Page Stack: {string.Join(" -> ", _pageStack.Select(p => p.PageId))}");
        }
    }
} 