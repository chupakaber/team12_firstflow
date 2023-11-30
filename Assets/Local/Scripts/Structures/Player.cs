using UnityEngine;
using System.Collections.Generic;
using Scripts.Enums;

namespace Scripts
{
    public class Player: Character
    {
        [Header("Player Config")]
        public float PlayerSpeedBoostAmount = 2f;

        public override void LevelUp()
        {
            Speed += PlayerSpeedBoostAmount;
        }
    }
}
