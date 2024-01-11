using UnityEngine;

namespace Scripts
{
    public class SetArrowPointerEvent: IEvent
    {
        public GameObject TargetGameObject;
        public Vector3 TargetPosition;
    }
}