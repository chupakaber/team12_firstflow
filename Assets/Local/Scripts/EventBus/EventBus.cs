using Scripts.Systems;
using System.Collections.Generic;

namespace Scripts
{
    public class EventBus
    {
        public List<ISystem> Systems = new List<ISystem>();
        public void CallEvent(IEvent newEvent)
        {
            foreach (var system in Systems)
            {
                system.EventCatch(newEvent);
            }
        }
    }
}
