using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class CharacterSpawnSystem: ISystem
    {
        public List<Character> Characters;
        public List<Building> Buildings;

        public PoolCollection<Assistant> AssistantPools;
        public PoolCollection<Apprentice> ApprenticePools;

        public void EventCatch(SpawnCharacterEvent newEvent)
        {
            switch (newEvent.CharacterType)
            {
                case CharacterType.ASSISTANT:
                    newEvent.Character = AssistantPools.Get(0);
                break;
                case CharacterType.APPRENTICE:
                    newEvent.Character = ApprenticePools.Get(0);
                break;
            }

            if (newEvent.Character != null)
            {
                newEvent.Character.transform.position = newEvent.Position;
            }
        }
    }
}