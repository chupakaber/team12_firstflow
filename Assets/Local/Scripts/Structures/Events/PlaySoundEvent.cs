using UnityEngine;

namespace Scripts
{
    public class PlaySoundEvent: IEvent
    {
        public bool IsMusic;
        public int SoundId;
        public Vector3 Position;
        public Transform AttachedTo;
    }
}