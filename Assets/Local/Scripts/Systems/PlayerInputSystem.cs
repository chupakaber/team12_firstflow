using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class PlayerInputSystem
    {
        public Character Character;
        private PlayerInput _playerInput;
        private Transform _cameraTransform;

        public void Init()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Default.Movement.performed += OnMovement;
            _playerInput.Default.Movement.canceled += OnMovement;
            _cameraTransform = Camera.main.transform;
        }

        private void OnMovement(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();
            Character.WorldDirection = new Vector3(direction.x, 0f, direction.y);
            Character.WorldDirection = Quaternion.Euler(0, _cameraTransform.rotation.eulerAngles.y, 0) * Character.WorldDirection;
        }
    }


}
