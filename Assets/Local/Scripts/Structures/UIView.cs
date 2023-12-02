using DG.Tweening;
using Scripts.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class UIView: MonoBehaviour
    {
        [Header("Spaces for Dynamic")]
        public RectTransform WorldSpaceTransform;
        public RectTransform FrontSpaceTransform;

        [Header("Permanent Indicators")]
        public List<ItemCounter> ItemCounters; 

        [Header("Flying Coin")]
        public RectTransform FlyingCoinPivot;
        public float FlyingCoinMinScale = 0.2f;
        public float FlyingCoinDuration = 0.5f;

        public void SetItemCount(ItemType itemType, int count)
        {
            foreach (var counter in ItemCounters)
            {
                if (counter.ItemType == itemType) 
                {
                    counter.Counter.text = count.ToString();

                    if (counter.ProgressBarImage != null)
                    {
                        counter.ProgressBarImage.fillAmount = count / 100f;
                    }

                    if (counter.RootObject != null) 
                    {
                        var isActive = count > 0;
                        counter.RootObject.SetActive(isActive);
                    }
                }
            }
        }

        public void FlyCoin(IconView icon, bool isIncrease = true)
        {
            var rectTransform = (RectTransform) icon.transform;
            rectTransform.SetParent(FrontSpaceTransform);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            var rectTransformParent = (RectTransform) rectTransform.parent;
            var counterPosition = rectTransform.parent.InverseTransformPoint(FlyingCoinPivot.TransformPoint(Vector2.zero));

            var pointA = new Vector2(rectTransformParent.rect.width / 2f, rectTransformParent.rect.height / 2f);
            // var pointA = Vector2.zero;
            var pointB = new Vector2(counterPosition.x, counterPosition.y);
            var startScale = isIncrease ? FlyingCoinMinScale : 1f;
            var endScale = isIncrease ? 1f : FlyingCoinMinScale;
            var from = isIncrease ? pointA : pointB;
            var to = isIncrease ? pointB : pointA;

            rectTransform.anchoredPosition = from;
            rectTransform.DOScale(startScale, 0f).OnComplete(() => {
                rectTransform.DOScale(endScale, 1.1f);
            });

            rectTransform.DOJumpAnchorPos(to, Random.Range(0f, 500f), 1, 1.1f).OnComplete(() => {
                icon.Release();
            });
        }
    }

    [System.Serializable]
    public class ItemCounter
    {
        public TMP_Text Counter;
        public GameObject RootObject;
        public ItemType ItemType;
        public Image ProgressBarImage;
    }
}
