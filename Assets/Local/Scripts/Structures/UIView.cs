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
        public Image CurrentRankPointsBar;
        public Image LastRankPointsBar;
        public Color RankPointsBarColor;

        [Header("Dynamic Indicators")]
        public RectTransform PointerArrowTransform;
        public Image PointerArrowImage;
        public Vector3 PointerArrowTargetPosition;
        public Vector3 PointerArrowTargetPositionOnNavMesh;

        [Header("Emoji")]
        public List<Sprite> EmojiSprites;

        [Header("Flying Coin")]
        public RectTransform FlyingCoinPivot;
        public float FlyingCoinMinScale = 0.2f;
        public float FlyingCoinDuration = 0.5f;

        [Header("Flying Honor")]
        public Animation FlyingHonorAnimation;
        public TMP_Text FlyingHonorLabel;

        [Header("Tutorial")]
        public RectTransform TutorialAnimationTransform;
        public Image TutorialAnimationImage;
        public RectTransform TutorialTaskTransform;
        public TMP_Text TutorialTaskLabel;
        public List<Material> TutorialStepMaterials = new List<Material>();
        public List<Sprite> TutorialStepSprites = new List<Sprite>();
        public List<string> TutorialStrings = new List<string>();

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
        public Slider TutorialSlider;

        [Header("Ranks")]
        public Button OpenRanksButton;
        public Button CloseRanksButton;
        public Button CloseRanksButton2;
        public RectTransform RanksListTransform;

        [Header("First Honor Effect")]
        public Animation FirstHonorAnimation;

        [HideInInspector]
        public EventBus EventBus;

        [HideInInspector]
        public static float TutorialSwitch = 0.33f;

        private LocalizationLabelView[] _localizedLabels = new LocalizationLabelView[0];
        private int _currentLangID = -1;
        private int StickIndex = -1;
        private int _currentRank;
        private int _currentHonorPoints;
        private bool _externalSoundChange;
        private bool _tutorialEnabled = false;
        private bool _showTutorial = false;
        private int _tutorialStepId = -1;
        private IQueuedEvent _rankPopupEvent;

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
            CloseRanksButton.onClick.AddListener(CloseRanks);
            CloseRanksButton2.onClick.AddListener(CloseRanks);
            OpenRanksButton.onClick.AddListener(OpenRanks);

            TutorialSlider.value = TutorialSwitch;
            TutorialSlider.onValueChanged.AddListener(OnTutorialChanged);

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
                    if (count > _currentHonorPoints)
                    {
                        _currentHonorPoints = count;
                        LastRankPointsBar.fillAmount = CurrentRankPointsBar.fillAmount;
                        CurrentRankPointsBar.color = Color.white;
                        CurrentRankPointsBar.DOColor(RankPointsBarColor, 0.3f);
                    }

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

        public void RanksSetup()
        {
            RankEffectCurrentLabel.text = (_currentRank + 1).ToString();
            RankEffectNextLabel.text = _currentRank > 0 ? _currentRank.ToString() : "";
            RankEffect.gameObject.SetActive(true);
            var lineHeight = ((RectTransform) RanksListTransform.GetChild(0)).sizeDelta.y;
            for (var i = 0; i < RanksListTransform.childCount; i++)
            {
                var line = RanksListTransform.GetChild(i);
                var visible = i <= 7 - _currentRank;
                var active = i <= 6 - _currentRank;
                line.gameObject.SetActive(visible);
                line.GetChild(0).gameObject.SetActive(!active);
                line.GetChild(1).gameObject.SetActive(active);
            }
            RanksListTransform.anchoredPosition = new Vector2(RanksListTransform.anchoredPosition.x, lineHeight * (4f - _currentRank));
            //RankEffect.transform.DOScale(1f, 5f).OnComplete(() => {
            //    RankEffect.gameObject.SetActive(false);
            //});
        }

        public void OpenRanks()
        {
            RanksSetup();
            RankEffect.Play("RankShort");
        }

        public void OpenNewRank(IQueuedEvent queuedEvent)
        {
            _rankPopupEvent = queuedEvent;
            RanksSetup();
            RankEffect.Play("Rank");
        }

        public void CloseRanks()
        {
            RankEffect.Play("RankClose");
            RankEffect.transform.DOScale(1f, 0.2f).OnComplete(() => {
                RankEffect.gameObject.SetActive(false);
                if (_rankPopupEvent != null)
                {
                    _rankPopupEvent.Locked = false;
                    _rankPopupEvent = null;
                }
            });
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
            _tutorialEnabled = false;
            if (TutorialTaskTransform.gameObject.activeSelf)
            {
                TutorialTaskTransform.DOScale(0.1f, 0.2f).OnComplete(() => {
                    TutorialTaskTransform.gameObject.SetActive(false);
                });
            }
            TutorialAnimationTransform.DOComplete(false);
            TutorialAnimationTransform.DOScale(0.01f, 0.5f).OnComplete(() => {
                TutorialAnimationTransform.gameObject.SetActive(false);
                TryShowTutorial();
            });
        }

        public int GetCurrentTutorialStep()
        {
            return _tutorialEnabled ? _tutorialStepId : -1;
        }

        public void ShowFlyingHonor(int value)
        {
            FlyingHonorLabel.text = "+" + value;
            FlyingHonorAnimation.Play();
        }

        public void ShowFirstHonorEffect()
        {
            FirstHonorAnimation.gameObject.SetActive(true);
            FirstHonorAnimation.Play();
            FirstHonorAnimation.transform.DOScale(1f, 5f).OnComplete(() => {
                FirstHonorAnimation.gameObject.SetActive(false);
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
                _tutorialEnabled = true;
                if (Mathf.Abs(TutorialSlider.value - 1f) < 0.1f || Mathf.Abs(TutorialSlider.value - 0.33f) < 0.1f)
                {
                    TutorialTaskTransform.gameObject.SetActive(true);
                    TutorialTaskTransform.DOScale(1.5f, 0.2f).OnComplete(() => {
                        TutorialTaskTransform.DOScale(1f, 0.2f);
                    });
                }
                TutorialTaskLabel.text = TutorialStrings[_tutorialStepId];
                if (_tutorialStepId < 0)
                {
#if DEBUG
                    Debug.LogWarning($"Wrong tutorial step index: '{_tutorialStepId}'.");
#endif
                }
                else if (_tutorialStepId >= TutorialStepMaterials.Count || _tutorialStepId >= TutorialStepSprites.Count)
                {
#if DEBUG
                    Debug.LogWarning($"Tutorial material with index '{_tutorialStepId}' not set.");
#endif
                }
                else if (TutorialStepMaterials[_tutorialStepId] == null)
                {
#if DEBUG
                    Debug.LogWarning($"Tutorial material with index '{_tutorialStepId}' is empty.");
#endif
                }
                else if (TutorialStepSprites[_tutorialStepId] == null)
                {
#if DEBUG
                    Debug.LogWarning($"Tutorial sprite with index '{_tutorialStepId}' is empty.");
#endif
                }
                else
                {
                    if (Mathf.Abs(TutorialSlider.value - 1f) < 0.1f || Mathf.Abs(TutorialSlider.value - 0.66f) < 0.1f)
                    {
                        TutorialAnimationTransform.gameObject.SetActive(true);
                        TutorialAnimationTransform.DOScale(1f, 0.5f).OnComplete(() => {
                        });
                    }
                    TutorialAnimationImage.material = TutorialStepMaterials[_tutorialStepId];
                    TutorialAnimationImage.sprite = TutorialStepSprites[_tutorialStepId];
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

        private void OnTutorialChanged(float value)
        {
            if (TutorialSlider.enabled)
            {
                TutorialSlider.enabled = false;
                TutorialSwitch = Mathf.Round(value * 3f) / 3f;
                TutorialSlider.value = TutorialSwitch;
                var taskEnabled = Mathf.Abs(TutorialSwitch - 0.33f) < 0.1f || Mathf.Abs(TutorialSwitch - 1f) < 0.1f;
                var bubbleEnabled = Mathf.Abs(TutorialSwitch - 0.66f) < 0.1f || Mathf.Abs(TutorialSwitch - 1f) < 0.1f;
                TutorialTaskTransform.gameObject.SetActive(taskEnabled);
                TutorialAnimationTransform.gameObject.SetActive(bubbleEnabled);
                PointerArrowImage.enabled = !(Mathf.Abs(TutorialSwitch) < 0.1f);
                TutorialSlider.enabled = true;
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
