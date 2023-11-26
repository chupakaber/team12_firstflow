using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class UIView: MonoBehaviour
    {
        public TMP_Text GoldCountText;
        public TMP_Text HonorCountText;
        public Image HonorProgressBarImage;
        public RectTransform WorldSpaceTransform;
        public RectTransform FrontSpaceTransform;

        public void SetGold(int count)
        {
            GoldCountText.text = count.ToString();
        }

        public void SetHonor(int count) 
        {
            HonorCountText.text = count.ToString();

            HonorProgressBarImage.fillAmount = count / 100f;
        }

        public void FlyIcon(IconView icon)
        {
            var rectTransform = (RectTransform) icon.transform;
            rectTransform.SetParent(FrontSpaceTransform);
            rectTransform.localScale = Vector3.one;
            var rectTransformParent = (RectTransform) rectTransform.parent;
            rectTransform.anchoredPosition = new Vector2(rectTransformParent.rect.width / 2f, rectTransformParent.rect.height / 2f);
            var newPosition = -rectTransform.parent.TransformPoint(GoldCountText.rectTransform.parent.InverseTransformPoint(GoldCountText.rectTransform.anchoredPosition));
            rectTransform.DOScale(0.2f, 0f).OnComplete(() => {
                rectTransform.DOScale(1f, 0.5f);
            });
            rectTransform.DOScale(1f, 0.5f);
            rectTransform.DOAnchorPos(newPosition, 0.5f).OnComplete(() => {
                icon.Release();
            });
        }
    }
}
