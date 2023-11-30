using UnityEngine;
using Scripts.Enums;
using System.Collections.Generic;

namespace Scripts
{
    public class Unit : MonoBehaviour
    {
        [Header("Unit Config")]
        public ItemStackView ItemStackView;
        public List<CollectingProgressView> CollectingProgressBars = new List<CollectingProgressView>();

        public ItemStack Items = new ItemStack();


        public void AddItem(ItemType type, int count)
        {
            Items.AddItem(type, count);
        }

        public void RemoveItem(ItemType type, int count)
        {            
            Items.RemoveItem(type, count);
        }

        public Vector3 GetStackTopPosition(ItemType itemType = ItemType.NONE)
        {
            return ItemStackView.GetTopPosition(itemType);
        }
    }
}
