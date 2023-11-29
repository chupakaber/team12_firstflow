using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class BagOfTriesView : PoolableObject
    {
        public RectTransform Transform;
        public RectTransform Circle;
        public List<RectTransform> Points;
        public List<Image> PointImages;
        public int PointsCount;
        
        [SerializeField]
        private float _radialScale = 50f;

        public void Roll(int index, float duration)
        {
            var stopAngle = 360f * index / Points.Count;
            Circle.DOLocalRotate(new Vector3(0f, 0f, stopAngle), duration, RotateMode.Fast);
        }

        public void SetValue(int index, bool value)
        {
            PointImages[index].DOFillAmount(value ? 1f : 0f, 0.8f);
        }

        public void Resize(int size)
        {
            PointsCount = size;

            for (var i = 0; i < Points.Count; i++)
            {
                var isActive = i < size;
                if (isActive)
                {
                    if (!Points[i].gameObject.activeSelf)
                    {
                        Points[i].gameObject.SetActive(true);
                    }
                    var degAngle = 360f * i / size;
                    var radAngle = Mathf.PI * 2f * i / size;
                    Points[i].anchoredPosition = new Vector2(Mathf.Sin(radAngle), Mathf.Cos(radAngle)) * _radialScale;
                    Points[i].transform.localRotation = Quaternion.Euler(0f, 0f, degAngle);
                }
                else
                {
                    if (Points[i].gameObject.activeSelf)
                    {
                        Points[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        public void Show()
        {
            if (PointsCount == 0)
            {
                return;
            }

            Transform.gameObject.SetActive(true);
            Transform.DOScale(1f, 0.2f);
        }

        public void Hide()
        {
            Transform.DOScale(0f, 0.2f).OnComplete(() => {
                Transform.gameObject.SetActive(false);
            });
        }
    }
}