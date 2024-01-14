using UnityEngine;

namespace Scripts
{
    public class Assistant: Worker
    {
        [Header("Assistant Config")]
        public int ItemLimitBoostAmount = 5;

        [Header("Assistant Runtime")]
        public Building ResourceBuilding;
        public float LastAssistantBehaviorTime;

        public override void LevelUp()
        {
            base.LevelUp();
            ItemLimit += ItemLimitBoostAmount;
        }
    }
}