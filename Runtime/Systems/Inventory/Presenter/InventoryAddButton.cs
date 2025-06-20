// Packages/anogame.framework/Runtime/Presenter/InventoryAddButton.cs
using UnityEngine;
using UnityEngine.UI;

namespace anogame.framework
{
    [RequireComponent(typeof(Button))]
    public class InventoryAddButton : MonoBehaviour
    {
        [SerializeField] private ItemDefinition itemToAdd;
        [SerializeField] private int amount = 1;
        [SerializeField] private InventoryPresenter targetPresenter;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (itemToAdd == null || targetPresenter == null)
            {
                Debug.LogWarning("ItemToAdd または TargetPresenter が設定されていません。");
                return;
            }

            // モデルに追加 → UI更新
            targetPresenter.Model.Add(itemToAdd, amount);
            targetPresenter.Refresh();
        }
    }
}
