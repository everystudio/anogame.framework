using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// シートの基底クラス
    /// </summary>
    public abstract class SheetBase : UIViewBase, ISheet
    {
        [SerializeField] private string _sheetId;
        [SerializeField] private bool _isActive = false;
        
        public string SheetId => _sheetId;
        public IUIView Parent { get; set; }
        public bool IsActive => _isActive;
        
        public event Action<bool> OnActiveChanged;
        
        /// <summary>
        /// シートをアクティブにする
        /// </summary>
        public virtual void Activate()
        {
            if (_isActive) return;
            
            _isActive = true;
            OnActivate();
            OnActiveChanged?.Invoke(true);
        }
        
        /// <summary>
        /// シートを非アクティブにする
        /// </summary>
        public virtual void Deactivate()
        {
            if (!_isActive) return;
            
            _isActive = false;
            OnDeactivate();
            OnActiveChanged?.Invoke(false);
        }
        
        /// <summary>
        /// シートがアクティブになった時に呼ばれる
        /// </summary>
        public virtual void OnActivate()
        {
            Show();
        }
        
        /// <summary>
        /// シートが非アクティブになった時に呼ばれる
        /// </summary>
        public virtual void OnDeactivate()
        {
            Hide();
        }
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // SheetIdが設定されていない場合はGameObject名を使用
            if (string.IsNullOrEmpty(_sheetId))
            {
                _sheetId = gameObject.name;
            }
        }
        
        protected virtual void Start()
        {
            // 初期状態でアクティブな場合は処理を実行
            if (_isActive)
            {
                OnActivate();
            }
        }
    }
} 