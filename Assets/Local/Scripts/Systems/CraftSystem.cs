using Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Systems
{
    public class CraftSystem: ISystem
    {
        private void AddItem(ItemType item, int count, Character character)
        {
            character.AddItem(item, count);
        }

        public void EventCatch(IEvent newEvent)
        {
            if (newEvent is AddItemEvent)
            {
                var addEvent = (AddItemEvent)newEvent;
                AddItem(addEvent.ItemType, addEvent.Count, addEvent.Character);
            }
        }
    }
}
