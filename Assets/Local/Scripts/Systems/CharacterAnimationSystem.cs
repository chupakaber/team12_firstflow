using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class CharacterAnimationSystem: ISystem
    {
        public List<Character> Characters;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (Character character in Characters)
            {
                character.UpdateAnimation();
            }
        }
    }
}
