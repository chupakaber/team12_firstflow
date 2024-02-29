using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class UIStickView: MonoBehaviour
    {
        public RectTransform Transform;
        public RectTransform Pointer;
        public float Radius;
        public float VisualRadius;
        public float ValuePow = 1f;
        public Vector2 ValueScale = Vector2.one;
        public Vector2 ActiveAreaMin;
        public Vector2 ActiveAreaMax;
        public CanvasScaler CanvasScaler;
        public Vector2 Value { get {
            return _value * ValueScale;
        } }

        private Camera _cameraMain;
        private float _aspect;
        private Vector2 _startPosition;
        private Vector2 _value = Vector2.zero;

        public void Start()
        {
            OnEnable();
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            if (_cameraMain == null)
            {
                _cameraMain = Camera.main;
            }
            var screenResolution = _cameraMain.pixelRect;
            _aspect = CanvasScaler.referenceResolution.y / screenResolution.height;
        }

        public bool StartTouch(Vector2 position)
        {
            _startPosition = position;
            var screenResolution = _cameraMain.pixelRect;
            var activeAreaMinX = ActiveAreaMin.x * screenResolution.width;
            var activeAreaMaxX = ActiveAreaMax.x * screenResolution.width;
            var activeAreaMinY = ActiveAreaMin.y * screenResolution.height;
            var activeAreaMaxY = ActiveAreaMax.y * screenResolution.height;
            if (position.x >= activeAreaMinX && position.y >= activeAreaMinY && position.x <= activeAreaMaxX && position.y <= activeAreaMaxY)
            {
                Transform.anchoredPosition = position * _aspect;
                gameObject.SetActive(true);
                return true;
            }
            return false;
        }

        public void EndTouch()
        {
            gameObject.SetActive(false);
        }

        public Vector2 ProcessTouch(Vector2 position)
        {
            var delta = position - _startPosition;
            var convertedDelta = delta * _aspect;
            convertedDelta = convertedDelta.normalized * Mathf.Min(convertedDelta.magnitude, Radius);
            var visualConvertedDelta = convertedDelta.normalized * Mathf.Min(convertedDelta.magnitude, VisualRadius);
            Pointer.anchoredPosition = visualConvertedDelta;
            _value = convertedDelta / Radius;
            _value.x = Mathf.Sign(_value.x) * Mathf.Pow(_value.x, ValuePow);
            _value.y = Mathf.Sign(_value.y) * Mathf.Pow(_value.y, ValuePow);
            return _value;
        }
    }
}