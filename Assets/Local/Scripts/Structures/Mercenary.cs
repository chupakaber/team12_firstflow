using UnityEngine;

namespace Scripts
{
    public class Mercenary: Worker
    {
        [Header("Mercenary Config")]
        public int ProductionAmount = 1;
        
        [Header("Mercenary Runtime")]
        public int State = 0;
        public float LastMercenaryBehaviorTime;
    }
}
