using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

            InitDebug();
        }

        public void EventCatch(DisposeEvent newEvent)
        {
            _playerInput.Disable();
            _playerInput.Default.Movement.performed -= OnMovement;
            _playerInput.Default.Movement.canceled -= OnMovement;
            
            ClearDebug();

            _playerInput = null;
        }

        private void OnMovement(InputAction.CallbackContext context)
        {
            foreach (Character character in Characters)
            {
               if (character.CharacterType == CharacterType.PLAYER)
               {
                    var direction = context.ReadValue<Vector2>();
                    character.WorldDirection = new Vector3(direction.x, 0f, direction.y);

                    character.WorldDirection = Quaternion.Euler(0, _cameraTransform.rotation.eulerAngles.y, 0) * character.WorldDirection;
               } 
            }
        }

#region DEBUG

        private float _lastTimeScale = 1f;
        
        private void InitDebug()
        {
            _playerInput.Default.DebugPause.started += DebugPause;
            _playerInput.Default.DebugReload.started += DebugReload;
            _playerInput.Default.DebugSpeedX1.started += DebugSpeedX1;
            _playerInput.Default.DebugSpeedX2.started += DebugSpeedX2;
            _playerInput.Default.DebugSpeedX5.started += DebugSpeedX5;
            _playerInput.Default.DebugSpeedX10.started += DebugSpeedX10;
        }

        private void ClearDebug()
        {
            _playerInput.Default.DebugPause.started -= DebugPause;
            _playerInput.Default.DebugReload.started -= DebugReload;
            _playerInput.Default.DebugSpeedX1.started -= DebugSpeedX1;
            _playerInput.Default.DebugSpeedX2.started -= DebugSpeedX2;
            _playerInput.Default.DebugSpeedX5.started -= DebugSpeedX5;
            _playerInput.Default.DebugSpeedX10.started -= DebugSpeedX10;

            Time.timeScale = 1f;
        }

        private void DebugPause(InputAction.CallbackContext context) {
            if (Time.timeScale < 1f) {
                Time.timeScale = _lastTimeScale;
            } else {
                _lastTimeScale = Time.timeScale;
                Time.timeScale = 0.1f;
            }
        }
        private void DebugReload(InputAction.CallbackContext context) {
            SceneManager.LoadScene(0);
        }
        private void DebugSpeedX1(InputAction.CallbackContext context) {
            Time.timeScale = 1f;
        }
        private void DebugSpeedX2(InputAction.CallbackContext context) {
            Time.timeScale = 2f;
        }
        private void DebugSpeedX5(InputAction.CallbackContext context) {
            Time.timeScale = 5f;
        }
        private void DebugSpeedX10(InputAction.CallbackContext context) {
            Time.timeScale = 10f;
        }

#endregion
    }
}
