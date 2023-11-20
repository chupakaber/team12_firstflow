using Scripts.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Startup : MonoBehaviour
    {
        private Camera _mainCamera;
        private UIView _uiView;
        private EventBus _eventBus = new EventBus();
        private DIContainer _container = new DIContainer();

        private List<Character> _characters = new List<Character>();
        private List<Building> _buildings = new List<Building>();

        private CharacterMovementSystem _playerMovementSystem = new CharacterMovementSystem();
        private PlayerInputSystem _playerInputSystem = new PlayerInputSystem();
        private BuildingProductionSystem _buildingProductionSystem = new BuildingProductionSystem();
        private CameraFollowSystem _cameraFollowSystem = new CameraFollowSystem();
        private AddItemSystem _craftSystem = new AddItemSystem();
        private BuildingCollectingSystem _buildingCollectingSystem = new BuildingCollectingSystem();
        private RemoveItemSystem _removeItemSystem = new RemoveItemSystem();
        private PickUpSystem _pickUpSystem = new PickUpSystem();
        private TriggerSystem _triggerSystem = new TriggerSystem();
        private AssignWorkerSystem _assignWorkerSystem = new AssignWorkerSystem();
        private WorkerSpawnSystem _workerSpawnSystem = new WorkerSpawnSystem();
        private WorkerPickUpSystem _workerPickUpSystem = new WorkerPickUpSystem();
        private WorkerBehaviorSystem _workerBehaviorSystem = new WorkerBehaviorSystem();
        private AssistantBehaviorSystem _assistantBehaviorSystem = new AssistantBehaviorSystem();
        private ApprenticeBehaviorSystem _apprenticeBehaviorSystem = new ApprenticeBehaviorSystem();
        private UISystem _uiSystem = new UISystem();
        

        public void Start()
        {
            _mainCamera = Camera.main;
            _characters.Add(FindObjectOfType<Character>());
            _uiView = FindObjectOfType<UIView>();

            AddSystem(_craftSystem);
            AddSystem(_playerMovementSystem);
            AddSystem(_buildingProductionSystem);
            AddSystem(_playerInputSystem);
            AddSystem(_cameraFollowSystem);
            AddSystem(_buildingCollectingSystem);
            AddSystem(_removeItemSystem);
            AddSystem(_pickUpSystem);
            AddSystem(_triggerSystem);
            AddSystem(_assignWorkerSystem);
            AddSystem(_workerSpawnSystem);
            AddSystem(_workerPickUpSystem);
            AddSystem(_workerBehaviorSystem);
            AddSystem(_assistantBehaviorSystem);
            AddSystem(_apprenticeBehaviorSystem);
            AddSystem(_uiSystem);

            _container.AddLink(_eventBus, "EventBus");
            _container.AddLink(_characters, "Characters");
            _container.AddLink(_mainCamera, "Camera");
            _container.AddLink(_uiView, "UIView");
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