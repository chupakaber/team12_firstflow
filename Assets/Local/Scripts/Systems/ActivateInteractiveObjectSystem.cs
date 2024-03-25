namespace Scripts
{
    public class ActivateInteractiveObjectSystem : ISystem
    {
        public EventBus EventBus;
        public Scenario Scenario;

        public void EventCatch(ActivateInteractiveObjectEvent newEvent)
        {
            newEvent.TargetObject.OnActivate(newEvent);
            EventBus.CallEvent(new SaveEvent());
        }
    }
}
