using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class AnimationEffect : MonoBehaviour
    {
        public Animation Animation;
        public List<ParticleSystem> ParticleSystems;

        public void Activate()
        {
            foreach (var particleSystem in ParticleSystems)
            {
                particleSystem.gameObject.SetActive(true);
                particleSystem.Clear();
                particleSystem.Play();
            }

            Animation.Play();
        }

        public void Deactivate()
        {
            foreach (var particleSystem in ParticleSystems)
            {
                particleSystem.Stop();
                particleSystem.gameObject.SetActive(false);
            }
        }
    }
}