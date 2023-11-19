using Scripts.Enums;
using UnityEngine;

namespace Scripts.Systems
{
    public class AddItemSystem: ISystem
    {
        public void EventCatch(AddItemEvent newEvent)
        {            
            newEvent.Unit.AddItem(newEvent.ItemType, newEvent.Count);

            var itemPrefab = Resources.Load<GameObject>($"Prefabs/{newEvent.ItemType.ToString()}");
            
            for (var i = 0; i < newEvent.Count; i++)
            {
                var item = Object.Instantiate(itemPrefab);
                var itemView = item.GetComponent<ItemView>(); 
                newEvent.Unit.ItemStackView.AddItem(itemView);
            }
        }
    }
}
