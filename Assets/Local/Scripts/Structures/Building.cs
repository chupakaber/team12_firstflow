using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class Building: MonoBehaviour
    {
        public Collider ProductionArea;
        public Collider CollectResourceArea;
        public float LastProductionTime;
        public bool IsWork;
        public ItemType ProduceItemType;
        public ItemType ConsumeItemType;
    }
}
