using System;

namespace anogame.framework.UI
{
    /// <summary>
    /// シートインターフェース
    /// Page内やModal内でタブ切り替えなどで表示される単位
    /// </summary>
    public interface ISheet : IUIView
    {
        /// <summary>
        /// シートID
        /// </summary>
        string SheetId { get; }
        
        /// <summary>
        /// 親となるUI要素
        /// </summary>
        IUIView Parent { get; set; }
        
        /// <summary>
        /// シートをアクティブにする
        /// </summary>
        void Activate();
        
        /// <summary>
        /// シートを非アクティブにする
        /// </summary>
        void Deactivate();
        
        /// <summary>
        /// シートがアクティブになった時に呼ばれる
        /// </summary>
        void OnActivate();
        
        /// <summary>
        /// シートが非アクティブになった時に呼ばれる
        /// </summary>
        void OnDeactivate();
        
        /// <summary>
        /// アクティブ状態
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// アクティブ状態変更イベント
        /// </summary>
        event Action<bool> OnActiveChanged;
    }
} 