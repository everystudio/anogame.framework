using System;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// UI表示要素の基本インターフェース
    /// </summary>
    public interface IUIView
    {
        /// <summary>
        /// 表示状態
        /// </summary>
        bool IsVisible { get; }
        
        /// <summary>
        /// アニメーション中かどうか
        /// </summary>
        bool IsAnimating { get; }
        
        /// <summary>
        /// GameObjectの参照
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// 表示する
        /// </summary>
        void Show();
        
        /// <summary>
        /// 非表示にする
        /// </summary>
        void Hide();
        
        /// <summary>
        /// 表示時に呼ばれる
        /// </summary>
        void OnShow();
        
        /// <summary>
        /// 非表示時に呼ばれる
        /// </summary>
        void OnHide();
        
        /// <summary>
        /// 表示状態変更イベント
        /// </summary>
        event Action<bool> OnVisibilityChanged;
    }
} 