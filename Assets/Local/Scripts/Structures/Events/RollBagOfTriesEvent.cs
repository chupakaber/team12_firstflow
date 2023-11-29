namespace Scripts
{
    public class RollBagOfTriesEvent: IEvent
    {
        public Character Character;
        public int LastIndex;
        public bool ChangedValue;
        public int NextIndex;
        public bool NextValue;
    }
}
