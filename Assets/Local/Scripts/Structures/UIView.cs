using DG.Tweening;
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
        public TMP_Text GoldCountText;
        public TMP_Text HonorCountText;
        public Image HonorProgressBarImage;

        [Header("Flying Coin")]
        public RectTransform FlyingCoinPivot;
        public float FlyingCoinMinScale = 0.2f;
        public float FlyingCoinDuration = 0.5f;

        public void SetGold(int count)
        {
            GoldCountText.text = count.ToString();
        }

        public void SetHonor(int count) 
        {
            HonorCountText.text = count.ToString();

            HonorProgressBarImage.fillAmount = count / 100f;
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
                rectTransform.DOScale(endScale, 0.5f);
            });

            rectTransform.DOAnchorPos(to, 0.5f).OnComplete(() => {
                icon.Release();
            });
        }
    }
}
