using Scripts.Enums;
using UnityEngine;

namespace Scripts
{
    public class Building: MonoBehaviour
    {
        public Collider ProductionArea;
        public float LastProductionTime;
        public bool IsWork;
        public ItemType ItemType;
    }
}
