using Scripts.Enums;

namespace Scripts
{
    public class BuildingProgressBarSystem: ISystem
    {
        public EventBus EventBus;

        public void EventCatch(AddItemEvent newEvent) 
        {
            UpdateProgress(newEvent.Unit, newEvent.ItemType);
        }

        public void EventCatch(RemoveItemEvent newEvent) 
        {
            UpdateProgress(newEvent.Unit, newEvent.ItemType);
        }

        private void UpdateProgress(Unit unit, ItemType itemType)
        {
            foreach (var progressBar in unit.CollectingProgressBars)
            {
                if (progressBar.ItemType == itemType)
                {
                    progressBar.FillValues();
                }
            }
        }
    }
}
