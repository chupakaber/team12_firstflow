using UnityEngine;

namespace Scripts
{
    public class CinematicBorders : MonoBehaviour
    {
        public Canvas Canvas;
        public Animation Animation;
        public AnimationClip CloseAnimationClip;
        public AnimationClip OpenAnimationClip;

        public void Open()
        {
            Animation.clip = OpenAnimationClip;
            Animation.Play();
        }

        public void Close()
        {
            Canvas.enabled = true;
            Animation.clip = CloseAnimationClip;
            Animation.Play();
        }
    }
}