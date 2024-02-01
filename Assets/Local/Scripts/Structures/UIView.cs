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
        public Animation RankEffect;
        public TMP_Text RankEffectCurrentLabel;
        public TMP_Text RankEffectNextLabel;

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

        [Header("Tutorial")]
        public RectTransform TutorialAnimationTransform;
        public Image TutorialAnimationImage;
        public List<Material> TutorialStepMaterials = new List<Material>();
        public List<Sprite> TutorialStepSprites = new List<Sprite>();

        [Header("Menu")]
        public Button SettingsButton;
        public Button MenuCloseButton;
        public Button BackgroundButton;
        public Button TitlesCloseButton;
        public Button TitlesOpenButton;
        public Button ResetButton;
        public Button ResetConfirmButton;
        public Button QuitGameButton;
        public Button LanguageButton;
        public RectTransform MenuScreen;
        public RectTransform SettingsPanel;
        public RectTransform TitlesPanel;
        public Slider SoundSlider;
        public Slider MusicSlider;

        [HideInInspector]
        public EventBus EventBus;

        private LocalizationLabelView[] _localizedLabels = new LocalizationLabelView[0];
        private int _currentLangID = -1;
        private int StickIndex = -1;
        private int _currentRank;
        private bool _externalSoundChange;
        private bool _showTutorial = false;
        private bool _hideTutorial = false;
        private int _tutorialStepId = -1;

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
                EventBus.CallEvent(new SaveEvent() {});
                Application.Quit();
            });
            SoundSlider.onValueChanged.AddListener(OnSoundChanged);
            MusicSlider.onValueChanged.AddListener(OnMusicChanged);
            LanguageButton.onClick.AddListener(() => {
                SwitchLanguage(_currentLangID == 1 ? 0 : 1);
            });

            _localizedLabels = FindObjectsOfType<LocalizationLabelView>();
            SwitchLanguage(Application.systemLanguage == SystemLanguage.Russian ? 1 : 0);

            MenuScreen.gameObject.SetActive(false);
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

                        if (currentRank < 6)
                        {
                            RankEffectCurrentLabel.text = (currentRank + 1).ToString();
                            RankEffectNextLabel.text = currentRank > 0 ? currentRank.ToString() : "";
                            RankEffect.gameObject.SetActive(true);
                            RankEffect.Play();
                            RankEffect.transform.DOScale(1f, 5f).OnComplete(() => {
                                RankEffect.gameObject.SetActive(false);
                            });
                        }
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

        public void SwitchLanguage(int langID)
        {
            if (_currentLangID == langID)
            {
                return;
            }

            _currentLangID = langID;

            foreach (var label in _localizedLabels)
            {
                label.SwitchLanguage(_currentLangID);
            }
        }

        public void ShowTutorial(int id)
        {
            _showTutorial = true;
            _tutorialStepId = id;
            TryShowTutorial();
        }

        public void HideTutorial()
        {
            TutorialAnimationTransform.DOComplete(false);
            TutorialAnimationTransform.DOScale(0.01f, 0.5f).OnComplete(() => {
                TutorialAnimationTransform.gameObject.SetActive(false);
                TryShowTutorial();
            });
        }

        private void TryShowTutorial()
        {
            if (!_showTutorial)
            {
                return;
            }

            if (!TutorialAnimationTransform.gameObject.activeSelf)
            {
                _showTutorial = false;
                TutorialAnimationTransform.gameObject.SetActive(true);
                if (_tutorialStepId < 0)
                {
                    throw new UnityException($"Wrong tutorial step index: '{_tutorialStepId}'.");
                }
                else if (_tutorialStepId >= TutorialStepMaterials.Count || _tutorialStepId >= TutorialStepSprites.Count)
                {
                    throw new UnityException($"Tutorial material with index '{_tutorialStepId}' not set.");
                }
                else if (TutorialStepMaterials[_tutorialStepId] == null)
                {
                    throw new UnityException($"Tutorial material with index '{_tutorialStepId}' is empty.");
                }
                else if (TutorialStepSprites[_tutorialStepId] == null)
                {
                    throw new UnityException($"Tutorial sprite with index '{_tutorialStepId}' is empty.");
                }
                else
                {
                    TutorialAnimationImage.material = TutorialStepMaterials[_tutorialStepId];
                    TutorialAnimationImage.sprite = TutorialStepSprites[_tutorialStepId];
                    TutorialAnimationTransform.DOScale(1f, 0.5f).OnComplete(() => {
                    });
                }
            }
            else
            {
                HideTutorial();
            }
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
