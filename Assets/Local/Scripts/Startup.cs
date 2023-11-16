using Scripts.Systems;
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

        [SerializeField] private Camera _mainCamera;

        public void Start()
        {
            character = FindObjectOfType<Character>();

            playerMovementSystem = new PlayerMovementSystem();

            playerInputSystem = new PlayerInputSystem();
            playerInputSystem.Character = character;
            playerInputSystem.Init();

            craftSystem = new CraftSystem();

            buildingProductionSystem = new BuildingProductionSystem();
            buildingProductionSystem.CraftSystem = craftSystem;
            buildingProductionSystem.character = character;
            buildingProductionSystem.Init();

            cameraFollowSystem = new CameraFollowSystem();
            cameraFollowSystem.Camera = _mainCamera;
            cameraFollowSystem.Character = character;
            cameraFollowSystem.Init();


        }


        public void Update()
        {
            cameraFollowSystem.CameraMovement();
        }

        public void FixedUpdate()
        {
            playerMovementSystem.Movement(character);

            buildingProductionSystem.Production();

            foreach (var building in buildingProductionSystem.Buildings)
            {
                building.IsWork = character.CollidedWith != null && character.CollidedWith.Equals(building.ProductionArea);
            }
        }
    }
}