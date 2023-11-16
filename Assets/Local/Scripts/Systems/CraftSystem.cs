using Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Systems
{
    public class CraftSystem
    {
        public void AddItem(ItemType item, int count, Character character)
        {
            character.AddItem(item, count);
        }
    }
}
