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
        private List<TimerBarView> _timerBarViews = new List<TimerBarView>();
        private PoolCollection<ItemView> _itemViewPools = new PoolCollection<ItemView>();
        private PoolCollection<IconView> _iconViewPools = new PoolCollection<IconView>();
        private PoolCollection<BagOfTriesView> _bagOfTriesViewPools = new PoolCollection<BagOfTriesView>();
        private PoolCollection<PinnedCounterView> _pinnedCounterViewPools = new PoolCollection<PinnedCounterView>();
        private PoolCollection<BubbleView> _bubbleViewPools = new PoolCollection<BubbleView>();
        private PoolCollection<MessageBubbleView> _messageBubbleViewPools = new PoolCollection<MessageBubbleView>();

        private PoolCollection<Assistant> _assistantPools = new PoolCollection<Assistant>();
        private PoolCollection<Apprentice> _apprenticePools = new PoolCollection<Apprentice>();
        private PoolCollection<Joker> _jokerPools = new PoolCollection<Joker>();
        private PoolCollection<Customer> _customerPools = new PoolCollection<Customer>();
        private PoolCollection<Mercenary> _mercenaryPools = new PoolCollection<Mercenary>();
        private PoolCollection<SmartCharacter> _alchemistPools = new PoolCollection<SmartCharacter>();
        private UnlockQueue _unlockQueue;
        private Scenario _scenario;
        private AudioListener _audioListener = new AudioListener();
        private SoundCollection _soundCollection;

        private PlayerInputSystem _playerInputSystem = new PlayerInputSystem();
        private CameraFollowSystem _cameraFollowSystem = new CameraFollowSystem();
        private ObjectActivationSystem _objectActivationSystem = new ObjectActivationSystem();
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
        private AlchemicSystem _alchemicSystem = new AlchemicSystem();
        private WorkerAssignSystem _workerAssignSystem = new WorkerAssignSystem();
        private WorkerSpawnSystem _workerSpawnSystem = new WorkerSpawnSystem();
        private WorkerPickUpSystem _workerPickUpSystem = new WorkerPickUpSystem();
        private WorkerBehaviorSystem _workerBehaviorSystem = new WorkerBehaviorSystem();
        private AssistantBehaviorSystem _assistantBehaviorSystem = new AssistantBehaviorSystem();
        private ApprenticeBehaviorSystem _apprenticeBehaviorSystem = new ApprenticeBehaviorSystem();
        private CustomerSpawnSystem _customerSpawnSystem = new CustomerSpawnSystem();
        private CustomerBehaviorSystem _customerBehaviorSystem = new CustomerBehaviorSystem();
        private SellItemSystem _sellItemSystem = new SellItemSystem();
        private JokerBehaviorSystem _jokerBehaviorSystem = new JokerBehaviorSystem();
        private SmartCharacterBehaviorSystem _smartCharacterBehaviorSystem = new SmartCharacterBehaviorSystem();
        private CharacterMovementSystem _characterMovementSystem = new CharacterMovementSystem();
        private CharacterAnimationSystem _characterAnimationSystem = new CharacterAnimationSystem();
        private UISystem _uiSystem = new UISystem();
        private EnvironmentShaderSystem _environmentShaderSystem = new EnvironmentShaderSystem();
        private CharactersStatsUpSystem _charactersStatsUpSystem = new CharactersStatsUpSystem();
        private CharacterInitSystem _characterInitSystem = new CharacterInitSystem();
        private ScenarioSystem _scenarioSystem = new ScenarioSystem();
        private MercenaryCampSystem _mercenaryCampSystem = new MercenaryCampSystem();
        private CharacterSpawnSystem _characterSpawnSystem = new CharacterSpawnSystem();
        private SaveLoadSystem _saveLoadSystem = new SaveLoadSystem();
        private SoundSystem _soundSystem = new SoundSystem();
        private CarnivalBehaviorSystem _carnivalBehaviorSystem = new CarnivalBehaviorSystem();
        private CutSceneSystem _cutSceneSystem = new CutSceneSystem();
        private FillingInteractionAreaSystem _fillingInteractionZoneSystem = new FillingInteractionAreaSystem();

        private bool _initialized;

        public void Start()
        {
            _mainCamera = Camera.main;
            _characters.Add(GameObject.Find("Character").GetComponent<SmartCharacter>());
            _uiView = FindObjectOfType<UIView>();
            _unlockQueue = FindObjectOfType<UnlockQueue>();
            _scenario = FindObjectOfType<Scenario>();
            _audioListener = new GameObject("AudioListener").AddComponent<AudioListener>();
            _soundCollection = ScriptableObject.Instantiate(Resources.Load<SoundCollection>("Settings/Sounds"));

            var names = System.Enum.GetNames(typeof(ItemType));
            var values = (ItemType[])System.Enum.GetValues(typeof(ItemType));
            for (int i = 0; i < values.Length; i++)
            {
                _itemViewPools.Pools.Add((int)values[i], new ObjectPool<ItemView>($"Prefabs/{names[i]}"));
            }

            _iconViewPools.Pools.Add(0, new ObjectPool<IconView>("Prefabs/UI/Icons/GOLD"));
            _bagOfTriesViewPools.Pools.Add(0, new ObjectPool<BagOfTriesView>("Prefabs/UI/BagOfTries"));
            _assistantPools.Pools.Add(0, new ObjectPool<Assistant>("Prefabs/Characters/Assistant"));
            _apprenticePools.Pools.Add(0, new ObjectPool<Apprentice>("Prefabs/Characters/Apprentice"));
            _customerPools.Pools.Add(0, new ObjectPool<Customer>("Prefabs/Characters/CustomerSoldier"));
            _customerPools.Pools.Add(1, new ObjectPool<Customer>("Prefabs/Characters/CustomerOfficial"));
            _customerPools.Pools.Add(2, new ObjectPool<Customer>("Prefabs/Characters/CustomerWoman"));
            _jokerPools.Pools.Add(0, new ObjectPool<Joker>("Prefabs/Characters/Joker"));
            _pinnedCounterViewPools.Pools.Add(0, new ObjectPool<PinnedCounterView>("Prefabs/UI/ShopCoinCounter"));
            _bubbleViewPools.Pools.Add(0, new ObjectPool<BubbleView>("Prefabs/UI/BubbleEmoji"));
            _messageBubbleViewPools.Pools.Add(0, new ObjectPool<MessageBubbleView>("Prefabs/UI/MessageBubble"));
            _messageBubbleViewPools.Pools.Add(1, new ObjectPool<MessageBubbleView>("Prefabs/UI/CartoonBubble"));
            _mercenaryPools.Pools.Add(0, new ObjectPool<Mercenary>("Prefabs/Characters/Mercenary"));
            _alchemistPools.Pools.Add(0, new ObjectPool<SmartCharacter>("Prefabs/Characters/Alchemist"));
            
            AddSystem(_buildingInitSystem);
            AddSystem(_cameraFollowSystem);
            AddSystem(_objectActivationSystem);
            AddSystem(_characterSpawnSystem);
            AddSystem(_saveLoadSystem);
            AddSystem(_addItemSystem);
            AddSystem(_removeItemSystem);
            AddSystem(_addHonorSystem);
            AddSystem(_scenarioSystem);
            AddSystem(_playerInputSystem);
            AddSystem(_triggerSystem);
            AddSystem(_buildingTriggerSystem);
            AddSystem(_buildingProductionSystem);
            AddSystem(_buildingUpgradeSystem);
            AddSystem(_buildingCollectingSystem);
            AddSystem(_buildingConstructionSystem);
            AddSystem(_buildingPickUpSystem);
            AddSystem(_buildingProgressBarSystem);
            AddSystem(_alchemicSystem);
            AddSystem(_workerAssignSystem);
            AddSystem(_workerSpawnSystem);
            AddSystem(_workerPickUpSystem);
            AddSystem(_workerBehaviorSystem);
            AddSystem(_assistantBehaviorSystem);
            AddSystem(_apprenticeBehaviorSystem);
            AddSystem(_customerSpawnSystem);
            AddSystem(_customerBehaviorSystem);
            AddSystem(_sellItemSystem);
            AddSystem(_jokerBehaviorSystem);
            AddSystem(_smartCharacterBehaviorSystem);
            AddSystem(_characterMovementSystem);
            AddSystem(_characterAnimationSystem);
            AddSystem(_uiSystem);
            AddSystem(_environmentShaderSystem);
            AddSystem(_charactersStatsUpSystem);
            AddSystem(_mercenaryCampSystem);
            AddSystem(_soundSystem);
            AddSystem(_carnivalBehaviorSystem);
            AddSystem(_cutSceneSystem);
            AddSystem(_characterInitSystem);
            AddSystem(_fillingInteractionZoneSystem);

            _container.AddLink(_eventBus, "EventBus");
            _container.AddLink(_characters, "Characters");
            _container.AddLink(_mainCamera, "Camera");
            _container.AddLink(_uiView, "UIView");
            _container.AddLink(_buildings, "Buildings");
            _container.AddLink(_progressBarViews, "ProgressBarViews");
            _container.AddLink(_timerBarViews, "TimerBarViews");
            _container.AddLink(_itemViewPools, "ItemViewPools");
            _container.AddLink(_iconViewPools, "IconViewPools");
            _container.AddLink(_unlockQueue, "UnlockQueue");
            _container.AddLink(_bagOfTriesViewPools, "BagOfTriesViewPools");
            _container.AddLink(_pinnedCounterViewPools, "PinnedCounterViewPools");
            _container.AddLink(_bubbleViewPools, "BubbleViewPools");
            _container.AddLink(_messageBubbleViewPools, "MessageBubbleViewPools");
            _container.AddLink(_assistantPools, "AssistantPools");
            _container.AddLink(_apprenticePools, "ApprenticePools");
            _container.AddLink(_customerPools, "CustomerPools");
            _container.AddLink(_jokerPools, "JokerPools");
            _container.AddLink(_alchemistPools, "AlchemistPools");
            _container.AddLink(_scenario, "Scenario");
            _container.AddLink(_mercenaryPools, "MercenaryPools");
            _container.AddLink(_audioListener, "AudioListener");
            _container.AddLink(_soundCollection, "SoundCollection");
            
            _container.Init();
            _eventBus.Init();

            _characters[0].Items.AddItem(ItemType.BOTTLE_HERO, 0);
            _characters[0].Items.AddItem(ItemType.BOTTLE_WORKER, 0);
            _characters[0].Items.AddItem(ItemType.HONOR, 0);
            _characters[0].ResizeBagOfTries(_characters[0].BaseBagOfTriesCapacity);

            _eventBus.CallEvent(new InitEvent());
            _eventBus.CallEvent(new StartEvent());

            _initialized = true;
        }

        public void Update()
        {
            if (!_initialized)
            {
                return;
            }

            _eventBus.CallEvent(new UpdateEvent());
        }

        public void FixedUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            
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