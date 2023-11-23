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

        public void SetGold(int count)
        {
            GoldCountText.text = count.ToString();
        }

        public void SetHonor(int count) 
        {
            HonorCountText.text = count.ToString();

            HonorProgressBarImage.fillAmount = count / 100f;
        }
    }
}
