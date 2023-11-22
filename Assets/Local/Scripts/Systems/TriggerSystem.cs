using System.Collections.Generic;

namespace Scripts
{
    public class TriggerSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (Character character in Characters)
            {
                while (character.EnterColliders.TryPop(out var collider))
                {
                    EventBus.CallEvent(new EnterTriggerEvent() { Character = character, Trigger = collider });
                }

                while (character.ExitColliders.TryPop(out var collider))
                {
                    EventBus.CallEvent(new ExitTriggerEvent() { Character = character, Trigger = collider });
                }
            }
        }
    }
}
