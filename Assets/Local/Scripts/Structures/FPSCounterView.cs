using TMPro;
using UnityEngine;

namespace Scripts.UnityComponents
{
    public class FPSCounterView: MonoBehaviour
    {
        public float SmoothFPS = 0f;
        public TMP_Text Label;
        public GameObject LogPanel;
        public TMP_Text LogLabel;

        public void Update()
        {
            SmoothFPS = Mathf.Lerp(SmoothFPS, 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f), 0.2f);
            Label.text = $"FPS: {SmoothFPS:N0}";
        }

        public void Start()
        {
            Application.logMessageReceived += OnLogCallback;
        }

        private void OnLogCallback(string condition, string stackTrace, LogType type)
        {
            LogLabel.text += $"[{type}] {condition}\n";
        }

        public void SwitchLogPanel()
        {
            LogPanel.SetActive(!LogPanel.activeSelf);
        }
    }
}