using UnityEngine;

namespace Scripts
{
    public class Assistant: Worker
    {
        [Header("Assistant Runtime")]
        public Building ResourceBuilding;
        public float LastAssistantBehaviorTime;
    }
}