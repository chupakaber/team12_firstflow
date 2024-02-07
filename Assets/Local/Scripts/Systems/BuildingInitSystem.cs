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
        public List<TimerBarView> TimerBarViews;
        
        private ProgressBarView _progressBarPrefab;
        private TimerBarView _timerBarPrefab;

        public void EventCatch(InitEvent newEvent)
        {
            var buildingsArray = Object.FindObjectsOfType<Building>();
            _progressBarPrefab = Resources.Load<ProgressBarView>("Prefabs/UI/ProgressBar");
            _timerBarPrefab = Resources.Load<TimerBarView>("Prefabs/UI/TimerBar");

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

                if (building.ProduceMethod == Building.ProductionMethod.RESOURCE_TO_TIME)
                {
                    var timerBar = Object.Instantiate(_timerBarPrefab);
                    timerBar.Building = building;
                    timerBar.transform.SetParent(UIView.WorldSpaceTransform);
                    timerBar.transform.localScale = Vector3.one;
                    TimerBarViews.Add(timerBar);
                }

                building.Initialized = true;
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
