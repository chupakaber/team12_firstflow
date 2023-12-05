using System;
using System.Collections.Generic;

namespace Scripts.BehaviorTree
{
    [Serializable]
    public class SmartCharacterState : IBehaviorState
    {
        public EventBus EventBus;
        public SmartCharacter Self;
        public List<Character> Characters;
        public List<Building> Buildings;
        public float[] Timers = new float[32];
        public int[] States = new int[32];
    }
}