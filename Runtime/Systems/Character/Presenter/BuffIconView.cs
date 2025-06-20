using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace anogame.framework
{
    public class BuffIconView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI turnText;
        public void Set(Sprite sprite, int remainingTurns)
        {
            icon.sprite = sprite;
            
            if (turnText != null)
            {
                turnText.text = remainingTurns.ToString();
            }
        }
    }
} 