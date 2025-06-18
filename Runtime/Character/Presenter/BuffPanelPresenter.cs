// Packages/anogame.framework/Runtime/UI/BuffPanelPresenter.cs
using System.Collections.Generic;
using UnityEngine;

namespace anogame.framework
{
    public class BuffPanelPresenter : MonoBehaviour
    {
        [SerializeField] private Transform contentParent;
        [SerializeField] private BuffIconView iconPrefab;

        private List<BuffIconView> views = new();

        public void Refresh(List<Buff> activeBuffs, Dictionary<string, BuffDefinition> definitions)
        {
            // 一旦すべて消す
            foreach (var view in views)
            {
                Destroy(view.gameObject);
            }
            views.Clear();

            foreach (var buff in activeBuffs)
            {
                if (!definitions.TryGetValue(buff.Id, out var def)) continue;

                var view = Instantiate(iconPrefab, contentParent);
                view.Set(def.IconSprite, buff.RemainingTurns);
                views.Add(view);
            }
        }
    }
}
