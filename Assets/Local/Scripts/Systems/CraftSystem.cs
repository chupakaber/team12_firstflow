using Scripts.Enums;
using UnityEngine;

namespace Scripts.Systems
{
    public class CraftSystem: ISystem
    {
        public void EventCatch(AddItemEvent newEvent)
        {            
            newEvent.Unit.AddItem(newEvent.ItemType, newEvent.Count);

            var itemPrefab = Resources.Load<GameObject>($"Prefabs/{newEvent.ItemType.ToString()}");

            var item = Object.Instantiate(itemPrefab);

            var itemView = item.GetComponent<ItemView>(); 

            newEvent.Unit.ItemStackView.AddItem(itemView); // ERROR add One Item
        }
    }
}
