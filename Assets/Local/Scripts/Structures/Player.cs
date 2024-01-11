using UnityEngine;

namespace Scripts
{
    public class Player: SmartCharacter
    {
        [Header("Player Config")]
        public float PlayerSpeedBoostAmount = 2f;

        public override void LevelUp()
        {
            Speed += PlayerSpeedBoostAmount;
        }
    }
}
