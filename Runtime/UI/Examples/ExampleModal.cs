using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace anogame.framework.UI
{
    /// <summary>
    /// モーダルの使用例
    /// </summary>
    public class ExampleModal : ModalBase
    {
        [Header("UI要素")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button openSubModalButton;
        [SerializeField] private Button backgroundButton;
        [SerializeField] private TextMeshProUGUI modalTitle;
        [SerializeField] private TextMeshProUGUI modalContent;
        
        [Header("設定")]
        [SerializeField] private string subModalId;
        [SerializeField] private string modalMessage = "これはサンプルモーダルです";
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // UIの初期設定
            if (modalTitle != null)
            {
                modalTitle.text = $"Modal: {ModalId}";
            }
            
            if (modalContent != null)
            {
                modalContent.text = modalMessage;
            }
            
            // ボタンイベントの設定
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(RequestClose);
            }
            
            if (openSubModalButton != null)
            {
                openSubModalButton.onClick.AddListener(OpenSubModal);
            }
            
            if (backgroundButton != null)
            {
                backgroundButton.onClick.AddListener(OnBackgroundClick);
            }
        }
        
        public override void OnOpen()
        {
            Debug.Log($"[ExampleModal] '{ModalId}' が開かれました");
            
            // 開く時のアニメーションなどをここに実装
            if (transform is RectTransform rectTransform)
            {
                rectTransform.localScale = Vector3.one * 0.8f;
                // 簡単なスケールアニメーション（LeanTweenの代わりにCoroutineを使用）
                StartCoroutine(ScaleAnimation(rectTransform));
            }
        }
        
        private System.Collections.IEnumerator ScaleAnimation(RectTransform rectTransform)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = rectTransform.localScale;
            Vector3 targetScale = Vector3.one;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // イーズアウトバック風の補間
                t = 1f - Mathf.Pow(1f - t, 3f);
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            rectTransform.localScale = targetScale;
        }
        
        public override void OnClose()
        {
            Debug.Log($"[ExampleModal] '{ModalId}' が閉じられました");
            
            // 閉じる時のアニメーションなどをここに実装
        }
        
        public override bool OnClosing()
        {
            Debug.Log($"[ExampleModal] '{ModalId}' を閉じようとしています");
            
            // 閉じる前の確認処理
            // falseを返すと閉じる処理をキャンセルできる
            return true;
        }
        
        private void OpenSubModal()
        {
            if (!string.IsNullOrEmpty(subModalId))
            {
                ModalManager.Instance.OpenModal(subModalId);
            }
        }
        
        /// <summary>
        /// ModalIDを動的に設定する（テスト用）
        /// </summary>
        /// <param name="modalId">設定するモーダルID</param>
        public new void SetModalId(string modalId)
        {
            base.SetModalId(modalId);
            
            // タイトルも更新
            if (modalTitle != null)
            {
                modalTitle.text = $"Modal: {ModalId}";
            }
        }
        
        /// <summary>
        /// モーダルメッセージを動的に設定する
        /// </summary>
        /// <param name="message">設定するメッセージ</param>
        public void SetModalMessage(string message)
        {
            modalMessage = message;
            
            // コンテンツも更新
            if (modalContent != null)
            {
                modalContent.text = message;
            }
        }
        
        protected override void OnCleanup()
        {
            // ボタンイベントのクリーンアップ
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(RequestClose);
            }
            
            if (openSubModalButton != null)
            {
                openSubModalButton.onClick.RemoveListener(OpenSubModal);
            }
            
            if (backgroundButton != null)
            {
                backgroundButton.onClick.RemoveListener(OnBackgroundClick);
            }
            
            base.OnCleanup();
        }
    }
} 