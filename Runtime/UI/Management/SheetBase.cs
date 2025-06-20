using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// シートの基底クラス
    /// </summary>
    public abstract class SheetBase : UIViewBase, ISheet
    {
        [SerializeField] private string sheetId;
        [SerializeField] private bool isActive = false;
        
        public string SheetId => sheetId;
        public IUIView Parent { get; set; }
        public bool IsActive => isActive;
        
        public event Action<bool> OnActiveChanged;
        
        /// <summary>
        /// シートをアクティブにする
        /// </summary>
        public virtual void Activate()
        {
            if (isActive) return;
            
            isActive = true;
            OnActivate();
            OnActiveChanged?.Invoke(true);
        }
        
        /// <summary>
        /// シートを非アクティブにする
        /// </summary>
        public virtual void Deactivate()
        {
            if (!isActive) return;
            
            isActive = false;
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
            if (string.IsNullOrEmpty(sheetId))
            {
                sheetId = gameObject.name;
            }
        }
        
        protected virtual void Start()
        {
            // 初期状態でアクティブな場合は処理を実行
            if (isActive)
            {
                OnActivate();
            }
        }
    }
} 