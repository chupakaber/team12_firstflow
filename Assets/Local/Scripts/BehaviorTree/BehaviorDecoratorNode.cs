using UnityEngine;

namespace Scripts.BehaviorTree
{
    public abstract class BehaviorDecoratorNode : BehaviorNode
    {
        [HideInInspector]
        public BehaviorNode Child;
        [HideInInspector]
        public int InputTargetIndex;

        public override void Clear()
        {
            base.Clear();
            if (Child != null)
            {
                Child.Clear();
            }
        }
    }
}