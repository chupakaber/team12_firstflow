using Scripts.Enums;
using UnityEngine;

namespace Scripts.Systems
{
    public class CraftSystem: ISystem
    {
        public void EventCatch(AddItemEvent newEvent)
        {            
            newEvent.Character.AddItem(newEvent.ItemType, newEvent.Count);

            var itemPrefab = Resources.Load<GameObject>($"Prefabs/{newEvent.ItemType.ToString()}");

            var item = Object.Instantiate(itemPrefab);

            var itemView = item.GetComponent<ItemView>(); 

            newEvent.Character.ItemStackView.AddItem(itemView); // ERROR add One Item
        }
    }
}
