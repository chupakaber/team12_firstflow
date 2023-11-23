using UnityEngine;
using Scripts.Enums;

namespace Scripts
{
    public class Unit : MonoBehaviour
    {
        [Header("Unit Config")]
        public ItemStackView ItemStackView;

        public ItemStack Items = new ItemStack();

        public void AddItem(ItemType type, int count)
        {
            Items.AddItem(type, count);
        }

        public void RemoveItem(ItemType type, int count)
        {            
            Items.RemoveItem(type, count);
        }
    }
}
