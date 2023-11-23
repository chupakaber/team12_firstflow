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
                var characterRigidbody = character.CharacterRigidbody;
                var newCharacterVelocity = character.WorldDirection * character.Speed;
                newCharacterVelocity.y = characterRigidbody.velocity.y;
                characterRigidbody.velocity = Vector3.Lerp(characterRigidbody.velocity, newCharacterVelocity, Time.deltaTime * 5f);

                if (character.WorldDirection.sqrMagnitude > 0.1f)
                {
                    character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.LookRotation(character.WorldDirection), Time.fixedDeltaTime * 10f);
                }
            }
        }
    }
}
