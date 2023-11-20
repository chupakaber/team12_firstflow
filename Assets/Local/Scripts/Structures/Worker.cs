using UnityEngine;

namespace Scripts
{
    public class Worker: SmartCharacter
    {
        [Header("Worker Runtime")]
        public Building SpawnBuilding;
        public Building TargetBuilding;
        public Character TargetCharacter;
    }
}