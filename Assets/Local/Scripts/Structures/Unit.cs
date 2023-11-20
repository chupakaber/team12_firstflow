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

            foreach (var item in Items)
            {
                Debug.Log($"{gameObject.name}{gameObject.GetHashCode()} Кол-во {item.Type} равно {item.Amount}");
            }
        }

        public void RemoveItem(ItemType type, int count)
        {            
            Items.RemoveItem(type, count);

            foreach (var item in Items)
            {
                Debug.Log($"{gameObject.name}{gameObject.GetHashCode()} Кол-во {item.Type} равно {item.Amount}");
            }            
        }
    }
}
