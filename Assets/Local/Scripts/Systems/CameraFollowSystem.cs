using UnityEngine;

namespace Scripts
{
    public class CameraFollowSystem: ISystem
    {
        public Camera Camera;
        public Character Character;
        private Vector3 _offSet;

        public void EventCatch(UpdateEvent newEvent)
        {
            var newPosition = Character.transform.position + _offSet;
            newPosition = Vector3.Lerp(Camera.transform.position, newPosition, Time.deltaTime * 10f);
            Camera.transform.position = newPosition;
        }

        public void EventCatch(StartEvent newEvent)
        {
            _offSet = Camera.transform.position;
        }
    }
}
