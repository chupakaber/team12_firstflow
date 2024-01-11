using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class RemoveItemSystem: ISystem
    {
        public EventBus EventBus;
        public UIView UIView;
        public PoolCollection<IconView> IconViewPools;

        private float _lastSoundTime;

        public void EventCatch(RemoveItemEvent newEvent)
        {
            newEvent.Unit.RemoveItem(newEvent.ItemType, newEvent.Count);

            if (newEvent.ItemType == ItemType.GOLD)
            {
                if (newEvent.Unit is Character && ((Character) newEvent.Unit).CharacterType == CharacterType.PLAYER)
                {
                    if (CheckLastSoundTime())
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 9, IsMusic = false, AttachedTo = newEvent.Unit.transform });
                    }

                    var icon = IconViewPools.Get(0);
                    UIView.FlyCoin(icon, false);
                }
                else if (newEvent.Unit is Building)
                {
                    newEvent.Unit.ItemStackView.ToggleExclusiveItemStack(newEvent.ItemType, newEvent.Unit.Items.GetAmount(newEvent.ItemType) > 0);
                }
            }
            else
            {
                newEvent.Unit.ItemStackView.RemoveItem(newEvent.ItemType, newEvent.Count);
            }
        }

        private bool CheckLastSoundTime()
        {
            if (Time.time > _lastSoundTime + 0.06f)
            {
                _lastSoundTime = Time.time;

                return true;
            }

            return false;
        }
    }
}
