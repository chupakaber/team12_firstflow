using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class ProgressBarView: MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _image;

        public float Progress 
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
