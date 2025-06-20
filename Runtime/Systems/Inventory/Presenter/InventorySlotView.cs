// Packages/anogame.framework/Runtime/Presenter/InventorySlotView.cs
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

namespace anogame.framework
{
    public class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image icon;
#if TMP_PRESENT
        [SerializeField] private TMP_Text amountText;
#endif
        [SerializeField] private Button useButton;

        private InventoryItem currentItem;
        private InventoryModel model;
        private InventoryPresenter presenter;

        public void Set(InventoryItem item, InventoryModel modelRef, InventoryPresenter presenterRef)
        {
            currentItem = item;
            model = modelRef;
            presenter = presenterRef;

            icon.sprite = item.Item.Icon;
            icon.enabled = item.Item.Icon != null;
#if TMP_PRESENT
            amountText.text = item.Item.IsStackable ? item.Amount.ToString() : "";
#endif

            if (useButton != null)
            {
                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(OnUseClicked);
            }
        }

        private void OnUseClicked()
        {
            if (currentItem == null || model == null)
            {
                return;
            }

            var type = currentItem.Item.ItemType;

            switch (type)
            {
                case ItemType.Consumable:
                    // ConsumableItemとして使用
                    if (currentItem.Item is ConsumableItem consumableItem)
                    {
                        // TODO: 実際のプレイヤーキャラクターを取得する仕組みが必要
                        // 現在はダミーのCharacterStatusを作成
                        var dummyTarget = new CharacterStatus();
                        
                        if (consumableItem.Use(dummyTarget))
                        {
                            model.RemoveItem(currentItem.Item, 1);
                            presenter.Refresh();
                            Debug.Log($"消費アイテム '{currentItem.Item.DisplayName}' を使用しました");
                        }
                        else
                        {
                            Debug.LogWarning($"アイテム '{currentItem.Item.DisplayName}' の使用に失敗しました");
                        }
                    }
                    else
                    {
                        // 従来の消費アイテム処理
                        model.RemoveItem(currentItem.Item, 1);
                        presenter.Refresh();
                        Debug.Log($"消費アイテム '{currentItem.Item.DisplayName}' を使用しました");
                    }
                    break;

                case ItemType.Equipment:
                    // 装備（ここではログのみ）
                    Debug.Log($"装備アイテム '{currentItem.Item.DisplayName}' を装備しました");
                    break;

                case ItemType.KeyItem:
                    // 使用不可
                    Debug.LogWarning($"貴重品 '{currentItem.Item.DisplayName}' は使用できません");
                    break;
            }
        }

    }
}
