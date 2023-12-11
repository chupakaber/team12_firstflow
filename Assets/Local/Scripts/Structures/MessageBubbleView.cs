using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class MessageBubbleView : PoolableObject
    {
        public Transform RelatedTransform;
        public Vector3 WorldOffset;

        public Vector2 ScreenOffset = new Vector2();

        [SerializeField]
        private RectTransform _self;
        [SerializeField]
        private RectTransform[] _arrows;
        [SerializeField]
        private TMP_Text _label;
        [SerializeField]
        private Image _icon;
        
        private Camera _mainCamera;

        public void Init(Camera camera)
        {
            _mainCamera = camera;
        }

        public void SetMessage(string value)
        {
            _label.text = value;
            StartCoroutine(UpdateAsync());
        }

        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }

        public void SetWorldAnchor(Vector3 position)
        {
            var screenPosition = _mainCamera.WorldToScreenPoint(position);
            SetAnchor(new Vector2(screenPosition.x, screenPosition.y));
        }
        
        public void SetAnchor(Vector2 position)
        {
            var localPosition = _self.parent.InverseTransformPoint(position);
            _self.localPosition = localPosition;
            var parentRect = ((RectTransform) _self.parent).rect;
            var dockSignX = _self.anchoredPosition.x > parentRect.width / 2f ? 0 : 1;
            var dockSignY = _self.anchoredPosition.y > parentRect.height / 2f ? 0 : 2;
            _self.anchoredPosition += new Vector2((dockSignX - 1) * (_self.rect.width + 10f) + ScreenOffset.x, (dockSignY - 1) * (_self.rect.height / 2f + 60f) + ScreenOffset.y);

            for (var i = 0; i < _arrows.Length; i++)
            {
                var arrow = _arrows[i];
                var isActive = i == dockSignX + dockSignY;
                if (arrow.gameObject.activeSelf != isActive)
                {
                    arrow.gameObject.SetActive(isActive);
                }
            }
        }

        private IEnumerator UpdateAsync()
        {
            yield return new WaitForEndOfFrame();
            _self.sizeDelta = new Vector2(_label.rectTransform.rect.width, _label.rectTransform.rect.height);
        }
    }
}