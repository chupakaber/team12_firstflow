using System.Collections.Generic;

namespace Scripts.BehaviorTree
{
    public interface IBehaviorState
    {
        public EventBus EventBus { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Character> Characters { get; set; }

        public int GetState(int index);
        public float GetTimer(int index);
        public void SetState(int index, int value);
        public void SetTimer(int index, float value);
    }
}