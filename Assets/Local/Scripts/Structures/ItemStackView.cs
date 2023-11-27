using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class ItemStackView: MonoBehaviour
    {
        public List<ItemView> items = new List<ItemView>();
        [SerializeField] private float offset;

        public void SortItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.transform.localPosition = new Vector3(0, i * offset, 0);
            }
        }

        public void AddItem(ItemView itemView)
        {
            items.Add(itemView);

            itemView.transform.SetParent(transform);

            SortItems();
        }

        public void RemoveItem(ItemType itemType, int removeCount)
        {
            int a = 0;
            for (var i = 0; i < items.Count; i++) 
            {
                var item = items[i];
                if (item.ItemType == itemType)
                {
                    items.Remove(item);
                    Destroy(item.gameObject);
                    SortItems();
                    i--;
                    a++;
                    if(a >= removeCount) 
                    { 
                        return; 
                    }
                }
            }
        }
    }
}
