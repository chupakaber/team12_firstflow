using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts
{
    public class BuildingCollectingSystem: ISystem
    {
        public EventBus EventBus;
        public Character Character;

        public List<Building> Buildings;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (Character.CollidedWith != null && Character.CollidedWith.Equals(building.CollectResourceArea))
                {
                    Collecting(building);
                }
            }
        }

        private void Collecting(Building building)
        {            
            if (Time.time >= Character.LastMoveItemTime + 1f)
            {
                var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = 1, Character = Character };
                EventBus.CallEvent(removeItemEvent);
                Character.LastMoveItemTime = Time.time;
            }            
        }
    }
}
