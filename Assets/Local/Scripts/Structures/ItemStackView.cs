using Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class ItemStackView: MonoBehaviour
    {
        [SerializeField] private float _offset;

        public int Count { get; private set; }
        public float Offset { get { return _offset; } }

        private List<ItemView> _items = new List<ItemView>();

        public void SortItems()
        {
            var counter = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item.gameObject.activeSelf)
                {
                    item.transform.localPosition = new Vector3(0, i * _offset, 0);
                    counter++;
                }
            }
        }

        public void AddItem(ItemView itemView)
        {
            Count++;

            _items.Add(itemView);

            itemView.transform.SetParent(transform);

            SortItems();
        }

        public void RemoveItem(ItemType itemType, int removeCount)
        {
            Count -= removeCount;
            
            int a = 0;
            for (var i = 0; i < _items.Count; i++) 
            {
                var item = _items[i];
                if (item.ItemType == itemType)
                {
                    _items.Remove(item);
                    item.Release();
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
