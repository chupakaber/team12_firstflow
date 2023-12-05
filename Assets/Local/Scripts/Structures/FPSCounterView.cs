using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UnityComponents
{
    public class FPSCounterView: MonoBehaviour
    {
        public float SmoothFPS = 0f;
        public TMP_Text Label;

        public void Update()
        {
            SmoothFPS = Mathf.Lerp(SmoothFPS, 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f), 0.2f);
            Label.text = $"{SmoothFPS:N0}";
        }
    }
}