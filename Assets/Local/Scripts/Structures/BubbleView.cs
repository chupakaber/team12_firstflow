using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class BubbleView: PoolableObject
    {
        public Image Icon;
        public Transform RelatedTransform;
        public Vector3 WorldOffset;

        public void SetIcon(Sprite sprite)
        {
            Icon.sprite = sprite;
        }
    }
}