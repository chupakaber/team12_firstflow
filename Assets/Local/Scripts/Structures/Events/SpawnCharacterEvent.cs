using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class SpawnCharacterEvent: IEvent
    {
        public CharacterType CharacterType;
        public Vector3 Position;
        public SmartCharacter Character;
    }
}
