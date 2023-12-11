using DG.Tweening;
using Scripts.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts
{
    public class UIView: MonoBehaviour
    {
        [Header("Spaces for Dynamic")]
        public RectTransform WorldSpaceTransform;
        public RectTransform FrontSpaceTransform;

        [Header("Permanent Indicators")]
        public List<ItemCounter> ItemCounters;
        public UIStickView Stick;
        public TMP_Text RankCurrentLabel;
        public TMP_Text RankNextLabel;

        [Header("Dynamic Indicators")]
        public RectTransform PointerArrowTransform;
        public Vector3 PointerArrowTargetPosition;
        public Vector3 PointerArrowTargetPositionOnNavMesh;

        [Header("Emoji")]
        public List<Sprite> EmojiSprites;

        [Header("Flying Coin")]
        public RectTransform FlyingCoinPivot;
        public float FlyingCoinMinScale = 0.2f;
        public float FlyingCoinDuration = 0.5f;

        [Header("Menu")]
        public Button SettingsButton;
        public Button MenuCloseButton;
        public Button BackgroundButton;
        public Button TitlesCloseButton;
        public Button TitlesOpenButton;
        public Button ResetButton;
        public Button ResetConfirmButton;
        public Button QuitGameButton;
        public RectTransform MenuScreen;
        public RectTransform SettingsPanel;
        public RectTransform TitlesPanel;
        public Slider SoundSlider;
        public Slider MusicSlider;

        [HideInInspector]
        public EventBus EventBus;

        private int StickIndex = -1;
        private int _currentRank;
        private bool _externalSoundChange;

        public void Awake()
        {
            SettingsButton.onClick.AddListener(() => {
                SettingsPanel.gameObject.SetActive(true);
                TitlesPanel.gameObject.SetActive(false);
                MenuScreen.gameObject.SetActive(true);
                ResetButton.gameObject.SetActive(true);
                ResetConfirmButton.gameObject.SetActive(false);
            });
            MenuCloseButton.onClick.AddListener(() => {
                SettingsPanel.gameObject.SetActive(false);
                MenuScreen.gameObject.SetActive(false);
            });
            BackgroundButton.onClick.AddListener(() => {
                MenuScreen.gameObject.SetActive(false);
            });
            TitlesOpenButton.onClick.AddListener(() => {
                SettingsPanel.gameObject.SetActive(false);
                TitlesPanel.gameObject.SetActive(true);
            });
            TitlesCloseButton.onClick.AddListener(() => {
                SettingsPanel.gameObject.SetActive(true);
                TitlesPanel.gameObject.SetActive(false);
            });
            ResetButton.onClick.AddListener(() => {
                ResetButton.gameObject.SetActive(false);
                ResetConfirmButton.gameObject.SetActive(true);
            });
            ResetConfirmButton.onClick.AddListener(() => {
                EventBus.CallEvent(new ClearGameProgressEvent() {});
            });
            QuitGameButton.onClick.AddListener(() => {
                MenuScreen.gameObject.SetActive(false);
            });
            SoundSlider.onValueChanged.AddListener(OnSoundChanged);
            MusicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        public void SetItemCount(ItemType itemType, int count)
        {
            foreach (var counter in ItemCounters)
            {
                if (counter.ItemType == itemType && itemType != ItemType.HONOR)
                {
                    counter.Counter.text = count.ToString();

                    if (counter.ProgressBarImage != null)
                    {
                        counter.ProgressBarImage.fillAmount = count / 100f;
                    }

                    if (counter.RootObject != null) 
                    {
                        var isActive = count > 0;
                        counter.RootObject.SetActive(isActive);
                    }
                }
            }
        }

        public void SetRank(int currentRank, int nextRank, int count, int rangeCount, float value)
        {
            foreach (var counter in ItemCounters)
            {
                if (counter.ItemType == ItemType.HONOR)
                {
                    if (currentRank <= 1)
                    {
                        counter.Counter.text = $"{count}";
                    }
                    else
                    {
                        counter.Counter.text = $"{count} / {rangeCount}";
                    }

                    if (counter.ProgressBarImage != null)
                    {
                        counter.ProgressBarImage.fillAmount = value;
                    }

                    if (counter.RootObject != null) 
                    {
                        var isActive = count > 0;
                        counter.RootObject.SetActive(isActive);
                    }

                    if (_currentRank != currentRank)
                    {
                        _currentRank = currentRank;
                        RankCurrentLabel.text = currentRank.ToString();
                        RankNextLabel.text = nextRank > 0 ? nextRank.ToString() : "";
                    }
                }
            }
        }

        public void FlyCoin(IconView icon, bool isIncrease = true)
        {
            var rectTransform = (RectTransform) icon.transform;
            rectTransform.SetParent(FrontSpaceTransform);
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            var rectTransformParent = (RectTransform) rectTransform.parent;
            var counterPosition = rectTransform.parent.InverseTransformPoint(FlyingCoinPivot.TransformPoint(Vector2.zero));

            var pointA = new Vector2(rectTransformParent.rect.width / 2f, rectTransformParent.rect.height / 2f);
            // var pointA = Vector2.zero;
            var pointB = new Vector2(counterPosition.x, counterPosition.y);
            var startScale = isIncrease ? FlyingCoinMinScale : 1f;
            var endScale = isIncrease ? 1f : FlyingCoinMinScale;
            var from = isIncrease ? pointA : pointB;
            var to = isIncrease ? pointB : pointA;

            rectTransform.anchoredPosition = from;
            rectTransform.DOScale(startScale, 0f).OnComplete(() => {
                rectTransform.DOScale(endScale, 1.1f);
            });

            rectTransform.DOJumpAnchorPos(to, Random.Range(0f, 500f), 1, 1.1f).OnComplete(() => {
                icon.Release();
            });
        }

        public void ProcessTouch(int touchIndex, int touchId, Vector2 position) {
            if (StickIndex == touchIndex)
            {
                var value = Stick.ProcessTouch(position);
                value.x = Mathf.Clamp(value.x, -1f, 1f);
                value.y = Mathf.Clamp(value.y, -1f, 1f);
                value.y = Mathf.Sign(value.y) * Mathf.Max(Mathf.Abs(value.y), Mathf.Abs(value.x));
                EventBus.CallEvent(new MovementInputEvent() { Direction = value });
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject(touchId))
            {
                return;
            }

            if (StickIndex < 0 && Stick.StartTouch(position))
            {
                StickIndex = touchIndex;
            }
        }

        public void EndTouch(int touchIndex, int touchId, Vector2 position) {
            if (StickIndex == touchIndex) {
                StickIndex = -1;
                Stick.EndTouch();

                EventBus.CallEvent(new MovementInputEvent() { Direction = Vector2.zero });
            }
        }

        public void SetVolume(float soundVolume, float musicVolume)
        {
            _externalSoundChange = true;
            SoundSlider.value = soundVolume;
            MusicSlider.value = musicVolume;
            _externalSoundChange = false;
        }

        private void OnSoundChanged(float value)
        {
            if (EventBus != null && !_externalSoundChange)
            {
                EventBus.CallEvent(new SetSoundVolumeEvent() { Volume = value, IsMusic = false });
            }
        }

        private void OnMusicChanged(float value)
        {
            if (EventBus != null && !_externalSoundChange)
            {
                EventBus.CallEvent(new SetSoundVolumeEvent() { Volume = value, IsMusic = true });
            }
        }
    }

    [System.Serializable]
    public class ItemCounter
    {
        public TMP_Text Counter;
        public GameObject RootObject;
        public ItemType ItemType;
        public Image ProgressBarImage;
    }
}
