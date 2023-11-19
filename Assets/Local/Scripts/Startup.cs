using Scripts.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Startup : MonoBehaviour
    {
        private Camera _mainCamera;
        private EventBus _eventBus = new EventBus();
        private DIContainer _container = new DIContainer();

        private List<Character> _characters = new List<Character>();
        private List<Building> _buildings = new List<Building>();

        private PlayerMovementSystem _playerMovementSystem = new PlayerMovementSystem();
        private PlayerInputSystem _playerInputSystem = new PlayerInputSystem();
        private BuildingProductionSystem _buildingProductionSystem = new BuildingProductionSystem();
        private CameraFollowSystem _cameraFollowSystem = new CameraFollowSystem();
        private CraftSystem _craftSystem = new CraftSystem();
        private BuildingCollectingSystem _buildingCollectingSystem = new BuildingCollectingSystem();
        private RemoveItemSystem _removeItemSystem = new RemoveItemSystem();
        private PickUpSystem _pickUpSystem = new PickUpSystem();
        private TriggerSystem _triggerSystem = new TriggerSystem();

        public void Start()
        {
            _mainCamera = Camera.main;
            _characters.Add(FindObjectOfType<Character>());

            AddSystem(_craftSystem);
            AddSystem(_playerMovementSystem);
            AddSystem(_buildingProductionSystem);
            AddSystem(_playerInputSystem);
            AddSystem(_cameraFollowSystem);
            AddSystem(_buildingCollectingSystem);
            AddSystem(_removeItemSystem);
            AddSystem(_pickUpSystem);
            AddSystem(_triggerSystem);

            _container.AddLink(_eventBus, "EventBus");
            _container.AddLink(_characters, "Characters");
            _container.AddLink(_mainCamera, "Camera");
            _container.AddLink(_buildings, "Buildings");
            _container.Init();
            _eventBus.Init();

            _eventBus.CallEvent(new StartEvent());
        }

        public void Update()
        {
            _eventBus.CallEvent(new UpdateEvent());
        }

        public void FixedUpdate()
        {
            _eventBus.CallEvent(new FixedUpdateEvent());
        }

        private void AddSystem(ISystem system)
        {
            _eventBus.Systems.Add(system);
            _container.AddSystem(system);
        }
    }
}