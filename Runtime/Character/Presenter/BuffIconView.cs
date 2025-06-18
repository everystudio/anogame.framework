// Packages/anogame.framework/Runtime/UI/BuffIconView.cs
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

namespace anogame.framework
{
    public class BuffIconView : MonoBehaviour
    {
        [SerializeField] private Image icon;
#if TMP_PRESENT
        [SerializeField] private TMP_Text turnText;
#endif
        public void Set(Sprite sprite, int remainingTurns)
        {
            icon.sprite = sprite;
#if TMP_PRESENT
            turnText.text = remainingTurns.ToString();
#endif
        }
    }
}
