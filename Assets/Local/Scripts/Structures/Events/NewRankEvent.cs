namespace Scripts
{
    public class NewRankEvent: IEvent, IQueuedEvent
    {
        public bool Started { get; set; } = false;
        public bool Locked { get; set; } = true;
    }
}
