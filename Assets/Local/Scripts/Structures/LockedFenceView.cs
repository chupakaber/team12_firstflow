using System.Collections;
using UnityEngine;

namespace Scripts
{
    public class LockedFenceView : GameObectWithStateView
    {
        public Animation LockAnimation;

        private Camera _camera = null;
        private Scenario _scenario = null;
        private Player _player = null;
        private float _endTime;

        public override void OnLoad()
        {
            if (States.Count > 0 && States[0] == 1)
            {
                gameObject.SetActive(false);
            }
        }

        public override void OnActivate(IQueuedEvent queuedEvent)
        {
            _camera = Camera.main;
            _scenario = FindObjectOfType<Scenario>();
            _player = FindObjectOfType<Player>();

            if (States.Count < 1)
            {
                States.Add(1);
            }
            else if (States[0] == 0)
            {
                States[0] = 1;
            }
            else
            {
                return;
            }
            
            _endTime = Time.time + 6f;
            StartCoroutine(LockCameraAsync(queuedEvent));
        }

        public IEnumerator LockCameraAsync(IQueuedEvent queuedEvent)
        {
            _player.IsCutSceneActiv = true;

            while (_endTime > Time.time)
            {
                yield return new WaitForEndOfFrame();

                var deltaTime = _endTime - Time.time;
                if (deltaTime >= 4f && deltaTime - Time.deltaTime < 4f)
                {
                    LockAnimation.Play();
                }

                var newPosition = Vector3.Lerp(_camera.transform.position, transform.position + _scenario.BaseCameraOffset, Time.deltaTime * 5f);
                _camera.transform.position = newPosition;
            }

            _player.IsCutSceneActiv = false;
            gameObject.SetActive(false);
            
            if (queuedEvent != null)
            {
                queuedEvent.Locked = false;
            }
        }
    }
}