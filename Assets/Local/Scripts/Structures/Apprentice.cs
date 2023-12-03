using UnityEngine;

namespace Scripts
{
    public class Apprentice: Worker
    {
        [Header("Apprentice Config")]
        public int TriesCapacityBoostAmount = 4;
        
        [Header("Apprentice Runtime")]
        public float LastApprenticeBehaviorTime;

        public override void LevelUp()
        {
            BaseBagOfTriesCapacity += TriesCapacityBoostAmount;
            BagOfTries.Resize(BaseBagOfTriesCapacity);
            if (BagOfTriesView != null)
            {
                BagOfTriesView.Resize(BaseBagOfTriesCapacity);
            }
        }
    }
}