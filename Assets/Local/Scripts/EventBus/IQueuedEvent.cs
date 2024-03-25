namespace Scripts
{
    public interface IQueuedEvent
    {
        bool Started { get; set; }
        bool Locked { get; set; }
    }
}
