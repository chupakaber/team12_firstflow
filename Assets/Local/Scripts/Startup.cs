using Scripts.Systems;
using UnityEngine;

namespace Scripts
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Character _character;
        public PlayerMovementSystem playerMovementSystem = new PlayerMovementSystem();
        public PlayerInputSystem playerInputSystem = new PlayerInputSystem();
        public BuildingProductionSystem buildingProductionSystem = new BuildingProductionSystem();
        public CameraFollowSystem cameraFollowSystem = new CameraFollowSystem();
        public CraftSystem craftSystem = new CraftSystem();
        public EventBus eventBus = new EventBus();
        public DIContainer container = new DIContainer();


        public void Start()
        {
            AddSystem(craftSystem);
            AddSystem(playerMovementSystem);
            AddSystem(buildingProductionSystem);
            AddSystem(playerInputSystem);
            AddSystem(cameraFollowSystem);

            container.AddLink(eventBus, "EventBus");
            container.AddLink(_character, "Character");
            container.AddLink(_mainCamera, "Camera");
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