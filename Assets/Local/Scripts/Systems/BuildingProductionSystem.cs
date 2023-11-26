﻿using System.Collections.Generic;
using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class BuildingProductionSystem: ISystem
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
            }
        }

        public void EventCatch(EnterTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    if (building.ProductionCharacters.Count == 1)
                    {
                        if (building.ProductionEndTime > building.LastProductionTime)
                        {
                            building.LastProductionTime = Time.time - (building.ProductionEndTime - building.LastProductionTime);
                            building.ProductionEndTime = building.LastProductionTime - 1f;
                        }
                        else
                        {
                            building.LastProductionTime = Time.time;
                        }
                    }
                }
            }
        }

        public void EventCatch(ExitTriggerEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (newEvent.Trigger.Equals(building.ProductionArea))
                {
                    if (building.ProductionCharacters.Count == 0)
                    {
                        building.ProductionEndTime = Time.time;
                    }
                }
            }
        }

        public void EventCatch(FixedUpdateEvent newEvent)
        {
            foreach (var building in Buildings)
            {
                if (building.Level < 1)
                {
                    continue;
                }

                if (Time.time < building.LastProductionTime + building.ProductionCooldown)
                {
                    continue;
                }

                if(building.ProductionArea != null && building.ProductionCharacters.Count < 1)
                {
                    continue;
                }

                if (building.ItemCost > building.Items.GetAmount(building.ConsumeItemType))
                {
                    continue;
                }

                if(building.ProductionLimit >= building.Items.GetAmount(building.ProduceItemType) + building.ProductionItemAmountPerCycle)
                {
                    var addItemEvent = new AddItemEvent() { ItemType = building.ProduceItemType, Count = building.ProductionItemAmountPerCycle, Unit = building };
                    EventBus.CallEvent(addItemEvent);

                    if (building.ItemCost > 0)
                    {
                        var removeItemEvent = new RemoveItemEvent() { ItemType = building.ConsumeItemType, Count = building.ItemCost, Unit = building };
                        EventBus.CallEvent(removeItemEvent);
                    }

                    building.LastProductionTime = Time.time;
                }
            }
        }
    }
}
