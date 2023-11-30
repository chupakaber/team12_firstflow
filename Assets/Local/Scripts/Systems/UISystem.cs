using System.Collections.Generic;
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
        }

    }
}
