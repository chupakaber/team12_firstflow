using UnityEngine;

namespace Scripts
{
    public class Apprentice: Worker
    {
        [Header("Apprentice Config")]
        public int TriesCapacityBoostAmount = 4;
        public int ProduceItemPerCicleBust = 1;
        
        [Header("Apprentice Runtime")]
        public float LastApprenticeBehaviorTime;

        public override void LevelUp()
        {
            base.LevelUp();
            if (this.TargetBuilding.ProduceItemType == Enums.ItemType.GOLD)
            {
                this.TargetBuilding.ProductionItemAmountPerCycle += ProduceItemPerCicleBust;
            }
            else
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
}