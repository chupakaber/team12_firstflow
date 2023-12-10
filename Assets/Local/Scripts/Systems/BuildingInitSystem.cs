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
        
        private ProgressBarView _progressBarPrefab;

        public void EventCatch(InitEvent newEvent)
        {
            var buildingsArray = Object.FindObjectsOfType<Building>();
            _progressBarPrefab = Resources.Load<ProgressBarView>("Prefabs/UI/ProgressBar");

            foreach (var building in buildingsArray)
            {
                Buildings.Add(building);
                building.Level = building.Level;
            }
        }

        public void EventCatch(StartEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                var progressBar = Object.Instantiate(_progressBarPrefab);
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
