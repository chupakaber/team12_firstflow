using Scripts.Enums;
using TMPro;
using UnityEngine;

namespace Scripts
{
    public class CollectingProgressView : MonoBehaviour
    {
        public GameObject RootGameObject;
        public ItemType ItemType;
        public int Capacity;
        public Unit Storage;
        public Transform Fill;
        public TMP_Text Label;

        public void FillValues()
        {
            if (!RootGameObject.activeSelf)
            {
                return;
            }
            
            var value = Storage.Items.GetAmount(ItemType);
            Fill.localScale = new Vector3(1f, (float) value / Capacity, 1f);
            Fill.localPosition = new Vector3(0f, 0.5f - (float) value / Capacity * 0.5f, 0f);
            Label.text = (Capacity - value).ToString();
        }
    }
}