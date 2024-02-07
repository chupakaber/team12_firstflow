using UnityEngine;
using TMPro;

namespace Scripts
{
    public class TimerBarView: ProgressBarView
    {
        [SerializeField] protected TMP_Text _text;
        
        public int ValueFromStateID = -1;
        public Building Building;

        private float _lastValue = -1000;

        public override float Progress
        {
            set
            {
                var visible = value > float.Epsilon;

                if (_image != null)
                {
                    _image.fillAmount = value;
                    _image.enabled = visible;
                }

                if (_text != null)
                {
                    _text.enabled = visible;
                    if (_text.enabled)
                    {
                        if (Mathf.Abs(Mathf.Floor(value) - Mathf.Floor(_lastValue)) > 0.5f)
                        {
                            var v = Mathf.Max(0f, Mathf.Floor(value));
                            var seconds = v % 60f;
                            var minutes = (v - seconds) / 60f % 60f;
                            var hours = ((v - seconds) / 60f - minutes) / 60f % 24f;
                            var stringValue = $"{(int)hours:D2}:{(int)minutes:D2}:{(int)seconds:D2}";
                            _text.text = stringValue;
                        }
                    }
                }

                _background.enabled = visible;
                _lastValue = value;
            }
        }
    }
}
