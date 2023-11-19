using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class CharacterMovementSystem: ISystem
    {
        public List<Character> Characters;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (Character character in Characters)
            {
                var newCharacterVelocity = character.WorldDirection * character.Speed;
                newCharacterVelocity.y = character.CharacterRigidbody.velocity.y;

                character.CharacterRigidbody.velocity = newCharacterVelocity;

                if (character.WorldDirection.sqrMagnitude > 0.1f)
                {
                    character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.LookRotation(character.WorldDirection), Time.fixedDeltaTime * 10f);
                }
            }
        }
    }
}
