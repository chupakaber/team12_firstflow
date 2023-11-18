namespace Scripts
{
    public class RemoveItemSystem: ISystem
    {
        public void EventCatch(RemoveItemEvent newEvent)
        {
            newEvent.Character.RemoveItem(newEvent.ItemType, newEvent.Count);

            newEvent.Character.ItemStackView.RemoveItem(newEvent.ItemType);
        }

    }
}
