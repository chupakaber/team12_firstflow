using System.Collections.Generic;
using DG.Tweening;
using Scripts.BehaviorTree;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    internal class UISystem: ISystem
    {
        public List<Character> Characters;
        public List<ProgressBarView> ProgressBarViews;
        public List<TimerBarView> TimerBarViews;
        public List<Building> Buildings;
        public EventBus EventBus;
        public UIView UIView;
        public Camera Camera;
        public Scenario Scenario;
        public PoolCollection<BagOfTriesView> BagOfTriesViewPools;
        public PoolCollection<PinnedCounterView> PinnedCounterViewPools;
        public PoolCollection<BubbleView> BubbleViewPools;
        public PoolCollection<MessageBubbleView> MessageBubbleViewPools;

        private Dictionary<int, PinnedCounterView> _shopCoinCounters = new Dictionary<int, PinnedCounterView>();
        private LinkedList<BubbleView> _bubbleViews = new LinkedList<BubbleView>();
        private LinkedList<MessageBubbleView> _messageBubbleViews = new LinkedList<MessageBubbleView>();
        private LinkedList<MessageBubbleView> _cartoonBubbleViews = new LinkedList<MessageBubbleView>();
        private LinkedList<TouchInputEvent> _touchInputEvents = new LinkedList<TouchInputEvent>();
        private NavMeshPath _path;
        private Vector3[] _pathCorners = new Vector3[100];
        private Vector3 _targetArrowWorldPosition = Vector3.zero;
        private int _currentRank = 6;

        public void EventCatch(StartEvent newEvent)
        {
            UIView.EventBus = EventBus;

            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.PLAYER)
                {
                    foreach (var item in character.Items) 
                    {
                        UpdateItemsCount(character, item.Type);
                    }
                }
            }

            var timerBars = Object.FindObjectsOfType<TimerBarView>();
            foreach (var timerBar in timerBars)
            {
                TimerBarViews.Add(timerBar);
            }
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            UpdateItemsCount(newEvent.Unit, newEvent.ItemType);

            if (newEvent.Unit is Player && newEvent.ItemType == ItemType.HONOR)
            {
                UIView.ShowFlyingHonor(newEvent.Count);
            }
        }

        public void EventCatch(RemoveItemEvent newEvent)
        {
            UpdateItemsCount(newEvent.Unit, newEvent.ItemType);
        }

        public void EventCatch(UpdateEvent newEvent)
        {
            for (var i = 0; i< ProgressBarViews.Count; i++)
            {
                var progressBar = ProgressBarViews[i];

                if (progressBar != null)
                {
                    var building = Buildings[i];

                    if (building.ProductionArea == null && building.ProgressBarPivot.gameObject.activeSelf)
                    {
                        if (!progressBar.gameObject.activeSelf)
                        {
                            progressBar.gameObject.SetActive(true);
                        }

                        progressBar.Progress = building.ProductionProgress();

                        var worldPosition = building.ProgressBarPivot.position;
                        var screenPosition = Camera.WorldToScreenPoint(worldPosition);
                        var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                        var progressBarTransform = (RectTransform)progressBar.transform;
                        progressBarTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                    }
                    else
                    {
                        if (progressBar.gameObject.activeSelf)
                        {
                            progressBar.gameObject.SetActive(false);
                        }

                        if (building.ProductionCharacters.Count > 0 && building.ProductionCharacters[0].BagOfTriesView != null)
                        {
                            building.ProductionCharacters[0].BagOfTriesView.SetProgress(building.ProductionProgress());
                        }
                    }
                }
            }

            for (var i = 0; i< TimerBarViews.Count; i++)
            {
                var timerBar = TimerBarViews[i];
                var building = timerBar.Building;

                var value = 0f;
                if (timerBar.ValueFromStateID > -1)
                {
                    value = Scenario.BehaviorTreeRunner.InternalState.GetState(timerBar.ValueFromStateID);
                }
                else if (building != null)
                {
                    value = Mathf.Max(0f, building.ProductionEndActivityTime - Time.time);
                }
                timerBar.Progress = value;

                if (building != null)
                {
                    var worldPosition = building.transform.position + Vector3.up * 3f;
                    var screenPosition = Camera.WorldToScreenPoint(worldPosition);
                    var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                    var timerBarTransform = (RectTransform)timerBar.transform;
                    timerBarTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                }
            }

            foreach (var character in Characters)
            {
                if (character.BagOfTriesView != null && character.BagOfTriesView.gameObject.activeSelf)
                {
                    var worldPosition = character.transform.position + Vector3.up * 3.5f;
                    var screenPosition = Camera.WorldToScreenPoint(worldPosition);
                    var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                    character.BagOfTriesView.Transform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);

                }
            }

            foreach (var building in Buildings)
            {
                if (building.ProduceItemType == ItemType.GOLD && building.Level > 0)
                {
                    if (_shopCoinCounters.TryGetValue(building.GetHashCode(), out var counter))
                    {
                        var worldPosition = building.ItemStackView.GetTopPosition(building.ProduceItemType);
                        if (building.Items.GetAmount(building.ProduceItemType) > 0 && worldPosition.sqrMagnitude > float.Epsilon)
                        {
                            if (!counter.gameObject.activeSelf)
                            {
                                counter.gameObject.SetActive(true);
                            }

                            var screenPosition = Camera.WorldToScreenPoint(worldPosition);
                            var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                            counter.transform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                        }
                        else if (counter.gameObject.activeSelf)
                        {
                            counter.gameObject.SetActive(false);
                        }
                    }
                }   
            }

            SmartCharacter player = null;
            foreach (var character in Characters)
            {
                if (character.CharacterType == CharacterType.PLAYER)
                {
                    player = (SmartCharacter) character;
                }
            }

            if (UIView.PointerArrowTargetPosition.sqrMagnitude <= float.Epsilon || (player.transform.position - UIView.PointerArrowTargetPosition).magnitude < 5f)
            {
                if (UIView.PointerArrowTransform.gameObject.activeSelf && UIView.PointerArrowTargetPosition.sqrMagnitude <= float.Epsilon)
                {
                    UIView.PointerArrowTransform.gameObject.SetActive(false);
                }

                if (UIView.PointerArrowTransform.gameObject.activeSelf)
                {
                    var screenPosition = Camera.WorldToScreenPoint(UIView.PointerArrowTargetPosition + Vector3.up * 1.5f);
                    var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                    UIView.PointerArrowTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                    UIView.PointerArrowTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                    _targetArrowWorldPosition = UIView.PointerArrowTargetPosition;
                    screenPosition = Camera.WorldToScreenPoint(UIView.PointerArrowTargetPosition);
                    UIView.TutorialAnimationTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                }
            }
            else
            {
                if (!UIView.PointerArrowTransform.gameObject.activeSelf)
                {
                    UIView.PointerArrowTransform.gameObject.SetActive(true);
                }

                var direction = (Vector3.back + Vector3.right).normalized;
                var worldPosition = UIView.PointerArrowTargetPosition;

                if (_path == null)
                {
                    _path = new NavMeshPath();
                }
                player.NavMeshAgent.enabled = true;
                var navMeshWorldPosition = worldPosition;
                if (UIView.PointerArrowTargetPositionOnNavMesh.sqrMagnitude > float.Epsilon)
                {
                    navMeshWorldPosition = UIView.PointerArrowTargetPositionOnNavMesh;
                }
                if (player.NavMeshAgent.CalculatePath(navMeshWorldPosition, _path))
                {
                    var cornersCount = _path.GetCornersNonAlloc(_pathCorners);
                    if (TryGetPathPositionAndDirection(cornersCount, 2.5f, out var pointerPosition, out var pointerDirection))
                    {
                        worldPosition = pointerPosition;
                        direction = pointerDirection;
                        if (TryGetPathPositionAndDirection(cornersCount, 5f, out var pointerPosition2, out var pointerDirection2))
                        {
                            direction = (pointerPosition2 - pointerPosition).normalized;
                        }
                    }
                }
                player.NavMeshAgent.enabled = false;

                worldPosition += Vector3.up * 1.1f;

                _targetArrowWorldPosition = Vector3.Lerp(_targetArrowWorldPosition, worldPosition, Time.deltaTime * 6f);
                var screenPosition = Camera.WorldToScreenPoint(_targetArrowWorldPosition);
                var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                UIView.PointerArrowTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                direction = Quaternion.Euler(0f, 45f, 0f) * direction;
                direction.z *= 0.7f;
                UIView.PointerArrowTransform.localRotation = Quaternion.Euler(0f, 0f, -Quaternion.LookRotation(direction).eulerAngles.y);

                screenPosition = Camera.WorldToScreenPoint(UIView.PointerArrowTargetPosition);
                UIView.TutorialAnimationTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
            }

            foreach (var bubble in _bubbleViews)
            {
                var worldPosition = bubble.RelatedTransform.position + bubble.WorldOffset;
                var screenPosition = Camera.WorldToScreenPoint(worldPosition);
                var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                bubble.transform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
            }

            foreach (var bubble in _messageBubbleViews)
            {
                bubble.SetWorldAnchor(bubble.RelatedTransform.position + bubble.WorldOffset);
            }

            foreach (var touchInputEvent in _touchInputEvents)
            {
                foreach (var character in Characters)
                {
                    if (character.CharacterType == CharacterType.PLAYER)
                    {
                        if (touchInputEvent.End)
                        {
                            UIView.EndTouch(touchInputEvent.Index, touchInputEvent.TouchID, touchInputEvent.Position);
                            character.WorldDirection = Vector2.zero;
                        }
                        else
                        {
                            UIView.ProcessTouch(touchInputEvent.Index, touchInputEvent.TouchID, touchInputEvent.Position);
                            character.WorldDirection = Quaternion.Euler(0f, Camera.transform.eulerAngles.y, 0f) * new Vector3(UIView.Stick.Value.x, 0f, UIView.Stick.Value.y);
                            character.WorldDirection = character.WorldDirection.normalized * (0.2f + character.WorldDirection.magnitude * 0.8f);
                        }
                    }
                }
            }
            _touchInputEvents.Clear();
        }

        private bool TryGetPathPositionAndDirection(int count, float distance, out Vector3 position, out Vector3 direction)
        {
            if (count < 1)
            {
                position = Vector3.zero;
                direction = (Vector3.back + Vector3.right).normalized;
                return false;
            }
            
            var d = 0f;
            var index = 0;
            while (index < count - 1)
            {
                var previousCorner = _pathCorners[index];
                var nextCorner = _pathCorners[index + 1];
                var delta = nextCorner - previousCorner;
                var segmentLength = delta.magnitude;
                if (d + segmentLength < distance)
                {
                    d += segmentLength;
                }
                else
                {
                    position = previousCorner + delta.normalized * (distance - d);
                    direction = delta.normalized;
                    return true;
                }
                index++;
            }

            position = _pathCorners[count - 1];
            direction = (Vector3.back + Vector3.right).normalized;
            return false;
        }

        public void EventCatch(RollBagOfTriesEvent newEvent)
        {
            var character = newEvent.Character;

            if (character.BagOfTriesView == null)
            {
                character.BagOfTriesView = BagOfTriesViewPools.Get(0);

                character.BagOfTriesView.ShowTries = character.CharacterType != CharacterType.PLAYER;
                
                character.BagOfTriesView.Transform.SetParent(UIView.FrontSpaceTransform);
                character.BagOfTriesView.Transform.localScale = Vector3.one;

                character.BagOfTriesView.Resize(character.BagOfTries.Values.Count);

                for (var i = 0; i < character.BagOfTries.Values.Count; i++)
                {
                    character.BagOfTriesView.SetValue(i, character.BagOfTries.Values[i]);
                }
            }

            character.BagOfTriesView.Roll(newEvent.NextIndex, 0.2f);
            character.BagOfTriesView.SetValue(newEvent.LastIndex, newEvent.ChangedValue);
            character.BagOfTriesView.ShowResult(newEvent.NextValue);
        }

        public void EventCatch(ShowEmojiEvent newEvent)
        {
            var bubble = BubbleViewPools.Get(0);
            _bubbleViews.AddLast(bubble);
            bubble.RelatedTransform = newEvent.Character.transform;
            bubble.WorldOffset = new Vector3(0f, 2f, 0.5f);
            bubble.Icon.sprite = UIView.EmojiSprites[newEvent.SpriteIndex];
            bubble.transform.SetParent(UIView.WorldSpaceTransform);
            bubble.transform.localScale = Vector3.one * 0.1f;
            bubble.transform.DOScale(1f, 0.3f).OnComplete(() => {
                bubble.transform.DOScale(1f, 3f).OnComplete(() => {
                    bubble.transform.DOScale(0.1f, 0.3f).OnComplete(() => {
                        _bubbleViews.Remove(bubble);
                        bubble.Release();
                    });
                });
            });
        }

        public void EventCatch(ShowMessageEvent newEvent)
        {
            var bubble = MessageBubbleViewPools.Get(0);
            _messageBubbleViews.AddLast(bubble);
            if (newEvent.Character.MessageEmitterPivot != null)
            {
                bubble.RelatedTransform = newEvent.Character.MessageEmitterPivot;
                bubble.WorldOffset = new Vector3(0f, 0f, 0f);
            }
            else
            {
                bubble.RelatedTransform = newEvent.Character.transform;
                bubble.WorldOffset = new Vector3(0f, 1.2f, 0f);
            }
            bubble.Init(Camera);
            bubble.SetMessage(newEvent.Message);
            bubble.transform.SetParent(UIView.WorldSpaceTransform);
            bubble.transform.localScale = Vector3.one * 0.1f;
            bubble.transform.DOScale(1f, 0.3f).OnComplete(() => {
                bubble.transform.DOScale(1f, 4f).OnComplete(() => {
                    bubble.transform.DOScale(0.1f, 0.3f).OnComplete(() => {
                        _messageBubbleViews.Remove(bubble);
                        bubble.Release();
                    });
                });
            });
        }

        public void EventCatch(ShowCartoonEvent newEvent)
        {
            var bubble = MessageBubbleViewPools.Get(1);
            _messageBubbleViews.AddLast(bubble);
            if (newEvent.Character.MessageEmitterPivot != null)
            {
                bubble.RelatedTransform = newEvent.Character.MessageEmitterPivot;
                bubble.WorldOffset = new Vector3(0f, 0f, 0f);
            }
            else
            {
                bubble.RelatedTransform = newEvent.Character.transform;
                bubble.WorldOffset = new Vector3(0f, 1.2f, 0f);
            }
            bubble.Init(Camera);
            bubble.SetIcon(UIView.EmojiSprites[newEvent.SpriteIndex]);
            bubble.transform.SetParent(UIView.WorldSpaceTransform);
            bubble.transform.localScale = Vector3.one * 0.1f;
            bubble.transform.DOScale(1f, 0.3f).OnComplete(() => {
                bubble.transform.DOScale(1f, 2f).OnComplete(() => {
                    bubble.transform.DOScale(0.1f, 0.3f).OnComplete(() => {
                        _messageBubbleViews.Remove(bubble);
                        bubble.Release();
                    });
                });
            });
        }

        public void EventCatch(TouchInputEvent currentEvent)
        {
            _touchInputEvents.AddLast(currentEvent);
        }

        public void EventCatch(ShowTutorialVideoEvent currentEvent)
        {
            UIView.ShowTutorial(currentEvent.Index);
        }

        public void EventCatch(HideTutorialVideoEvent currentEvent)
        {
            UIView.HideTutorial();
        }

        public void EventCatch(ShowUIEffectEvent currentEvent)
        {
            if (currentEvent.EffectID == 1)
            {
                UIView.ShowFirstHonorEffect();
            }
        }

        public void EventCatch(NewRankEvent currentEvent)
        {
            UIView.OpenNewRank(currentEvent);
        }

        private void UpdateItemsCount(Unit unit, ItemType itemType)
        {
            if (unit is Character)
            {
                var character = (Character)unit;

                if (character.CharacterType == CharacterType.PLAYER)
                {
                    var itemCount = character.Items.GetAmount(itemType);
                    if (itemType == ItemType.HONOR)
                    {
                        character.GetRank(out var rank, out var currentPoints, out var rankPoints);
                        if (_currentRank != rank)
                        {
                            _currentRank = rank;

                            EventBus.CallEvent(new PlaySoundEvent() { SoundId = 7, IsMusic = false, FadeMusic = true, AttachedTo = character.transform, Position = character.transform.position });

                            // TODO: make visual effect

                            ((ScenarioState) Scenario.BehaviorTreeRunner.InternalState).EventsQueue.AddLast(new NewRankEvent() {});
                        }

                        UIView.SetRank(rank, rank - 1, currentPoints, rankPoints, (float) currentPoints / rankPoints);
                    }
                    else
                    {
                        UIView.SetItemCount(itemType, itemCount);
                    }
                }
            }
            else if (unit is Building)
            {
                var building = (Building) unit;
                if (building.ProduceItemType == ItemType.GOLD && building.Level > 0)
                {
                    var buildingHash = building.GetHashCode();
                    
                    if (!_shopCoinCounters.TryGetValue(buildingHash, out var counter))
                    {
                        counter = PinnedCounterViewPools.Get(0);
                        counter.transform.SetParent(UIView.WorldSpaceTransform);
                        counter.transform.localScale = Vector3.one;
                        counter.transform.localRotation = Quaternion.identity;
                        _shopCoinCounters.TryAdd(buildingHash, counter);
                    }
                    
                    if (counter != null)
                    {
                        counter.Count = building.Items.GetAmount(ItemType.GOLD);
                    }
                }
            }
        }

    }
}
