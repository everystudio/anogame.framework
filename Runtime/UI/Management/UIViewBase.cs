using System;
using System.Collections;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// UI表示要素の基底クラス
    /// </summary>
    public abstract class UIViewBase : MonoBehaviour, IUIView
    {
        [SerializeField] private bool isVisible = false;
        
        [Header("アニメーション設定")]
        [SerializeField] private bool showAnimationSettings = false;
        [SerializeField] private bool useAnimator = true;
        
        // アニメーション詳細設定（showAnimationSettingsがtrueの時のみ表示）
        [SerializeField] private string openTrigger = "Open";
        [SerializeField] private string closeTrigger = "Close";
        [SerializeField] private string isOpenBool = "IsOpen";
        [SerializeField] private float animationTimeout = 2.0f;
        
        private Animator animator;
        private bool isAnimating = false;
        
        public bool IsVisible => isVisible;
        public GameObject GameObject => gameObject;
        public bool IsAnimating => isAnimating;
        
        public event Action<bool> OnVisibilityChanged;
        
        /// <summary>
        /// 表示する
        /// </summary>
        public virtual void Show()
        {
            if (isVisible || isAnimating) return;
            
            isVisible = true;
            gameObject.SetActive(true);
            
            if (useAnimator && animator != null)
            {
                StartCoroutine(ShowWithAnimation());
            }
            else
            {
                // アニメーションなしの場合は即座に表示
                OnShow();
                OnVisibilityChanged?.Invoke(true);
            }
        }
        
        /// <summary>
        /// 非表示にする
        /// </summary>
        public virtual void Hide()
        {
            if (!isVisible || isAnimating) return;
            
            if (useAnimator && animator != null)
            {
                StartCoroutine(HideWithAnimation());
            }
            else
            {
                // アニメーションなしの場合は即座に非表示
                isVisible = false;
                OnHide();
                gameObject.SetActive(false);
                OnVisibilityChanged?.Invoke(false);
            }
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
        /// アニメーション付きで表示
        /// </summary>
        private IEnumerator ShowWithAnimation()
        {
            isAnimating = true;
            
            // Boolパラメータを設定
            if (!string.IsNullOrEmpty(isOpenBool))
            {
                animator.SetBool(isOpenBool, true);
            }
            
            // Triggerパラメータを設定
            if (!string.IsNullOrEmpty(openTrigger))
            {
                animator.SetTrigger(openTrigger);
            }
            
            // アニメーション完了を待つ
            yield return StartCoroutine(WaitForAnimationToComplete("Open"));
            
            isAnimating = false;
            OnShow();
            OnVisibilityChanged?.Invoke(true);
            
            Debug.Log($"[UIViewBase] '{gameObject.name}' の表示アニメーション完了");
        }
        
        /// <summary>
        /// アニメーション付きで非表示
        /// </summary>
        private IEnumerator HideWithAnimation()
        {
            isAnimating = true;
            
            // Boolパラメータを設定
            if (!string.IsNullOrEmpty(isOpenBool))
            {
                animator.SetBool(isOpenBool, false);
            }
            
            // Triggerパラメータを設定
            if (!string.IsNullOrEmpty(closeTrigger))
            {
                animator.SetTrigger(closeTrigger);
            }
            
            // アニメーション完了を待つ
            yield return StartCoroutine(WaitForAnimationToComplete("Close"));
            
            isAnimating = false;
            isVisible = false;
            OnHide();
            gameObject.SetActive(false);
            OnVisibilityChanged?.Invoke(false);
            
            Debug.Log($"[UIViewBase] '{gameObject.name}' の非表示アニメーション完了");
        }
        
        /// <summary>
        /// アニメーション完了を待つ
        /// </summary>
        /// <param name="animationType">アニメーションタイプ（ログ用）</param>
        private IEnumerator WaitForAnimationToComplete(string animationType)
        {
            float timer = 0f;
            
            // 1フレーム待ってからアニメーション状態をチェック
            yield return null;
            
            // アニメーションが再生中の間待機
            while (timer < animationTimeout)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                
                // アニメーションが完了したかチェック
                if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
                {
                    yield break;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // タイムアウト警告
            Debug.LogWarning($"[UIViewBase] '{gameObject.name}' の{animationType}アニメーションがタイムアウトしました");
        }
        
        /// <summary>
        /// UI要素の初期化処理（継承先で実装）
        /// </summary>
        protected virtual void OnInitialize()
        {
            // Animatorを取得
            if (useAnimator)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogWarning($"[UIViewBase] '{gameObject.name}' にAnimatorが見つかりません。アニメーションは無効になります。");
                    useAnimator = false;
                }
                else
                {
                    Debug.Log($"[UIViewBase] '{gameObject.name}' でAnimatorを使用したアニメーションが有効です");
                }
            }
            
            // 初期状態を設定
            gameObject.SetActive(isVisible);
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
        
        /// <summary>
        /// アニメーション設定を変更
        /// </summary>
        /// <param name="enabled">アニメーションを有効にするか</param>
        /// <param name="openTrigger">開くトリガー名</param>
        /// <param name="closeTrigger">閉じるトリガー名</param>
        /// <param name="isOpenBool">開いているかのBool名</param>
        public void SetAnimationSettings(bool enabled, string openTrigger = "Open", string closeTrigger = "Close", string isOpenBool = "IsOpen")
        {
            useAnimator = enabled;
            this.openTrigger = openTrigger;
            this.closeTrigger = closeTrigger;
            this.isOpenBool = isOpenBool;
            
            Debug.Log($"[UIViewBase] '{gameObject.name}' のアニメーション設定を更新しました（有効: {enabled}）");
        }
        
        /// <summary>
        /// 即座に表示（アニメーションなし）
        /// </summary>
        public void ShowImmediate()
        {
            if (isVisible) return;
            
            isVisible = true;
            gameObject.SetActive(true);
            OnShow();
            OnVisibilityChanged?.Invoke(true);
        }
        
        /// <summary>
        /// 即座に非表示（アニメーションなし）
        /// </summary>
        public void HideImmediate()
        {
            if (!isVisible) return;
            
            isVisible = false;
            OnHide();
            gameObject.SetActive(false);
            OnVisibilityChanged?.Invoke(false);
        }
    }
} 