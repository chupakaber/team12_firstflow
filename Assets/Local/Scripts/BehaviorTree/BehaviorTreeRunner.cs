using System.Collections.Generic;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree Tree;
        public IBehaviorState InternalState;

        public void EventCatch(IEvent currentEvent)
        {
            Tree.Run(InternalState, currentEvent);
        }

        public class RunnerState : IBehaviorState
        {
            public Character Character;
            public List<Building> Buildings;
        }
    }
}