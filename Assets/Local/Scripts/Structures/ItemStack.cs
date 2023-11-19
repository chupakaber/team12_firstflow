using Scripts.Enums;
using System.Collections;
using System.Collections.Generic;

namespace Scripts
{
    public class ItemStack: IEnumerable<Item>
    {
        private List<Item> _items = new List<Item>();

        public void AddItem(ItemType type, int amount = 1)
        {
            foreach(var item in _items)
            {
                if (type.Equals(item.Type))
                {
                    item.Amount += amount;
                    return;
                }
            }
            var newItem = new Item() { Amount = amount, Type = type};
            _items.Add(newItem);
        }

        public bool RemoveItem(ItemType type, int amount = 1)
        {
            foreach (var item in _items)
            {
                if (type.Equals(item.Type))
                {
                    if(item.Amount >= amount) 
                    { 
                        item.Amount -= amount;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }            
            }
            return false;
        }

        public int GetAmount(ItemType type)
        {
            foreach (var item in _items)
            {
                if (type.Equals(item.Type))
                {
                    return item.Amount;
                }
            }
            return 0; 
        }

        public int GetAmount()
        {
            var amount = 0;
            foreach (var item in _items)
            {
                amount += item.Amount;
            }
            return amount;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public IEnumerator<Item> GetEnumerator()
        {
             return _items.GetEnumerator();
        }
    }
    public class Item
    {
        public int Amount;
        public ItemType Type;
    }

    
}
