using System.Collections.Generic;

namespace Scripts
{
    public class PlayerMovementSystem: ISystem
    {
        public List<Character> Characters;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (Character character in Characters)
            {
                var newCharacterVelocity = character.WorldDirection * character.Speed;
                newCharacterVelocity.y = character.CharacterRigidbody.velocity.y;

                character.CharacterRigidbody.velocity = newCharacterVelocity;
            }
        }
    }
}
