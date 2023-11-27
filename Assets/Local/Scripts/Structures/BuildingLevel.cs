using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    [System.Serializable]
    public class BuildingLevel
    {
        public GameObject Visual;
        [SerializeField]
        public List<Item> Cost;
    }
}