using System.Collections.Generic;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public abstract class BehaviorCompositeNode : BehaviorNode
    {
        [HideInInspector]
        public List<BehaviorNode> Children = new List<BehaviorNode>();
        [HideInInspector]
        public List<int> InputTargetIndex = new List<int>();
        [HideInInspector]
        public List<int> OutputIndex = new List<int>();

        public override void Clear()
        {
            base.Clear();
            foreach(var child in Children)
            {
                child.Clear();
            }
        }
    }
}