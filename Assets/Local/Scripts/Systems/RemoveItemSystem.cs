namespace Scripts
{
    public class RemoveItemSystem: ISystem
    {
        public void EventCatch(RemoveItemEvent newEvent)
        {
            newEvent.Unit.RemoveItem(newEvent.ItemType, newEvent.Count);

            newEvent.Unit.ItemStackView.RemoveItem(newEvent.ItemType, newEvent.Count);
        }

    }
}
