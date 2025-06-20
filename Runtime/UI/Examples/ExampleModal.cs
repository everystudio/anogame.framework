using UnityEngine;
using UnityEngine.UI;

namespace anogame.framework.UI.Examples
{
    /// <summary>
    /// モーダルの使用例
    /// </summary>
    public class ExampleModal : ModalBase
    {
        [Header("UI要素")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openSubModalButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Text _modalTitle;
        [SerializeField] private Text _modalContent;
        
        [Header("設定")]
        [SerializeField] private string _subModalId;
        [SerializeField] private string _modalMessage = "これはサンプルモーダルです";
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // ボタンイベントの設定
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
            
            if (_openSubModalButton != null)
            {
                _openSubModalButton.onClick.AddListener(OnOpenSubModalButtonClicked);
            }
            
            if (_backgroundButton != null)
            {
                _backgroundButton.onClick.AddListener(OnBackgroundClick);
            }
        }
        
        public override void OnOpen()
        {
            base.OnOpen();
            
            // モーダルタイトルとコンテンツの設定
            if (_modalTitle != null)
            {
                _modalTitle.text = $"モーダル: {ModalId}";
            }
            
            if (_modalContent != null)
            {
                _modalContent.text = _modalMessage;
            }
            
            Debug.Log($"[ExampleModal] モーダル '{ModalId}' を開きました");
        }
        
        public override bool OnClosing()
        {
            Debug.Log($"[ExampleModal] モーダル '{ModalId}' の閉じる処理が要求されました");
            
            // 例：保存確認などの処理をここで行う
            // 閉じることを許可する場合は true を返す
            // 閉じることを拒否する場合は false を返す
            return true;
        }
        
        public override void OnClose()
        {
            Debug.Log($"[ExampleModal] モーダル '{ModalId}' を閉じました");
            base.OnClose();
        }
        
        public override void OnShow()
        {
            Debug.Log($"[ExampleModal] モーダル '{ModalId}' を表示しました");
        }
        
        public override void OnHide()
        {
            Debug.Log($"[ExampleModal] モーダル '{ModalId}' を非表示にしました");
        }
        
        /// <summary>
        /// 閉じるボタンが押された時の処理
        /// </summary>
        private void OnCloseButtonClicked()
        {
            RequestClose();
        }
        
        /// <summary>
        /// サブモーダルを開くボタンが押された時の処理
        /// </summary>
        private void OnOpenSubModalButtonClicked()
        {
            if (!string.IsNullOrEmpty(_subModalId))
            {
                ModalManager.Instance.OpenModal(_subModalId);
            }
        }
        
        /// <summary>
        /// カスタムメッセージを設定
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        public void SetMessage(string message)
        {
            _modalMessage = message;
            if (_modalContent != null)
            {
                _modalContent.text = message;
            }
        }
        
        /// <summary>
        /// 背景クリック処理のオーバーライド例
        /// </summary>
        public override void OnBackgroundClick()
        {
            Debug.Log($"[ExampleModal] モーダル '{ModalId}' の背景がクリックされました");
            base.OnBackgroundClick(); // 基底クラスの処理を呼び出す
        }
        
        protected override void Update()
        {
            base.Update();
            
            // カスタムキー処理の例
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Enterキーでも閉じることができる
                RequestClose();
            }
        }
        
        protected override void OnCleanup()
        {
            // ボタンイベントのクリーンアップ
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            }
            
            if (_openSubModalButton != null)
            {
                _openSubModalButton.onClick.RemoveListener(OnOpenSubModalButtonClicked);
            }
            
            if (_backgroundButton != null)
            {
                _backgroundButton.onClick.RemoveListener(OnBackgroundClick);
            }
            
            base.OnCleanup();
        }
    }
} 