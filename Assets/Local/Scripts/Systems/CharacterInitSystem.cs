using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class CharacterInitSystem: ISystem
    {
        public EventBus EventBus;

        public List<Character> Characters;

        public void EventCatch(StartEvent newEvent)
        {
            foreach (var character in Characters)
            {
                character.Initialized = true;
            }
        }
    }
}
