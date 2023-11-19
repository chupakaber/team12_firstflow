using Scripts.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private List<Character> _characters;
        private PlayerMovementSystem playerMovementSystem = new PlayerMovementSystem();
        private PlayerInputSystem playerInputSystem = new PlayerInputSystem();
        private BuildingProductionSystem buildingProductionSystem = new BuildingProductionSystem();
        private CameraFollowSystem cameraFollowSystem = new CameraFollowSystem();
        private CraftSystem craftSystem = new CraftSystem();
        private BuildingCollectingSystem buildingCollectingSystem = new BuildingCollectingSystem();
        private RemoveItemSystem removeItemSystem = new RemoveItemSystem();
        private PickUpSystem pickUpSystem = new PickUpSystem();
        private TriggerSystem triggerSystem = new TriggerSystem();
        private EventBus eventBus = new EventBus();
        private DIContainer container = new DIContainer();
        private List<Building> Buildings = new List<Building>();


        public void Start()
        {
            _characters.Add(Object.FindObjectOfType<Character>());
            AddSystem(craftSystem);
            AddSystem(playerMovementSystem);
            AddSystem(buildingProductionSystem);
            AddSystem(playerInputSystem);
            AddSystem(cameraFollowSystem);
            AddSystem(buildingCollectingSystem);
            AddSystem(removeItemSystem);
            AddSystem(pickUpSystem);
            AddSystem(triggerSystem);

            container.AddLink(eventBus, "EventBus");
            container.AddLink(_characters, "Characters");
            container.AddLink(_mainCamera, "Camera");
            container.AddLink(Buildings, "Buildings");
            container.Init();
            eventBus.Init();

            eventBus.CallEvent(new StartEvent());
        }

        public void Update()
        {
            eventBus.CallEvent(new UpdateEvent());
        }

        public void FixedUpdate()
        {
            eventBus.CallEvent(new FixedUpdateEvent());
        }

        private void AddSystem(ISystem system)
        {
            eventBus.Systems.Add(system);
            container.AddSystem(system);
        }
    }
}