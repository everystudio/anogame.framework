using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// ページの基底クラス
    /// </summary>
    public abstract class PageBase : UIViewBase, IPage
    {
        [SerializeField] private string pageId;
        
        private string instanceId;
        
        public string PageId => pageId;
        public string InstanceId => instanceId;
        
        /// <summary>
        /// PageIDを設定する（主にテスト用）
        /// </summary>
        /// <param name="id">設定するページID</param>
        public void SetPageId(string id)
        {
            pageId = id;
            
            // GameObjectの名前も更新
            UpdateGameObjectName();
            
            Debug.Log($"[PageBase] PageId を '{id}' に設定しました");
        }
        
        /// <summary>
        /// 新しいインスタンスIDを生成する
        /// </summary>
        public void GenerateNewInstanceId()
        {
            instanceId = System.Guid.NewGuid().ToString();
        }
        
        /// <summary>
        /// インスタンスIDを手動設定する
        /// </summary>
        /// <param name="id">設定するインスタンスID</param>
        public void SetInstanceId(string id)
        {
            instanceId = id;
        }
        
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
        /// 戻ることができるかどうか
        /// デフォルトでは戻ることを許可
        /// </summary>
        /// <returns>戻ることが可能な場合はtrue</returns>
        public virtual bool CanGoBack()
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
        
        /// <summary>
        /// GameObjectの名前を更新
        /// </summary>
        private void UpdateGameObjectName()
        {
            if (gameObject != null && !string.IsNullOrEmpty(pageId))
            {
                // インスタンスIDの短縮版（最初の8文字）
                var shortInstanceId = instanceId?.Length > 8 ? instanceId.Substring(0, 8) : instanceId ?? "Unknown";
                
                // 新しい名前を設定
                gameObject.name = $"Page_{pageId}_{shortInstanceId}";
                
                Debug.Log($"[PageBase] GameObjectの名前を '{gameObject.name}' に更新しました");
            }
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // PageIdが設定されていない場合はGameObject名を使用
            if (string.IsNullOrEmpty(pageId))
            {
                pageId = gameObject.name;
            }
            
            // InstanceIdを自動生成
            if (string.IsNullOrEmpty(instanceId))
            {
                GenerateNewInstanceId();
            }
            
            // GameObjectの名前を更新
            UpdateGameObjectName();
        }
    }
} 