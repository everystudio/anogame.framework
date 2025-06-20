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

        private CharacterStatus _targetStatus;
        private float _currentHPRatio;
        private float _currentMPRatio;

        /// <summary>
        /// 対象キャラクターを設定
        /// </summary>
        public void SetTarget(CharacterStatus status)
        {
            _targetStatus = status;
            if (_targetStatus != null)
            {
                UpdateBarsImmediate();
            }
        }

        private void Update()
        {
            if (_targetStatus != null)
            {
                UpdateBars();
            }
        }

        /// <summary>
        /// バーを即座に更新
        /// </summary>
        private void UpdateBarsImmediate()
        {
            if (_targetStatus == null) return;

            // HP更新
            float targetHPRatio = (float)_targetStatus.CurrentHP / _targetStatus.MaxHP;
            _currentHPRatio = targetHPRatio;
            UpdateHPDisplay();

            // MP更新
            float targetMPRatio = (float)_targetStatus.CurrentMP / _targetStatus.MaxMP;
            _currentMPRatio = targetMPRatio;
            UpdateMPDisplay();
        }

        /// <summary>
        /// バーをアニメーション付きで更新
        /// </summary>
        private void UpdateBars()
        {
            if (_targetStatus == null) return;

            // HP更新
            float targetHPRatio = (float)_targetStatus.CurrentHP / _targetStatus.MaxHP;
            if (Mathf.Abs(_currentHPRatio - targetHPRatio) > 0.001f)
            {
                if (useAnimation)
                {
                    _currentHPRatio = Mathf.MoveTowards(_currentHPRatio, targetHPRatio, 
                        animationSpeed * Time.deltaTime);
                }
                else
                {
                    _currentHPRatio = targetHPRatio;
                }
                UpdateHPDisplay();
            }

            // MP更新
            float targetMPRatio = (float)_targetStatus.CurrentMP / _targetStatus.MaxMP;
            if (Mathf.Abs(_currentMPRatio - targetMPRatio) > 0.001f)
            {
                if (useAnimation)
                {
                    _currentMPRatio = Mathf.MoveTowards(_currentMPRatio, targetMPRatio, 
                        animationSpeed * Time.deltaTime);
                }
                else
                {
                    _currentMPRatio = targetMPRatio;
                }
                UpdateMPDisplay();
            }
        }

        /// <summary>
        /// HP表示を更新
        /// </summary>
        private void UpdateHPDisplay()
        {
            if (hpSlider != null)
            {
                hpSlider.value = _currentHPRatio;
            }

            if (hpText != null)
            {
                hpText.text = $"{_targetStatus.CurrentHP} / {_targetStatus.MaxHP}";
            }

            if (hpFillImage != null)
            {
                // HPが少ない時は色を変更
                Color targetColor = _currentHPRatio <= lowHPThreshold ? lowHPColor : fullHPColor;
                hpFillImage.color = targetColor;
            }
        }

        /// <summary>
        /// MP表示を更新
        /// </summary>
        private void UpdateMPDisplay()
        {
            if (mpSlider != null)
            {
                mpSlider.value = _currentMPRatio;
            }

            if (mpText != null)
            {
                mpText.text = $"{_targetStatus.CurrentMP} / {_targetStatus.MaxMP}";
            }

            if (mpFillImage != null)
            {
                // MPの色設定
                Color targetColor = _currentMPRatio <= 0.3f ? lowMPColor : fullMPColor;
                mpFillImage.color = targetColor;
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