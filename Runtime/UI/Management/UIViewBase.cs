using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// UI表示要素の基底クラス
    /// </summary>
    public abstract class UIViewBase : MonoBehaviour, IUIView
    {
        [SerializeField] private bool _isVisible = false;
        
        public bool IsVisible => _isVisible;
        public GameObject GameObject => gameObject;
        
        public event Action<bool> OnVisibilityChanged;
        
        /// <summary>
        /// 表示する
        /// </summary>
        public virtual void Show()
        {
            if (_isVisible) return;
            
            _isVisible = true;
            gameObject.SetActive(true);
            OnShow();
            OnVisibilityChanged?.Invoke(true);
        }
        
        /// <summary>
        /// 非表示にする
        /// </summary>
        public virtual void Hide()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            OnHide();
            gameObject.SetActive(false);
            OnVisibilityChanged?.Invoke(false);
        }
        
        /// <summary>
        /// 表示時に呼ばれる（継承先で実装）
        /// </summary>
        public virtual void OnShow() { }
        
        /// <summary>
        /// 非表示時に呼ばれる（継承先で実装）
        /// </summary>
        public virtual void OnHide() { }
        
        /// <summary>
        /// UI要素の初期化処理（継承先で実装）
        /// </summary>
        protected virtual void OnInitialize()
        {
            // 初期状態を設定
            gameObject.SetActive(_isVisible);
        }
        
        /// <summary>
        /// Unity標準のAwake。OnInitializeを呼び出す
        /// </summary>
        private void Awake()
        {
            OnInitialize();
        }
        
        /// <summary>
        /// UI要素のクリーンアップ処理（継承先で実装）
        /// </summary>
        protected virtual void OnCleanup()
        {
            // 基底クラスでの終了処理
        }
        
        /// <summary>
        /// Unity標準のOnDestroy。OnCleanupを呼び出す
        /// </summary>
        private void OnDestroy()
        {
            OnCleanup();
        }
    }
} 