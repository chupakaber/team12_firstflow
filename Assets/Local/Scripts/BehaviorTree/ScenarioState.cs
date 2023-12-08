using System;
using System.Collections.Generic;
using Scripts.Enums;
using Scripts.Serialization;
using Scripts.Serializators;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    [Serializable]
    public class ScenarioState : IBehaviorState, ISerializable
    {
        private const int STRUCTURE_VERSION = 1;

        // Global settings
        public float MusicVolume = 1f;
        public float SoundVolume = 1f;

        // Global context
        public EventBus EventBus { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Character> Characters { get; set; }

        public SmartCharacter Player;

        // Entity state
        public float[] Timers = new float[32];
        public int[] States = new int[32];

        public void Pack(ByteBuffer buffer)
        {
            SerializationUtils.Put(buffer, STRUCTURE_VERSION);
            
            SerializationUtils.Put(buffer, Timers.Length);
            for (var i = 0; i < Timers.Length; i++)
            {
                SerializationUtils.Put(buffer, Timers[i]);
            }

            SerializationUtils.Put(buffer, States.Length);
            for (var i = 0; i < States.Length; i++)
            {
                SerializationUtils.Put(buffer, States[i]);
            }
            
            SerializationUtils.Put(buffer, Buildings.Count);
            foreach (var building in Buildings)
            {
                SerializationUtils.Put(buffer, building.ID);
                SerializationUtils.Put(buffer, building.gameObject.activeSelf ? 1 : 0);
                SerializationUtils.Put(buffer, building.Level);
                SerializationUtils.Put(buffer, building.UpgradeArea != null && building.UpgradeArea.gameObject.activeSelf ? 1 : 0);
                
                var itemsCount = 0;
                foreach (var item in building.Items)
                {
                    itemsCount++;
                }
                SerializationUtils.Put(buffer, itemsCount);

                foreach (var item in building.Items)
                {
                    SerializationUtils.Put(buffer, (int) item.Type);
                    SerializationUtils.Put(buffer, item.Amount);
                }
            }

            SerializationUtils.Put(buffer, Characters.Count);
            foreach (var character in Characters)
            {
                var smartCharacter = (SmartCharacter) character;
                SerializationUtils.Put(buffer, (int) character.CharacterType);
                SerializationUtils.Put(buffer, Vector3Serializer<Vector3>.Instance, character.transform.position);
                SerializationUtils.Put(buffer, smartCharacter.TargetBuilding != null ? smartCharacter.TargetBuilding.ID : 0);
                if (character.CharacterType == CharacterType.ASSISTANT)
                {
                    var assistant = (Assistant) character;
                    SerializationUtils.Put(buffer, assistant.ResourceBuilding != null ? assistant.ResourceBuilding.ID : 0);
                }

                var itemsCount = 0;
                foreach (var item in character.Items)
                {
                    itemsCount++;
                }
                SerializationUtils.Put(buffer, itemsCount);

                foreach (var item in character.Items)
                {
                    SerializationUtils.Put(buffer, (int) item.Type);
                    SerializationUtils.Put(buffer, item.Amount);
                }
            }
        }

        public void Unpack(ByteBuffer buffer)
        {
            var version = SerializationUtils.Get(buffer, 1);

            var timersCount = SerializationUtils.Get(buffer, 0);
            for (var i = 0; i < timersCount; i++)
            {
                Timers[i] = SerializationUtils.Get(buffer, 0f);
            }

            var statesCount = SerializationUtils.Get(buffer, 0);
            for (var i = 0; i < statesCount; i++)
            {
                States[i] = SerializationUtils.Get(buffer, 0);
            }

            var buildingsCount = SerializationUtils.Get(buffer, 0);
            for (var i = 0; i < buildingsCount; i++)
            {
                var buildingID = SerializationUtils.Get(buffer, 0);
                
                Building currentBuilding = null;
                var buildingActive = SerializationUtils.Get(buffer, 0) == 1;
                var buildingLevel = SerializationUtils.Get(buffer, 0);
                var buildingAreaEnabled = SerializationUtils.Get(buffer, 1) == 1;
                
                foreach (var building in Buildings)
                {
                    if (building.ID == buildingID)
                    {
                        currentBuilding = building;
                    }
                }
                
                if (currentBuilding != null)
                {
                    currentBuilding.gameObject.SetActive(buildingActive);
                    currentBuilding.Level = buildingLevel;
                    if (currentBuilding.UpgradeArea != null)
                    {
                        currentBuilding.UpgradeArea.gameObject.SetActive(buildingAreaEnabled);
                    }
                }

                var itemsCount = SerializationUtils.Get(buffer, 0);
                for (var j = 0; j < itemsCount; j++)
                {
                    var itemType = (ItemType) SerializationUtils.Get(buffer, 0);
                    var itemAmount = SerializationUtils.Get(buffer, 0);

                    if (currentBuilding != null)
                    {
                        EventBus.CallEvent(new AddItemEvent() { ItemType = itemType, Count = itemAmount, Unit = currentBuilding });
                    }
                }
            }

            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.PLAYER || character.CharacterType == CharacterType.CARNIVAL)
                {
                    continue;
                }

                character.Release();
            }
            Characters.Clear();

            var carnival = GameObject.FindObjectsOfType<Carnival>();
            foreach (var character in carnival)
            {
                Characters.Add(character);
            }
            
            var charactersCount = SerializationUtils.Get(buffer, 0);
            for (var i = 0; i < charactersCount; i++)
            {
                var characterType = (CharacterType) SerializationUtils.Get(buffer, 0);
                var characterPosition = SerializationUtils.Get(buffer, Vector3Serializer<Vector3>.Instance);
                var characterTargetBuildingID = SerializationUtils.Get(buffer, 0);
                
                SmartCharacter character = null;
                switch (characterType)
                {
                    case CharacterType.PLAYER:
                        character = UnityEngine.Object.FindObjectOfType<Player>();
                        foreach (var item in character.Items)
                        {
                            character.ItemStackView.RemoveItem(item.Type, item.Amount);
                        }
                        character.Items.Clear();
                        character.transform.position = characterPosition;
                    break;
                    case CharacterType.ASSISTANT:
                    case CharacterType.APPRENTICE:
                        var spawnEvent = new SpawnCharacterEvent() { CharacterType = characterType, Position = characterPosition, Character = null };
                        EventBus.CallEvent(spawnEvent);
                        character = spawnEvent.Character;
                    break;
                }

                if (character != null)
                {
                    Characters.Add(character);
                }

                if (characterType == CharacterType.ASSISTANT)
                {
                    var characterResourceBuildingID = SerializationUtils.Get(buffer, 0);
                    if (character != null)
                    {
                        var assistant = (Assistant) character;
                        foreach (var building in Buildings)
                        {
                            if (building.ID == characterResourceBuildingID)
                            {
                                assistant.ResourceBuilding = building;
                                assistant.ResourceBuilding.AssignedProductionCharacters.Add(assistant);
                                break;
                            }
                        }
                    }
                }

                if (character != null)
                {
                    foreach (var building in Buildings)
                    {
                        if (building.ID == characterTargetBuildingID)
                        {
                            character.TargetBuilding = building;
                            if (character.CharacterType == CharacterType.ASSISTANT)
                            {
                                character.TargetBuilding.AssignedUnloadingCharacters.Add(character);
                            }
                            else if (character.CharacterType == CharacterType.APPRENTICE)
                            {
                                character.TargetBuilding.AssignedProductionCharacters.Add(character);
                            }
                            break;
                        }
                    }
                }

                var itemsCount = SerializationUtils.Get(buffer, 0);
                for (var j = 0; j < itemsCount; j++)
                {
                    var itemType = (ItemType) SerializationUtils.Get(buffer, 0);
                    var itemAmount = SerializationUtils.Get(buffer, 0);

                    if (character != null)
                    {
                        EventBus.CallEvent(new AddItemEvent() { ItemType = itemType, Count = itemAmount, Unit = character });
                    }
                }
            }
        }

        public int GetStructureLength()
        {
            var bytesCount = 0;

            bytesCount += SerializationUtils.SIZE_OF_INT;
            
            bytesCount += SerializationUtils.SIZE_OF_INT;
            bytesCount += Timers.Length * SerializationUtils.SIZE_OF_INT;

            bytesCount += SerializationUtils.SIZE_OF_INT;
            bytesCount += States.Length * SerializationUtils.SIZE_OF_FLOAT;

            bytesCount += SerializationUtils.SIZE_OF_INT;
            
            foreach (var building in Buildings)
            {
                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += SerializationUtils.SIZE_OF_INT;

                var itemsCount = 0;
                foreach (var item in building.Items)
                {
                    itemsCount++;
                }

                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += itemsCount * 2 * SerializationUtils.SIZE_OF_INT;
            }

            bytesCount += SerializationUtils.SIZE_OF_INT;
            foreach (var character in Characters)
            {
                var smartCharacter = (SmartCharacter) character;
                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += Vector3Serializer<Vector3>.SIZE_OF_STRUCTURE;
                bytesCount += SerializationUtils.SIZE_OF_INT;
                if (character.CharacterType == CharacterType.ASSISTANT)
                {
                    bytesCount += SerializationUtils.SIZE_OF_INT;
                }

                var itemsCount = 0;
                foreach (var item in character.Items)
                {
                    itemsCount++;
                }
                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += itemsCount * 2 * SerializationUtils.SIZE_OF_INT;
            }

            return bytesCount;
        }
    }
}