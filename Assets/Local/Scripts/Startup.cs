using Scripts.Enums;
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
        private PoolCollection<ItemView> _itemViewPools = new PoolCollection<ItemView>();
        private PoolCollection<IconView> _iconViewPools = new PoolCollection<IconView>();
        private PoolCollection<BagOfTriesView> _bagOfTriesViewPools = new PoolCollection<BagOfTriesView>();
        private UnlockQueue _unlockQueue = new UnlockQueue();

        private PlayerInputSystem _playerInputSystem = new PlayerInputSystem();
        private CameraFollowSystem _cameraFollowSystem = new CameraFollowSystem();
        private AddItemSystem _addItemSystem = new AddItemSystem();
        private AddHonorSystem _addHonorSystem = new AddHonorSystem();
        private RemoveItemSystem _removeItemSystem = new RemoveItemSystem();
        private BuildingPickUpSystem _buildingPickUpSystem = new BuildingPickUpSystem();
        private TriggerSystem _triggerSystem = new TriggerSystem();
        private BuildingTriggerSystem _buildingTriggerSystem = new BuildingTriggerSystem();
        private BuildingInitSystem _buildingInitSystem = new BuildingInitSystem();
        private BuildingProductionSystem _buildingProductionSystem = new BuildingProductionSystem();
        private BuildingCollectingSystem _buildingCollectingSystem = new BuildingCollectingSystem();
        private BuildingConstructionSystem _buildingConstructionSystem = new BuildingConstructionSystem();
        private BuildingProgressBarSystem _buildingProgressBarSystem = new BuildingProgressBarSystem();
        private BuildingUpgradeSystem _buildingUpgradeSystem = new BuildingUpgradeSystem();
        private BuildingUnlockSystem _buildingUnlockSystem = new BuildingUnlockSystem();
        private WorkerAssignSystem _workerAssignSystem = new WorkerAssignSystem();
        private WorkerSpawnSystem _workerSpawnSystem = new WorkerSpawnSystem();
        private WorkerPickUpSystem _workerPickUpSystem = new WorkerPickUpSystem();
        private WorkerBehaviorSystem _workerBehaviorSystem = new WorkerBehaviorSystem();
        private AssistantBehaviorSystem _assistantBehaviorSystem = new AssistantBehaviorSystem();
        private ApprenticeBehaviorSystem _apprenticeBehaviorSystem = new ApprenticeBehaviorSystem();
        private CharacterMovementSystem _characterMovementSystem = new CharacterMovementSystem();
        private CharacterAnimationSystem _characterAnimationSystem = new CharacterAnimationSystem();
        private UISystem _uiSystem = new UISystem();
        private EnvironmentShaderSystem _environmentShaderSystem = new EnvironmentShaderSystem();
        private CharactersStatsUpSystem _charactersStatsUpSystem = new CharactersStatsUpSystem();


        public void Start()
        {
            _mainCamera = Camera.main;
            _characters.Add(FindObjectOfType<Character>());
            _uiView = FindObjectOfType<UIView>();
            _unlockQueue = FindObjectOfType<UnlockQueue>();

            var names = System.Enum.GetNames(typeof(ItemType));
            var values = (ItemType[])System.Enum.GetValues(typeof(ItemType));
            for (int i = 0; i < values.Length; i++)
            {
                _itemViewPools.Pools.Add((int)values[i], new ObjectPool<ItemView>($"Prefabs/{names[i]}"));
            }

            _iconViewPools.Pools.Add(0, new ObjectPool<IconView>("Prefabs/UI/Icons/GOLD"));
            _bagOfTriesViewPools.Pools.Add(0, new ObjectPool<BagOfTriesView>("Prefabs/UI/BagOfTries"));

            AddSystem(_addItemSystem);
            AddSystem(_addHonorSystem);
            AddSystem(_playerInputSystem);
            AddSystem(_cameraFollowSystem);
            AddSystem(_removeItemSystem);
            AddSystem(_triggerSystem);
            AddSystem(_buildingInitSystem);
            AddSystem(_buildingTriggerSystem);
            AddSystem(_buildingProductionSystem);
            AddSystem(_buildingCollectingSystem);
            AddSystem(_buildingConstructionSystem);
            AddSystem(_buildingPickUpSystem);
            AddSystem(_buildingProgressBarSystem);
            AddSystem(_buildingUpgradeSystem);
            AddSystem(_buildingUnlockSystem);
            AddSystem(_workerAssignSystem);
            AddSystem(_workerSpawnSystem);
            AddSystem(_workerPickUpSystem);
            AddSystem(_workerBehaviorSystem);
            AddSystem(_assistantBehaviorSystem);
            AddSystem(_apprenticeBehaviorSystem);
            AddSystem(_characterMovementSystem);
            AddSystem(_characterAnimationSystem);
            AddSystem(_uiSystem);
            AddSystem(_environmentShaderSystem);
            AddSystem(_charactersStatsUpSystem);

            _container.AddLink(_eventBus, "EventBus");
            _container.AddLink(_characters, "Characters");
            _container.AddLink(_mainCamera, "Camera");
            _container.AddLink(_uiView, "UIView");
            _container.AddLink(_buildings, "Buildings");
            _container.AddLink(_progressBarViews, "ProgressBarViews");
            _container.AddLink(_itemViewPools, "ItemViewPools");
            _container.AddLink(_iconViewPools, "IconViewPools");
            _container.AddLink(_unlockQueue, "UnlockQueue");
            _container.AddLink(_bagOfTriesViewPools, "BagOfTriesViewPools");
            _container.Init();
            _eventBus.Init();

            _characters[0].Items.AddItem(ItemType.BOTTLE_HERO, 0);
            _characters[0].Items.AddItem(ItemType.BOTTLE_WORKER, 0);
            _characters[0].Items.AddItem(ItemType.HONOR, 0);
            _characters[0].ResizeBagOfTries(_characters[0].BaseBagOfTriesCapacity);

            _eventBus.CallEvent(new StartEvent());

            _eventBus.CallEvent(new AddItemEvent() { ItemType = ItemType.GOLD, Count = 300, Unit = _characters[0]});
            _eventBus.CallEvent(new AddItemEvent() { ItemType = ItemType.WOOD, Count = 100, Unit = _characters[0]});
        }

        public void Update()
        {
            _eventBus.CallEvent(new UpdateEvent());
        }

        public void FixedUpdate()
        {
            _eventBus.CallEvent(new FixedUpdateEvent());
        }

        public void OnDisable()
        {
            _eventBus.CallEvent(new DisposeEvent());
        }

        private void AddSystem(ISystem system)
        {
            _eventBus.Systems.Add(system);
            _container.AddSystem(system);
        }
    }
}