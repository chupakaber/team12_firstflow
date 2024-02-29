using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class ProgressBarView: MonoBehaviour
    {
        [SerializeField] protected Image _background;
        [SerializeField] protected Image _image;

        public virtual float Progress
        {
            set
            {
                _image.fillAmount = value;
                
                if (value <= float.Epsilon)
                {
                    _image.enabled = false;
                }
                else
                {
                    _image.enabled = true;
                }

                _background.enabled = _image.enabled;
            }
        }
    }
}
