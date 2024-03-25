namespace Scripts
{
    public class ActivateInteractiveObjectEvent: IEvent, IQueuedEvent
    {
        public bool Started { get; set; } = false;
        public bool Locked { get; set; } = true;
        public IGameObjectWithState TargetObject;
    }
}
