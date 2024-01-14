using DG.Tweening;
using Scripts.Enums;
using UnityEngine;

namespace Scripts.Systems
{
    public class AddItemSystem: ISystem
    {
        public EventBus EventBus;
        public PoolCollection<ItemView> ItemViewPools;
        public PoolCollection<IconView> IconViewPools;
        public UIView UIView;

        private float _lastSoundTime;

        public void EventCatch(AddItemEvent newEvent)
        {
            newEvent.Unit.AddItem(newEvent.ItemType, newEvent.Count);
            
            for (var i = 0; i < newEvent.Count; i++)
            {
                var itemView = ItemViewPools.Get((int) newEvent.ItemType);
                if (itemView != null)
                {
                    //if (newEvent.Unit is Character)
                    //{
                    if (CheckLastSoundTime())
                    {
                        var soundId = -1;
                        switch (newEvent.ItemType)
                        {
                            case ItemType.WOOD:
                            case ItemType.POWDER:
                            case ItemType.SPEAR:
                                soundId = 5;
                            break;
                            case ItemType.IRON:
                            case ItemType.SWORD:
                            case ItemType.GUN:
                                soundId = 4;
                            break;
                            case ItemType.ROCK:
                                soundId = 18;
                            break;
                            case ItemType.BOTTLE_HERO:
                            case ItemType.BOTTLE_WORKER:
                                soundId = 1;
                            break;
                        }

                        if (soundId > -1)
                        {
                            EventBus.CallEvent(new PlaySoundEvent() { SoundId = soundId, IsMusic = false, AttachedTo = newEvent.Unit.transform });
                        }
                    }
                    //}

                    newEvent.Unit.ItemStackView.AddItem(itemView);
                    var endPosition = newEvent.Unit.GetStackTopPosition(newEvent.ItemType);
                    if (newEvent.FromPosition.sqrMagnitude > float.Epsilon)
                    {
                        itemView.gameObject.SetActive(false);
                        var itemViewForFly = ItemViewPools.Get((int) newEvent.ItemType);
                        itemViewForFly.transform.position = newEvent.FromPosition;
                        itemViewForFly.transform.DOJump(endPosition, 1f, 1, 0.5f).OnComplete(() => {
                            itemViewForFly.Release();
                            itemView.gameObject.SetActive(true);
                            newEvent.Unit.ItemStackView.SortItems();
                        });
                    }
                }
                else
                {
                    if (newEvent.ItemType == ItemType.GOLD)
                    {
                        if (newEvent.Unit is Character && ((Character) newEvent.Unit).CharacterType == CharacterType.PLAYER)
                        {
                            if (CheckLastSoundTime())
                            {
                                EventBus.CallEvent(new PlaySoundEvent() { SoundId = 9, IsMusic = false, AttachedTo = newEvent.Unit.transform });
                            }

                            var icon = IconViewPools.Get(0);
                            UIView.FlyCoin(icon, true);
                        }
                        else if (newEvent.Unit is Building)
                        {
                            newEvent.Unit.ItemStackView.ToggleExclusiveItemStack(newEvent.ItemType, true);
                        }
                    }
                    else if ((newEvent.ItemType == ItemType.BOTTLE_HERO || newEvent.ItemType == ItemType.BOTTLE_WORKER) && newEvent.Unit is SmartCharacter)
                    {
                        EventBus.CallEvent(new PlaySoundEvent() { SoundId = 1, IsMusic = false, AttachedTo = newEvent.Unit.transform });

                        var smartCharacter = (SmartCharacter) newEvent.Unit;
                        if (smartCharacter.LevelDecor != null)
                        {
                            var bottlesAmount = smartCharacter.Items.GetAmount(newEvent.ItemType);
                            for (var j = 0; j < smartCharacter.LevelDecor.Count; j++)
                            {
                                smartCharacter.LevelDecor[j].SetActive(j == bottlesAmount);
                            }
                        }
                    }
                }
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
