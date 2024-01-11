namespace Scripts
{
    public class ObjectActivationSystem : ISystem
    {
        public EventBus EventBus;

        public void EventCatch(ActivateObjectEvent newEvent)
        {
            newEvent.Target.Activate();
        }

        public void EventCatch(DeactivateObjectEvent newEvent)
        {
            newEvent.Target.Deactivate();
        }
    }
}