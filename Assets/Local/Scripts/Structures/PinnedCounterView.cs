using TMPro;
using UnityEngine;

namespace Scripts
{
    public class PinnedCounterView: PoolableObject
    {
        [SerializeField] private TMP_Text _label;

        public int Count 
        { 
            set
            {
                _label.text = value.ToString();
            } 
        }
    }
}
