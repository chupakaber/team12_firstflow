namespace Scripts
{
    public class PlayerMovementSystem: ISystem
    {
        public Character Character;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            var newCharacterVelocity = Character.WorldDirection * Character.Speed;
            newCharacterVelocity.y = Character.CharacterRigidbody.velocity.y;

            Character.CharacterRigidbody.velocity = newCharacterVelocity;
        }
    }
}
