using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    [CreateAssetMenu(fileName = "Sounds", menuName = "Team 12/Sounds", order = 10000)]
    public class SoundCollection : ScriptableObject
    {
        [System.Serializable]
        public class ClipDecription
        {
            public int Id;
            public AudioClip Clip;
            public bool IsMusic;
        }

        [Header("Audio Clips")]
        public List<ClipDecription> AudioClips = new List<ClipDecription>();

        public ClipDecription GetClip(int id, bool isMusic)
        {
            foreach (var clip in AudioClips)
            {
                if (clip.Id == id && clip.IsMusic == isMusic)
                {
                    return clip;
                }
            }
            
            return null;
        }
    }
}
