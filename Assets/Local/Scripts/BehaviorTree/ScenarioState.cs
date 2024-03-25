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
        private const int STRUCTURE_VERSION = 2;

        // Global settings
        public float MusicVolume = 1f;
        public float SoundVolume = 1f;

        // Global context
        public EventBus EventBus { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Character> Characters { get; set; }
        public List<IGameObjectWithState> GameObjectsWithState { get; set; }
        public UIView UIView { get; set; }

        public SmartCharacter Player;

        // Entity state
        public float[] Timers = new float[32];
        public int[] States = new int[32];

        public float GetTimer(int index)
        {
            return Timers[index];
        }

        public int GetState(int index)
        {
            return States[index];
        }

        public void SetTimer(int index, float value)
        {
            Timers[index] = value;
        }

        public void SetState(int index, int value)
        {
            States[index] = value;
        }

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
                    SerializationUtils.Put(buffer, (int) assistant.PickUpItemConstraint);
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

            SerializationUtils.Put(buffer, GameObjectsWithState.Count);
            foreach (var obj in GameObjectsWithState)
            {
                var gameObject = obj.GetGameObject();
                SerializationUtils.Put(buffer, obj.ObjectID);
                SerializationUtils.Put(buffer, gameObject.activeSelf ? 1 : 0);
                var states = obj.States;
                var statesCount = states.Count;
                SerializationUtils.Put(buffer, statesCount);
                for (var i = 0; i < statesCount; i++)
                {
                    SerializationUtils.Put(buffer, states[i]);
                }
            }

            SerializationUtils.Put(buffer, Vector3Serializer<Vector3>.Instance, UIView.PointerArrowTargetPosition);
            SerializationUtils.Put(buffer, Vector3Serializer<Vector3>.Instance, UIView.PointerArrowTargetPositionOnNavMesh);
            SerializationUtils.Put(buffer, UIView.GetCurrentTutorialStep());
            
            SerializationUtils.Put(buffer, SoundVolume);
            SerializationUtils.Put(buffer, MusicVolume);
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
                if (character.CharacterType == CharacterType.PLAYER || character.CharacterType == CharacterType.CARNIVAL || character.CharacterType == CharacterType.DAOS || character.CharacterType == CharacterType.DONKEY)
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
                    case CharacterType.MERCENARY:
                        var spawnEvent = new SpawnCharacterEvent() { CharacterType = characterType, Position = characterPosition, Character = null };
                        EventBus.CallEvent(spawnEvent);
                        character = spawnEvent.Character;
                    break;
                    case CharacterType.DAOS:
                        Characters.Add(GameObject.Find("DaosCharacter").GetComponent<SmartCharacter>());
                    break;
                    case CharacterType.DONKEY:
                        Characters.Add(GameObject.Find("Donkey").GetComponent<SmartCharacter>());
                    break;
                }

                if (characterType == CharacterType.MERCENARY && character != null)
                {
                    foreach (var building in Buildings)
                    {
                        if (building.ProduceItemType == ItemType.GOLD && building.ConsumeItemType == ItemType.NONE)
                        {
                            if (character is Mercenary)
                            {
                                var mercenary = (Mercenary) character;
                                building.ProductionItemAmountPerCycle += mercenary.ProductionAmount;
                                building.ProductionLimit = building.ProductionItemAmountPerCycle;
                            }
                        }
                    }
                }

                if (character != null)
                {
                    Characters.Add(character);
                }

                if (characterType == CharacterType.ASSISTANT)
                {
                    var characterResourceBuildingID = SerializationUtils.Get(buffer, 0);
                    var pickUpItemConstraint = (ItemType) SerializationUtils.Get(buffer, 0);
                    if (character != null)
                    {
                        var assistant = (Assistant) character;
                        assistant.PickUpItemConstraint = pickUpItemConstraint;
                        foreach (var building in Buildings)
                        {
                            if (building.ID == characterResourceBuildingID)
                            {
                                assistant.ResourceBuilding = building;
                                assistant.ResourceBuilding.AssignedPickingUpCharacters.Add(assistant);
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

                    if ((character.CharacterType == CharacterType.ASSISTANT || character.CharacterType == CharacterType.APPRENTICE) && character.TargetBuilding == null)
                    {
                        foreach (var otherCharacter in Characters)
                        {
                            if (otherCharacter.CharacterType == CharacterType.PLAYER)
                            {
                                otherCharacter.AddLastInQueue(character);
                            }
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

            if (version > 1)
            {
                var gameObjectsWithStateCount = SerializationUtils.Get(buffer, 0);
                for (var i = 0; i < gameObjectsWithStateCount; i++)
                {
                    var objectID = SerializationUtils.Get(buffer, 0);
                    var active = SerializationUtils.Get(buffer, 0) == 1;
                    var objStatesCount = SerializationUtils.Get(buffer, 0);
                    
                    IGameObjectWithState gameObjectWithState = null;

                    for (var j = 0; j < GameObjectsWithState.Count; j++)
                    {
                        var iteratedGameObjectsWithState = GameObjectsWithState[j];
                        if (iteratedGameObjectsWithState.ObjectID == objectID)
                        {
                            gameObjectWithState = iteratedGameObjectsWithState;
                        }
                    }

                    if (gameObjectWithState != null)
                    {
                        var gameObject = gameObjectWithState.GetGameObject();
                        gameObject.SetActive(active);

                        var objStates = gameObjectWithState.States;
                        while (objStates.Count < objStatesCount && objStates.Count < 1000)
                        {
                            objStates.Add(0f);
                        }
                        for (var j = 0; j < objStatesCount; j++)
                        {
                            objStates[j] = SerializationUtils.Get(buffer, 0f);
                        }

                        gameObjectWithState.OnLoad();
                    }
                    else
                    {
                        for (var j = 0; j < objStatesCount; j++)
                        {
                            SerializationUtils.Get(buffer, 0f);
                        }
                    }
                }
            }

            var storedTargetPosition = SerializationUtils.Get(buffer, Vector3Serializer<Vector3>.Instance);
            var storedTargetPositionOnNavMesh = SerializationUtils.Get(buffer, Vector3Serializer<Vector3>.Instance);
            if (storedTargetPosition.sqrMagnitude > float.Epsilon)
            {
                UIView.PointerArrowTargetPosition = storedTargetPosition;
                EventBus.CallEvent(new SetArrowPointerEvent() { TargetPosition = storedTargetPosition });
            }

            var tutorialStep = SerializationUtils.Get(buffer, 0);
            if (tutorialStep > -1)
            {
                UIView.ShowTutorial(tutorialStep);
            }
            else
            {
                UIView.HideTutorial();
            }

            SoundVolume = SerializationUtils.Get(buffer, 1f);
            MusicVolume = SerializationUtils.Get(buffer, 1f);
            UIView.SetVolume(SoundVolume, MusicVolume);
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
                    bytesCount += 2 * SerializationUtils.SIZE_OF_INT;
                }

                var itemsCount = 0;
                foreach (var item in character.Items)
                {
                    itemsCount++;
                }
                bytesCount += SerializationUtils.SIZE_OF_INT;
                bytesCount += itemsCount * 2 * SerializationUtils.SIZE_OF_INT;
            }

            bytesCount += SerializationUtils.SIZE_OF_INT;
            foreach (var obj in GameObjectsWithState)
            {
                bytesCount += SerializationUtils.SIZE_OF_INT * 3;
                bytesCount += SerializationUtils.SIZE_OF_FLOAT * obj.States.Count;
            }

            bytesCount += Vector3Serializer<Vector3>.SIZE_OF_STRUCTURE;
            bytesCount += Vector3Serializer<Vector3>.SIZE_OF_STRUCTURE;

            bytesCount += SerializationUtils.SIZE_OF_INT;

            bytesCount += SerializationUtils.SIZE_OF_FLOAT;
            bytesCount += SerializationUtils.SIZE_OF_FLOAT;

            return bytesCount;
        }
    }
}