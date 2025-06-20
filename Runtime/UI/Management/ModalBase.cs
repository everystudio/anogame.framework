using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// モーダルの基底クラス
    /// </summary>
    public abstract class ModalBase : UIViewBase, IModal
    {
        [SerializeField] private string _modalId;
        [SerializeField] private int _sortOrder = 0;
        [SerializeField] private bool _canCloseByBackgroundClick = true;
        [SerializeField] private bool _canCloseByEscapeKey = true;
        
        public string ModalId => _modalId;
        public int SortOrder { get => _sortOrder; set => _sortOrder = value; }
        public bool CanCloseByBackgroundClick => _canCloseByBackgroundClick;
        public bool CanCloseByEscapeKey => _canCloseByEscapeKey;
        
        public event Action<IModal> OnCloseRequested;
        
        /// <summary>
        /// モーダルを開く
        /// </summary>
        public virtual void Open()
        {
            Show();
            OnOpen();
        }
        
        /// <summary>
        /// モーダルを閉じる
        /// </summary>
        public virtual void Close()
        {
            if (OnClosing())
            {
                OnClose();
                Hide();
            }
        }
        
        /// <summary>
        /// モーダルが開かれた時に呼ばれる
        /// </summary>
        public virtual void OnOpen() { }
        
        /// <summary>
        /// モーダルが閉じられる前に呼ばれる
        /// </summary>
        /// <returns>閉じることを許可するかどうか</returns>
        public virtual bool OnClosing()
        {
            return true;
        }
        
        /// <summary>
        /// モーダルが閉じられた時に呼ばれる
        /// </summary>
        public virtual void OnClose() { }
        
        /// <summary>
        /// 閉じる要求を送信
        /// </summary>
        protected void RequestClose()
        {
            OnCloseRequested?.Invoke(this);
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // ModalIdが設定されていない場合はGameObject名を使用
            if (string.IsNullOrEmpty(_modalId))
            {
                _modalId = gameObject.name;
            }
        }
        
        protected virtual void Update()
        {
            // ESCキーでの閉じる処理
            if (_canCloseByEscapeKey && Input.GetKeyDown(KeyCode.Escape))
            {
                RequestClose();
            }
        }
        
        /// <summary>
        /// 背景クリック時の処理（UIから呼び出される）
        /// </summary>
        public virtual void OnBackgroundClick()
        {
            if (_canCloseByBackgroundClick)
            {
                RequestClose();
            }
        }
    }
} 