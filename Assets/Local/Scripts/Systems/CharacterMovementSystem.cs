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
                var worldDirection = character.WorldDirection;
                var horizontalDirection = worldDirection;
                horizontalDirection.y = 0f;

                var origin = character.transform.position + Vector3.up * 0.75f;
                var castDirection = horizontalDirection * 0.3f - Vector3.up * 0.75f;
                var hasHitGround = false;
                if (Physics.Raycast(origin, castDirection.normalized, out var hitInfo, 1.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    worldDirection = hitInfo.point - character.transform.position;
                    var worldDirectionY = Mathf.Sign(worldDirection.y) * Mathf.Min(Mathf.Abs(worldDirection.y), 1f);
                    worldDirection = worldDirection.normalized * Mathf.Min(character.WorldDirection.magnitude, 1f);
                    worldDirection.y = worldDirectionY;
                    hasHitGround = true;
                }

                var characterRigidbody = character.CharacterRigidbody;
                var newCharacterVelocity = worldDirection * character.Speed;
                if (!hasHitGround)
                {
                    newCharacterVelocity.y = characterRigidbody.velocity.y;
                }

                //newCharacterVelocity.y = Mathf.Lerp(characterRigidbody.velocity.y, newCharacterVelocity.y, 0.5f);
                //newCharacterVelocity.y += characterRigidbody.velocity.y;
                characterRigidbody.velocity = Vector3.Lerp(characterRigidbody.velocity, newCharacterVelocity, Time.fixedDeltaTime * 8f);

                if (horizontalDirection.sqrMagnitude > 0.1f)
                {
                    character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.LookRotation(horizontalDirection), Time.fixedDeltaTime * 10f);
                    character.ClearDropItemCooldown();
                    character.ClearPickUpItemCooldown();
                }

                if (character.CharacterType == Enums.CharacterType.PLAYER && character.WorldDirection.sqrMagnitude <= float.Epsilon)
                {
                    character.CharacterRigidbody.velocity *= 0f;
                }
            }
        }
    }
}
