using UnityEngine;

namespace Scripts
{
    public class ExitTriggerEvent : IEvent
    {
        public Character Character;
        public Collider Trigger;
    }
}