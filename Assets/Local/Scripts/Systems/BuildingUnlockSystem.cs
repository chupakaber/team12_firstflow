namespace Scripts
{
    public class BuildingUnlockSystem: ISystem
    {
        public EventBus EventBus;
        public UnlockQueue UnlockQueue;

        public void EventCatch(StartEvent newEvent) 
        {
            foreach (var stage in UnlockQueue.Stages)
            {
                stage.UnlockingBuilding.gameObject.SetActive(false);
            }
        }

        public void EventCatch(ConstructionCompleteEvent newEvent) 
        {
            foreach (var stage in UnlockQueue.Stages)
            {
                if (stage.PreviousBuilding == newEvent.Building)
                {
                    stage.UnlockingBuilding.gameObject.SetActive(true);
                }
            }
        }
    }
}
