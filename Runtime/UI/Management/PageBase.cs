using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// ページの基底クラス
    /// </summary>
    public abstract class PageBase : UIViewBase, IPage
    {
        [SerializeField] private string _pageId;
        
        public string PageId => _pageId;
        
        public event Action<IPage> OnPageTransition;
        
        /// <summary>
        /// ページに入る
        /// </summary>
        public virtual void Enter()
        {
            OnEnter();
        }
        
        /// <summary>
        /// ページから出る
        /// </summary>
        public virtual void Exit()
        {
            OnExit();
        }
        
        /// <summary>
        /// ページに入った時に呼ばれる
        /// </summary>
        public virtual void OnEnter()
        {
            Show();
        }
        
        /// <summary>
        /// ページから出る時に呼ばれる
        /// </summary>
        public virtual void OnExit()
        {
            Hide();
        }
        
        /// <summary>
        /// 戻るボタンが押された時の処理
        /// デフォルトでは戻る処理を許可
        /// </summary>
        /// <returns>戻る処理を実行するかどうか</returns>
        public virtual bool OnBackPressed()
        {
            return true;
        }
        
        /// <summary>
        /// 他のページへの遷移を要求
        /// </summary>
        /// <param name="targetPage">遷移先のページ</param>
        protected void RequestPageTransition(IPage targetPage)
        {
            OnPageTransition?.Invoke(targetPage);
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // PageIdが設定されていない場合はGameObject名を使用
            if (string.IsNullOrEmpty(_pageId))
            {
                _pageId = gameObject.name;
            }
        }
    }
} 