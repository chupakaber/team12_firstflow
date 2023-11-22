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
        private List<ProgressBarView> _progressBarViews = new List<ProgressBarView>();

        private CharacterMovementSystem _playerMovementSystem = new CharacterMovementSystem();
        private PlayerInputSystem _playerInputSystem = new PlayerInputSystem();
        private CameraFollowSystem _cameraFollowSystem = new CameraFollowSystem();
        private AddItemSystem _addItemSystem = new AddItemSystem();
        private AddHonorSystem _addHonorSystem = new AddHonorSystem();
        private RemoveItemSystem _removeItemSystem = new RemoveItemSystem();
        private PickUpSystem _pickUpSystem = new PickUpSystem();
        private TriggerSystem _triggerSystem = new TriggerSystem();
        private BuildingTriggerSystem _buildingTriggerSystem = new BuildingTriggerSystem();
        private BuildingProductionSystem _buildingProductionSystem = new BuildingProductionSystem();
        private BuildingCollectingSystem _buildingCollectingSystem = new BuildingCollectingSystem();
        private BuildingConstructionSystem _buildingConstructionSystem = new BuildingConstructionSystem();
        private WorkerAssignSystem _workerAssignSystem = new WorkerAssignSystem();
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

            AddSystem(_addItemSystem);
            AddSystem(_addHonorSystem);
            AddSystem(_playerMovementSystem);
            AddSystem(_playerInputSystem);
            AddSystem(_cameraFollowSystem);
            AddSystem(_removeItemSystem);
            AddSystem(_triggerSystem);
            AddSystem(_buildingTriggerSystem);
            AddSystem(_buildingProductionSystem);
            AddSystem(_buildingCollectingSystem);
            AddSystem(_buildingConstructionSystem);
            AddSystem(_pickUpSystem);
            AddSystem(_workerAssignSystem);
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
            _container.AddLink(_progressBarViews, "ProgressBarViews");
            _container.Init();
            _eventBus.Init();

            _eventBus.CallEvent(new StartEvent());

            _eventBus.CallEvent(new AddItemEvent() { Count = 220, ItemType = Enums.ItemType.WOOD, Unit = _characters[0] });
            _eventBus.CallEvent(new AddItemEvent() { Count = 220, ItemType = Enums.ItemType.GOLD, Unit = _characters[0] });
            _eventBus.CallEvent(new AddItemEvent() { Count = 15, ItemType = Enums.ItemType.HONOR, Unit = _characters[0] });
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