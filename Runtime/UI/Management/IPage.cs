using System;

namespace anogame.framework.UI
{
    /// <summary>
    /// ページインターフェース
    /// 遷移機能と戻る機能を持つUI要素
    /// </summary>
    public interface IPage : IUIView
    {
        /// <summary>
        /// ページID
        /// </summary>
        string PageId { get; }
        
        /// <summary>
        /// ページに入る
        /// </summary>
        void Enter();
        
        /// <summary>
        /// ページから出る
        /// </summary>
        void Exit();
        
        /// <summary>
        /// ページに入った時に呼ばれる
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// ページから出る時に呼ばれる
        /// </summary>
        void OnExit();
        
        /// <summary>
        /// 戻るボタンが押された時の処理
        /// </summary>
        /// <returns>戻る処理を実行するかどうか（falseの場合は独自処理）</returns>
        bool OnBackPressed();
        
        /// <summary>
        /// 戻ることができるかどうか
        /// </summary>
        /// <returns>戻ることが可能な場合はtrue</returns>
        bool CanGoBack();
        
        /// <summary>
        /// ページ遷移イベント
        /// </summary>
        event Action<IPage> OnPageTransition;
    }
} 