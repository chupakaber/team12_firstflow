using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class BuildingInitSystem: ISystem
    {
        public EventBus EventBus;
        public UIView UIView;

        public List<Building> Buildings;
        public List<ProgressBarView> ProgressBarViews;

        public void EventCatch(StartEvent newEvent)
        {
            var buildingsArray = Object.FindObjectsOfType<Building>();
            var progressBarPrefab = Resources.Load<ProgressBarView>("Prefabs/UI/ProgressBar");

            foreach (Building building in buildingsArray)
            {
                building.Level = building.Level;
                Buildings.Add(building);

                var progressBar = Object.Instantiate(progressBarPrefab);
                progressBar.transform.SetParent(UIView.WorldSpaceTransform);
                progressBar.transform.localScale = Vector3.one;
                ProgressBarViews.Add(progressBar);

                UpdateProgress(building);
            }
        }

        private void UpdateProgress(Building building)
        {
            if (building.Level + 1 < building.Levels.Count)
            {
                var levelConfig = building.Levels[building.Level + 1];
                foreach (var progressBar in building.CollectingProgressBars)
                {
                    foreach (var requirement in levelConfig.Cost)
                    {
                        if (requirement.Type == progressBar.ItemType)
                        {
                            progressBar.Capacity = requirement.Amount;
                            progressBar.FillValues();
                        }
                    }
                }
            }
        }
    }
}
