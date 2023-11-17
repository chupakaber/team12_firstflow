using Scripts.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Startup : MonoBehaviour
    {
        public Character character;
        public PlayerMovementSystem playerMovementSystem;
        public PlayerInputSystem playerInputSystem;
        public BuildingProductionSystem buildingProductionSystem;
        public CameraFollowSystem cameraFollowSystem;
        public CraftSystem craftSystem;
        public EventBus eventBus = new EventBus();

        [SerializeField] private Camera _mainCamera;

        public void Start()
        {            
            character = FindObjectOfType<Character>();

            playerMovementSystem = new PlayerMovementSystem();
            playerMovementSystem.Character = character;

            playerInputSystem = new PlayerInputSystem();
            playerInputSystem.Character = character;
            playerInputSystem.Init();

            craftSystem = new CraftSystem();

            buildingProductionSystem = new BuildingProductionSystem();
            buildingProductionSystem.EventBus = eventBus;
            buildingProductionSystem.character = character;
            buildingProductionSystem.Init();

            cameraFollowSystem = new CameraFollowSystem();
            cameraFollowSystem.Camera = _mainCamera;
            cameraFollowSystem.Character = character;
            cameraFollowSystem.Init();

            eventBus.Systems.Add(craftSystem);
            eventBus.Systems.Add(playerMovementSystem);
        }


        public void Update()
        {
            cameraFollowSystem.CameraMovement();
        }

        public void FixedUpdate()
        {
            eventBus.CallEvent(new FixedUpdateEvent());

            buildingProductionSystem.Production();

            foreach (var building in buildingProductionSystem.Buildings)
            {
                building.IsWork = character.CollidedWith != null && character.CollidedWith.Equals(building.ProductionArea);
            }
        }
    }
}