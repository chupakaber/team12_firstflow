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
