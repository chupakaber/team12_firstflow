namespace Scripts
{
    public class PlayerMovementSystem
    {
        public void Movement(Character character)
        {
            character.CharacterRigidbody.velocity = character.WorldDirection * character.Speed;
        }
    }
}
