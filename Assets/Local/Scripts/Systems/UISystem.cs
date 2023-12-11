using System.Collections.Generic;
using DG.Tweening;
using Scripts.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    internal class UISystem: ISystem
    {
        public List<Character> Characters;
        public List<ProgressBarView> ProgressBarViews;
        public List<Building> Buildings;
        public EventBus EventBus;
        public UIView UIView;
        public Camera Camera;
        public PoolCollection<BagOfTriesView> BagOfTriesViewPools;
        public PoolCollection<PinnedCounterView> PinnedCounterViewPools;
        public PoolCollection<BubbleView> BubbleViewPools;
        public PoolCollection<MessageBubbleView> MessageBubbleViewPools;

        private Dictionary<int, PinnedCounterView> _shopCoinCounters = new Dictionary<int, PinnedCounterView>();
        private LinkedList<BubbleView> _bubbleViews = new LinkedList<BubbleView>();
        private LinkedList<MessageBubbleView> _messageBubbleViews = new LinkedList<MessageBubbleView>();
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
        }

        public void EventCatch(AddItemEvent newEvent)
        {
            UpdateItemsCount(newEvent.Unit, newEvent.ItemType);
        }

        public void EventCatch(RemoveItemEvent newEvent)
        {
            UpdateItemsCount(newEvent.Unit, newEvent.ItemType);
        }

        public void EventCatch(UpdateEvent newEvent)
        {
            for (var i = 0; i< ProgressBarViews.Count; i++)
            {
                var building = Buildings[i];
                var progressBar = ProgressBarViews[i];

                progressBar.Progress = building.ProductionProgress();

                var worldPosition = building.ProgressBarPivot.position;
                var screenPosition = Camera.WorldToScreenPoint(worldPosition);
                var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                var progressBarTransform = (RectTransform)progressBar.transform;
                progressBarTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
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

            if (UIView.PointerArrowTargetPosition.sqrMagnitude <= float.Epsilon || (player.transform.position - UIView.PointerArrowTargetPosition).magnitude < 3f)
            {
                if (UIView.PointerArrowTransform.gameObject.activeSelf)
                {
                    UIView.PointerArrowTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!UIView.PointerArrowTransform.gameObject.activeSelf)
                {
                    UIView.PointerArrowTransform.gameObject.SetActive(true);
                }

                var direction = Vector3.forward;
                var worldPosition = UIView.PointerArrowTargetPosition;

                var minimalMagnitude = (worldPosition - player.transform.position).magnitude;

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
                    if (cornersCount > 2)
                    {
                        var d1 = (_pathCorners[1] - _pathCorners[0]).magnitude;
                        var d2 = (_pathCorners[2] - _pathCorners[0]).magnitude;
                        var d = Mathf.Clamp((d2 - d1) / d1, 0f, 1f);
                        var w1 = d;
                        var w2 = 1f - d;
                        worldPosition = _pathCorners[1] * w1 + _pathCorners[2] * w2;
                    }
                }
                player.NavMeshAgent.enabled = false;

                direction = worldPosition - player.transform.position;
                var distance = Mathf.Min(Mathf.Max(minimalMagnitude, direction.magnitude), 2.5f);
                worldPosition = player.transform.position + direction.normalized * distance;

                _targetArrowWorldPosition = Vector3.Lerp(_targetArrowWorldPosition, worldPosition, Time.deltaTime * 3f);
                var screenPosition = Camera.WorldToScreenPoint(_targetArrowWorldPosition);
                var canvasTransform = (RectTransform)UIView.WorldSpaceTransform.transform;
                UIView.PointerArrowTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
                UIView.PointerArrowTransform.localRotation = Quaternion.Euler(0f, 0f, -Quaternion.LookRotation(direction).eulerAngles.y - 45f);
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

        public void EventCatch(RollBagOfTriesEvent newEvent)
        {
            var character = newEvent.Character;

            if (character.BagOfTriesView == null)
            {
                character.BagOfTriesView = BagOfTriesViewPools.Get(0);
                
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

        public void EventCatch(TouchInputEvent currentEvent)
        {
            _touchInputEvents.AddLast(currentEvent);
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
