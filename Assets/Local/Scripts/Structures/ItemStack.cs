using Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class ItemStack: IEnumerable<Item>
    {
        private List<Item> Items = new List<Item>();

        public void AddItem(ItemType type, int amount = 1)
        {
            foreach(var item in Items)
            {
                if (type.Equals(item.Type))
                {
                    item.Amount += amount;
                    return;
                }
            }
            var newItem = new Item() { Amount = amount, Type = type};
            Items.Add(newItem);
        }

        public bool RemoveItem(ItemType type, int amount = 1)
        {
            foreach (var item in Items)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerator<Item> GetEnumerator()
        {
             return Items.GetEnumerator();
        }
    }
        public class Item
        {
            public int Amount;
            public ItemType Type;
        }

    
}
