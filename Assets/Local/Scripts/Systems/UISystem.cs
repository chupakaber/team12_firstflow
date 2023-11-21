using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scripts
{
    internal class UISystem: ISystem
    {
        public List<Character> Characters;
        public List<ProgressBarView> ProgressBarViews;
        public List<Building> Buildings;
        public UIView UIView;
        public Camera Camera;

        public void EventCatch(AddItemEvent newEvent)
        {
            UpdateGoldAndHonor(newEvent.Unit);
        }

        public void EventCatch(RemoveItemEvent newEvent)
        {
            UpdateGoldAndHonor(newEvent.Unit);
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
                var canvasTransform = (RectTransform)UIView.transform;
                var progressBarTransform = (RectTransform)progressBar.transform;
                progressBarTransform.localPosition = canvasTransform.InverseTransformPoint(screenPosition);
            }
        }

        private void UpdateGoldAndHonor(Unit unit)
        {
            if (unit is Character)
            {
                var character = (Character)unit;

                if (character.CharacterType == Enums.CharacterType.PLAYER)
                {
                    var goldCount = character.Items.GetAmount(Enums.ItemType.GOLD);

                    UIView.SetGold(goldCount);

                    var honorCount = character.Items.GetAmount(Enums.ItemType.HONOR);

                    UIView.SetHonor(honorCount);
                }
            }
        }
    }
}
