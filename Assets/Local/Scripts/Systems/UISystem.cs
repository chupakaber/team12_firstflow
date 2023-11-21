using System.Collections.Generic;

namespace Scripts
{
    internal class UISystem: ISystem
    {
        public List<Character> Characters;
        public UIView UIView;

        public void EventCatch(AddItemEvent newEvent)
        {
            UpdateGoldAndHonor(newEvent.Unit);
        }

        public void EventCatch(RemoveItemEvent newEvent)
        {
            UpdateGoldAndHonor(newEvent.Unit);
        }

        private void UpdateGoldAndHonor(Unit unit)
        {
            if (unit is Character)
            {
                var character = (Character)unit;

                if (character.CharacterType == Enums.CharacterType.PLAYER)
                {
                    var goldCount = character.Items.GetAmount(Enums.ItemType.GOLD);

                    UIView.SetGold(goldCount);

                    var honorCount = character.Items.GetAmount(Enums.ItemType.HONOR);

                    UIView.SetHonor(honorCount);
                }
            }
        }
    }
}
