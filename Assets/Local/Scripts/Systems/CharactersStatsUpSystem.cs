using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class CharactersStatsUpSystem: ISystem
    {
        public EventBus EventBus;
        public List<Character> Characters;

        private const float COLLISION_CHECK_PERIOD = 0.3f;
        private const float BREAK_COLLISION_DISTANCE_SQR = 4f;

        private float _lastCollisionCheckTime;

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            if (Time.time < _lastCollisionCheckTime + COLLISION_CHECK_PERIOD)
            {
                return;
            }

            _lastCollisionCheckTime = Time.time;

            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.PLAYER)
                {
                    var characterPosition = character.transform.position;
                    var node = character.CharacterCollisions.First;
                    while (node != null)
                    {
                        var nodeNext = node.Next;

                        var collisionCharacter = node.Value;
                        if ((characterPosition - collisionCharacter.transform.position).sqrMagnitude > BREAK_COLLISION_DISTANCE_SQR)
                        {
                            character.CharacterCollisions.Remove(node);
                        }
                        else
                        {
                            if (collisionCharacter.CharacterType == CharacterType.ASSISTANT || collisionCharacter.CharacterType == CharacterType.APPRENTICE)
                            {
                                var worker = (Worker)collisionCharacter;
                                if (worker.TargetBuilding != null && worker.TargetBuilding.ProduceItemType != ItemType.ASSISTANT && worker.TargetBuilding.ProduceItemType != ItemType.APPRENTICE)
                                {
                                    GiveBoostBottle(character, collisionCharacter);
                                }
                            }
                        }
                        
                        node = nodeNext;
                    }
                }
            }
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            if (newEvent.ItemType == ItemType.BOTTLE_HERO)
            {
                if (newEvent.Unit is Character)
                {
                    var character = (Character) newEvent.Unit;
                    if (character.CharacterType == CharacterType.PLAYER)
                    {
                        character.LevelUp();
                    }
                }
            }
            else if(newEvent.ItemType == ItemType.BOTTLE_WORKER)
            {
                if (newEvent.Unit is Assistant)
                {
                    var asssistant = (Assistant) newEvent.Unit;
                    asssistant.LevelUp();
                }
                else if (newEvent.Unit is Apprentice)
                {
                    var apprentice = (Apprentice) newEvent.Unit;
                    apprentice.LevelUp();
                }
            }
        }

        public void GiveBoostBottle(Character character, Character npc)
        {
            var boostBottleType = ItemType.BOTTLE_WORKER;
            if (character.Items.GetAmount(boostBottleType) > 0)
            {
                if (npc.Items.GetAmount(boostBottleType) < 1)
                {
                    EventBus.CallEvent(new AddItemEvent() { Unit = npc, ItemType = boostBottleType, Count = 1, FromPosition = character.transform.position });
                    EventBus.CallEvent(new RemoveItemEvent() { Unit = character, ItemType = boostBottleType, Count = 1 });
                }
            }
        }
    }
}
