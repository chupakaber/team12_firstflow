using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class AlchemicSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(ConstructionCompleteEvent newEvent)
        {
            if (newEvent.Building.ProduceItemType == ItemType.BOTTLE_HERO)
            {
                foreach (var building in Buildings)
                {
                    if (building.ProduceItemType == ItemType.BOTTLE_WORKER)
                    {
                        building.transform.SetParent(newEvent.Building.transform.parent);
                    }
                }

                var availableCraftBottle = CheckAvailableCraftBottle();
                SwitchProductionBuilding(ItemType.BOTTLE_WORKER, availableCraftBottle);
            }
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            if (newEvent.ItemType == ItemType.BOTTLE_WORKER)
            {
                var needCheck = false;

                if (newEvent.Unit is Character)
                {
                    var character = (Character) newEvent.Unit;
                    if (character.CharacterType == CharacterType.ASSISTANT || character.CharacterType == CharacterType.APPRENTICE)
                    {
                        needCheck = true;
                    }
                }
                else if (newEvent.Unit is Building)
                {
                    var building = (Building) newEvent.Unit;
                    if (building.ProduceItemType == ItemType.BOTTLE_WORKER)
                    {
                        needCheck = true;
                    }
                }

                if (needCheck)
                {
                    var availableCraftBottle = CheckAvailableCraftBottle();
                    SwitchProductionBuilding(ItemType.BOTTLE_WORKER, availableCraftBottle);
                }
            }
            else if (newEvent.ItemType == ItemType.BOTTLE_HERO)
            {
                if (newEvent.Unit is Character)
                {
                    var character = (Character) newEvent.Unit;
                    if (character.CharacterType == CharacterType.PLAYER)
                    {
                        if (character.Items.GetAmount(ItemType.BOTTLE_HERO) >= 2)
                        {
                            SwitchProductionBuilding(ItemType.BOTTLE_HERO, false);
                        }
                    }
                }
            }
            else if (newEvent.ItemType == ItemType.ASSISTANT || newEvent.ItemType == ItemType.APPRENTICE)
            {
                SwitchProductionBuilding(ItemType.BOTTLE_WORKER, true);
            }
        }

        public bool CheckAvailableCraftBottle()
        {
            var playerBottles = 0;
            var notBuffedWorkers = 0;
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.ASSISTANT || character.CharacterType == CharacterType.APPRENTICE)
                {
                    var worker = (Worker) character;
                    if (worker.TargetBuilding == null || !(worker.TargetBuilding.ProduceItemType == ItemType.ASSISTANT || worker.TargetBuilding.ProduceItemType == ItemType.APPRENTICE))
                    {
                        if (character.Items.GetAmount(ItemType.BOTTLE_WORKER) < 1)
                        {
                            notBuffedWorkers++;
                        }
                    }
                }
                else if (character.CharacterType == CharacterType.PLAYER)
                {
                    playerBottles = character.Items.GetAmount(ItemType.BOTTLE_WORKER);
                }
            }

            return playerBottles < notBuffedWorkers;
        }

        public void SwitchProductionBuilding(ItemType produceItemType, bool value)
        {
            foreach (var building in Buildings)
            {
                if (building.ProduceItemType == produceItemType)
                {
                    building.gameObject.SetActive(value);
                }
            }
        }
    }
}