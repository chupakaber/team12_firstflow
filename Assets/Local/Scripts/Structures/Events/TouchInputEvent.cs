using UnityEngine;

namespace Scripts
{
    public class TouchInputEvent: IEvent
    {
        public TouchInputEvent() {}
        public int Index;
        public int TouchID;
        public bool Start;
        public bool End;
        public Vector2 Position;
    }
}