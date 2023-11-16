namespace Scripts
{
    public class PlayerMovementSystem: ISystem
    {
        public Character Character;
        public void EventCatch(IEvent newEvent)
        {
            if (newEvent is FixedUpdateEvent)
            {
                Movement(Character);
            }
        }

        private void Movement(Character character)
        {
            character.CharacterRigidbody.velocity = character.WorldDirection * character.Speed;
        }
    }
}
