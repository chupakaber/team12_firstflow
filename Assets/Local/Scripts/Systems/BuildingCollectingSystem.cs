using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingCollectingSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;
        public List<Building> Buildings;

        public void EventCatch(FixedUpdateEvent newEvent) 
        {
            foreach (var building in Buildings)
            {
                if (!building.gameObject.activeSelf)
                {
                    building.UnloadingCharacters.Clear();
                }
                
                for (var i = 0; i < building.UnloadingCharacters.Count; i++)
                {
                    var character = building.UnloadingCharacters[i];
                    Collecting(building, character);
                }
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {                
                if (newEvent.Trigger.Equals(building.UnloadingArea) && newEvent.Character.CharacterType == Enums.CharacterType.PLAYER)
                {
                    if (building.UnloadingAreaMeshRenderer != null)
                    {
                        EventBus.CallEvent(new PreparationForInteractionEvent() {MeshRenderer = building.UnloadingAreaMeshRenderer});
                    }
                }
            }
        }

        private void Collecting(Building building, Character character)
        {
            var consumeItemType = building.ConsumeItemType;
            var characterHorizontalVelocity = character.CharacterRigidbody.velocity;
            characterHorizontalVelocity.y = 0f;
            if (characterHorizontalVelocity.sqrMagnitude > 0.5f)
            {
                character.LastDropItemTime = Time.time;
            }

            var storageAmount = building.Items.GetAmount(consumeItemType);
            var requiredAmount = building.ResourceLimit - storageAmount;
            if (consumeItemType == Enums.ItemType.ALL_PHYSIC_NON_UNIQUE)
            {
                foreach (var item in character.Items)
                {
                    if (item.Amount > 0 && character.Items.CompareItemType(item.Type, consumeItemType))
                    {
                        consumeItemType = item.Type;
                        requiredAmount = item.Amount;
                        break;
                    }
                }
            }
            var dropCooldown = character.GetDropCooldown(consumeItemType, out var itemsMovingAmount, requiredAmount);

            if (Time.time >= character.LastDropItemTime + dropCooldown)
            {
                var availableAmount = character.Items.GetAmount(consumeItemType);
                itemsMovingAmount = Mathf.Min(itemsMovingAmount, Mathf.Min(requiredAmount, availableAmount));
                if (itemsMovingAmount > 0)
                {
                    var sourcePileTopPosition = character.GetStackTopPosition(consumeItemType);
                    var removeItemEvent = new RemoveItemEvent() { ItemType = consumeItemType, Count = itemsMovingAmount, Unit = character };
                    var addItemEvent = new AddItemEvent() { ItemType = consumeItemType, Count = itemsMovingAmount, Unit = building, FromPosition = sourcePileTopPosition };
                    EventBus.CallEvent(removeItemEvent);
                    EventBus.CallEvent(addItemEvent);
                    character.LastDropItemTime = Time.time;

                    if (consumeItemType == Enums.ItemType.GOLD)
                    {
                        storageAmount = building.Items.GetAmount(consumeItemType);
                        requiredAmount = building.ResourceLimit - storageAmount;
                        if (requiredAmount <= 0)
                        {
                            building.UnloadingCharacters.Remove(character);
                        }
                    }

                    if (building.ProduceMethod == Building.ProductionMethod.RESOURCE_TO_TIME)
                    {
                        var activityEndTime = Mathf.Max(Time.time, building.ProductionEndActivityTime);
                        building.ProductionEndActivityTime = activityEndTime + building.ProductionConversionRate * itemsMovingAmount;
                    }
                }
            }            
        }
    }
}
