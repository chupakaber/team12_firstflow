namespace Scripts
{
    public class PlayerMovementSystem: ISystem
    {
        public Character Character;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            Character.CharacterRigidbody.velocity = Character.WorldDirection * Character.Speed;
        }
    }
}
