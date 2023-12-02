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

        public void Clear()
        {
            _items.Clear();
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
                amount += GetItemVolume(item.Type) * item.Amount;
            }
            return amount;
        }

        public int GetItemVolume(ItemType type)
        {
            if (type == ItemType.GOLD || type == ItemType.HONOR || type == ItemType.BOTTLE_HERO || type == ItemType.BOTTLE_WORKER)
            {
                return 0;
            }
            return 1;
        }

        public bool TryGetFirstItem(out ItemType type, out int amount)
        {
            foreach (var item in _items)
            {
                if (item.Amount > 0)
                {
                    type = item.Type;
                    amount = item.Amount;
                    return true;
                }
            }
            type = ItemType.NONE;
            amount = 0;
            return false;
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
}
