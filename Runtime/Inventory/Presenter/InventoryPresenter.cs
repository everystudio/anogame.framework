// Packages/anogame.framework/Runtime/Presenter/InventoryPresenter.cs
using UnityEngine;

namespace anogame.framework
{
    public class InventoryPresenter : MonoBehaviour
    {
        [SerializeField] private Transform contentParent;
        [SerializeField] private InventorySlotView slotPrefab;

        private InventoryModel model;
        public InventoryModel Model => model;

        public void SetModel(InventoryModel inventoryModel)
        {
            model = inventoryModel;
            Refresh();
        }

        public void Refresh()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in model.Items)
            {
                var slot = Instantiate(slotPrefab, contentParent);
                slot.Set(item, model, this);
            }
        }
    }
}
