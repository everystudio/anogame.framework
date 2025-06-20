using System;

namespace anogame.framework.UI
{
    /// <summary>
    /// モーダルインターフェース
    /// ページ内からポップアップで表示され、重ね表示も可能なUI要素
    /// </summary>
    public interface IModal : IUIView
    {
        /// <summary>
        /// モーダルID
        /// </summary>
        string ModalId { get; }
        
        /// <summary>
        /// 表示順序（高いほど前面）
        /// </summary>
        int SortOrder { get; set; }
        
        /// <summary>
        /// 背景をクリックして閉じることができるか
        /// </summary>
        bool CanCloseByBackgroundClick { get; }
        
        /// <summary>
        /// ESCキーで閉じることができるか
        /// </summary>
        bool CanCloseByEscapeKey { get; }
        
        /// <summary>
        /// モーダルを開く
        /// </summary>
        void Open();
        
        /// <summary>
        /// モーダルを閉じる
        /// </summary>
        void Close();
        
        /// <summary>
        /// モーダルが開かれた時に呼ばれる
        /// </summary>
        void OnOpen();
        
        /// <summary>
        /// モーダルが閉じられる前に呼ばれる
        /// </summary>
        /// <returns>閉じることを許可するかどうか</returns>
        bool OnClosing();
        
        /// <summary>
        /// モーダルが閉じられた時に呼ばれる
        /// </summary>
        void OnClose();
        
        /// <summary>
        /// モーダル閉じる要求イベント
        /// </summary>
        event Action<IModal> OnCloseRequested;
    }
} 