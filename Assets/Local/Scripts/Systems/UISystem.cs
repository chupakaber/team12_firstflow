using System.Collections.Generic;
using DG.Tweening;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    internal class UISystem: ISystem
    {
        public List<Character> Characters;
        public List<ProgressBarView> ProgressBarViews;
        public List<Building> Buildings;
        public UIView UIView;
        public Camera Camera;
        public PoolCollection<BagOfTriesView> BagOfTriesViewPools;
        public PoolCollection<PinnedCounterView> PinnedCounterViewPools;
        public PoolCollection<BubbleView> BubbleViewPools;

        private Dictionary<int, PinnedCounterView> _shopCoinCounters = new Dictionary<int, PinnedCounterView>();
        private LinkedList<BubbleView> _bubbleViews = new LinkedList<BubbleView>();

        public void EventCatch(StartEvent newEvent)
        {
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
                    var worldPosition = character.transform.position + Vector3.up * 2.5f;
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
                        if (worldPosition.sqrMagnitude > float.Epsilon)
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

            if (UIView.PointerArrowTargetPosition.sqrMagnitude <= float.Epsilon)
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
                foreach (var character in Characters)
                {
                    if (character.CharacterType == CharacterType.PLAYER)
                    {
                        direction = worldPosition - character.transform.position;
                        var distance = Mathf.Min(direction.magnitude, 2.5f);
                        worldPosition = character.transform.position + direction.normalized * distance;
                    }
                }
                var screenPosition = Camera.WorldToScreenPoint(worldPosition);
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

        private void UpdateItemsCount(Unit unit, ItemType itemType)
        {
            if (unit is Character)
            {
                var character = (Character)unit;

                if (character.CharacterType == CharacterType.PLAYER)
                {
                    var itemCount = character.Items.GetAmount(itemType);
                    UIView.SetItemCount(itemType, itemCount);
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
