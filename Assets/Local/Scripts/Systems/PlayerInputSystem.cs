using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class PlayerInputSystem: ISystem
    {
        public List<Character> Characters;
        private PlayerInput _playerInput;
        private Transform _cameraTransform;

        public void EventCatch(StartEvent newEvent)
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _playerInput.Default.Movement.performed += OnMovement;
            _playerInput.Default.Movement.canceled += OnMovement;
            _cameraTransform = Camera.main.transform;
        }

        private void OnMovement(InputAction.CallbackContext context)
        {
            foreach (Character character in Characters) 
            {
               if (character.IsPlayer)
               {
                    var direction = context.ReadValue<Vector2>();
                    character.WorldDirection = new Vector3(direction.x, 0f, direction.y);
                    character.WorldDirection = Quaternion.Euler(0, _cameraTransform.rotation.eulerAngles.y, 0) * character.WorldDirection;                
               } 
            }
        }
    }


}
