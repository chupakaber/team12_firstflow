using System.Collections.Generic;

namespace Scripts.BehaviorTree
{
    public interface IBehaviorState
    {
        public EventBus EventBus { get; set; }
        public List<Building> Buildings { get; set; }
        public List<Character> Characters { get; set; }
    }
}