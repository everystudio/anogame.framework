using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace anogame.framework
{
    /// <summary>
    /// HP/MPバーを表示するUIコンポーネント
    /// </summary>
    public class HPMPBar : MonoBehaviour
    {
        [Header("HP設定")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Image hpFillImage;
        [SerializeField] private Color fullHPColor = Color.green;
        [SerializeField] private Color lowHPColor = Color.red;
        [SerializeField] private float lowHPThreshold = 0.3f;

        [Header("MP設定")]
        [SerializeField] private Slider mpSlider;
        [SerializeField] private TextMeshProUGUI mpText;
        [SerializeField] private Image mpFillImage;
        [SerializeField] private Color fullMPColor = Color.blue;
        [SerializeField] private Color lowMPColor = Color.cyan;

        [Header("アニメーション設定")]
        [SerializeField] private float animationSpeed = 2.0f;
        [SerializeField] private bool useAnimation = true;

        private CharacterStatus targetStatus;
        private float currentHPRatio;
        private float currentMPRatio;

        /// <summary>
        /// 対象のキャラクターステータスを設定
        /// </summary>
        /// <param name="status">対象のキャラクターステータス</param>
        public void SetTarget(CharacterStatus status)
        {
            targetStatus = status;
            if (targetStatus != null)
            {
                UpdateImmediate();
            }
        }

        private void Start()
        {
            if (targetStatus != null)
            {
                UpdateImmediate();
            }
        }

        /// <summary>
        /// 即座にUIを更新（アニメーションなし）
        /// </summary>
        public void UpdateImmediate()
        {
            if (targetStatus == null) return;

            // HP
            float targetHPRatio = (float)targetStatus.CurrentHP / targetStatus.MaxHP;
            currentHPRatio = targetHPRatio;
            
            // MP
            if (targetStatus.MaxMP > 0)
            {
                float targetMPRatio = (float)targetStatus.CurrentMP / targetStatus.MaxMP;
                currentMPRatio = targetMPRatio;
            }
            
            UpdateDisplay();
        }

        private void Update()
        {
            if (targetStatus == null) return;

            // HP の更新
            float targetHPRatio = (float)targetStatus.CurrentHP / targetStatus.MaxHP;
            if (Mathf.Abs(currentHPRatio - targetHPRatio) > 0.001f)
            {
                if (useAnimation)
                {
                    currentHPRatio = Mathf.MoveTowards(currentHPRatio, targetHPRatio,
                        animationSpeed * Time.deltaTime);
                }
                else
                {
                    currentHPRatio = targetHPRatio;
                }
            }
            
            // MP の更新
            if (targetStatus.MaxMP > 0)
            {
                float targetMPRatio = (float)targetStatus.CurrentMP / targetStatus.MaxMP;
                if (Mathf.Abs(currentMPRatio - targetMPRatio) > 0.001f)
                {
                    if (useAnimation)
                    {
                        currentMPRatio = Mathf.MoveTowards(currentMPRatio, targetMPRatio,
                            animationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        currentMPRatio = targetMPRatio;
                    }
                }
            }

            UpdateDisplay();
        }

        /// <summary>
        /// 表示を更新
        /// </summary>
        private void UpdateDisplay()
        {
            UpdateHPDisplay();
            UpdateMPDisplay();
        }

        /// <summary>
        /// HP表示を更新
        /// </summary>
        private void UpdateHPDisplay()
        {
            if (hpSlider != null)
            {
                hpSlider.value = currentHPRatio;
            }

            if (hpText != null)
            {
                hpText.text = $"{targetStatus.CurrentHP} / {targetStatus.MaxHP}";
            }

            if (hpSlider != null && hpSlider.fillRect != null)
            {
                Color targetColor = currentHPRatio <= lowHPThreshold ? lowHPColor : fullHPColor;
                if (hpSlider.fillRect.GetComponent<Image>() != null)
                {
                    hpSlider.fillRect.GetComponent<Image>().color = targetColor;
                }
            }
        }

        /// <summary>
        /// MP表示を更新
        /// </summary>
        private void UpdateMPDisplay()
        {
            if (mpSlider != null)
            {
                mpSlider.value = currentMPRatio;
            }

            if (mpText != null)
            {
                mpText.text = $"{targetStatus.CurrentMP} / {targetStatus.MaxMP}";
            }

            if (mpSlider != null && mpSlider.fillRect != null)
            {
                Color targetColor = currentMPRatio <= 0.3f ? lowMPColor : fullMPColor;
                if (mpSlider.fillRect.GetComponent<Image>() != null)
                {
                    mpSlider.fillRect.GetComponent<Image>().color = targetColor;
                }
            }
        }

        /// <summary>
        /// ダメージエフェクトを表示
        /// </summary>
        public void ShowDamageEffect()
        {
            if (hpFillImage != null)
            {
                StartCoroutine(FlashEffect(hpFillImage, Color.white, 0.2f));
            }
        }

        /// <summary>
        /// 回復エフェクトを表示
        /// </summary>
        public void ShowHealEffect()
        {
            if (hpFillImage != null)
            {
                StartCoroutine(FlashEffect(hpFillImage, Color.yellow, 0.2f));
            }
        }

        /// <summary>
        /// フラッシュエフェクト
        /// </summary>
        private System.Collections.IEnumerator FlashEffect(Image target, Color flashColor, float duration)
        {
            Color originalColor = target.color;
            target.color = flashColor;
            
            yield return new WaitForSeconds(duration);
            
            target.color = originalColor;
        }
    }
} 