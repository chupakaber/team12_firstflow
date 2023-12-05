using UnityEngine;

namespace Scripts
{
    public class Customer: SmartCharacter
    {
        [Header("Customer Config")]
        public int Rank = 6;

        [Header("Customer Runtime")]
        public int State;

        public bool IsReadyToBuy()
        {
            return State == 1;
        }
    }
}