
using UnityEngine;

namespace Scripts
{
    public class CameraFollowSystem
    {
        public Camera Camera;
        public Character Character;
        private Vector3 _offSet;

        public void CameraMovement()
        {
            var newPosition = Character.transform.position + _offSet;
            newPosition = Vector3.Lerp(Camera.transform.position, newPosition, Time.deltaTime * 10f);
            Camera.transform.position = newPosition;
        }

        public void Init()
        {
            _offSet = Camera.transform.position;
        }
    }
}
