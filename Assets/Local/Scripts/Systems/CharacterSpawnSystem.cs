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
        public PoolCollection<Mercenary> MercenaryPools;

        public void EventCatch(SpawnCharacterEvent newEvent)
        {
            switch (newEvent.CharacterType)
            {
                case CharacterType.ASSISTANT:
                    newEvent.Character = AssistantPools.Get(0);
                    newEvent.Character.FollowingOffset = 2.2f;
                break;
                case CharacterType.APPRENTICE:
                    newEvent.Character = ApprenticePools.Get(0);
                    newEvent.Character.ResizeBagOfTries(newEvent.Character.BaseBagOfTriesCapacity);
                    newEvent.Character.FollowingOffset = 2.2f;
                break;
                case CharacterType.MERCENARY:
                    newEvent.Character = MercenaryPools.Get(0);
                    newEvent.Character.FollowingOffset = 2.2f;
                break;
            }

            if (newEvent.Character != null)
            {
                newEvent.Character.transform.position = newEvent.Position;
            }
        }
    }
}