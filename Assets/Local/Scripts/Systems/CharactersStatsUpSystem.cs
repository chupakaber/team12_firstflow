using System.Collections.Generic;

namespace Scripts
{
    public class CharactersStatsUpSystem: ISystem
    {
        public List<Character> Characters;

        public void EventCatch(AddItemEvent newEvent)
        {
            if (newEvent.ItemType == Enums.ItemType.BOTTLE_HERO)
            {
                foreach (Character character in Characters)
                {
                    if (character.CharacterType == Enums.CharacterType.PLAYER)
                    {
                        if(character.Items.GetAmount(Enums.ItemType.BOTTLE_HERO) > 0) 
                        {
                            character.TierLvlUp();
                            character.Items.RemoveItem(Enums.ItemType.BOTTLE_HERO, 1);
                        }
                    }
                }
            }
        }

        public void NpcStatsUp(Character character, Character npc)
        {
            if (character.Items.GetAmount(Enums.ItemType.BOTTLE_WORKER) > 0)
            {
                character.Items.RemoveItem(Enums.ItemType.BOTTLE_WORKER, 1);
                npc.TierLvlUp();
            }
        }
    }
}
