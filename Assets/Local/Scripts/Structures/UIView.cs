using UnityEngine;
using TMPro;

namespace Scripts
{
    public class UIView: MonoBehaviour
    {
        public TMP_Text GoldCountText;
        public TMP_Text HonorCountText;
        public RectTransform HonorProgressBarTransform;
        public RectTransform WorldSpaceTransform;

        public void SetGold(int count)
        {
            GoldCountText.text = count.ToString();
        }

        public void SetHonor(int count) 
        {
            HonorCountText.text = count.ToString();
            
            var progressBarSize = HonorProgressBarTransform.sizeDelta;
            progressBarSize.x = count / 100f * ((RectTransform)HonorProgressBarTransform.parent).rect.width;
            HonorProgressBarTransform.sizeDelta = progressBarSize;
        }
    }
}
