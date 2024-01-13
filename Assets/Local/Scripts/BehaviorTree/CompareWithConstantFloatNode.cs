using System.Collections.Generic;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class CompareWithConstantFloatNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.2f, 0.3f, 0.5f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Math"; } }

        public override string Output1Name { get { return "True"; } }
        public override string Output2Name { get { return "False"; } }

        public enum CompareOperator {
            Equal = 0,
            Greater = 1,
            Less = 2
        }

        [HideInInspector]
        public List<BehaviorNode> ChildrenForFailure = new List<BehaviorNode>();

        public CompareOperator Operator;
        public float Value = 0f;

        public CompareWithConstantFloatNode()
        {
            Input1Type = typeof(float);
            Output1Type = typeof(bool);
            Output2Type = typeof(bool);
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputFloat)
            {
                var output = (IOutputFloat) parent;
                var value = output.GetOutputFloat();
                var result = Compare(value, Value);

                for (var i = 0; i < Children.Count; i++)
                {
                    var outputPortIndex = OutputIndex[i];
                    if (outputPortIndex == (result ? 0 : 1))
                    {
                        var child = Children[i];
                        var targetPortIndex = InputTargetIndex[i];
                        var childState = child.Run(this, targetPortIndex, internalState, currentEvent);

                        if (childState == State.FAILURE)
                        {
                            return childState;
                        }
                    }
                }

                return State.SUCCESS;
            }
            
            return State.FAILURE;
        }

        public override void Clear()
        {
            base.Clear();
        }

        private bool Compare(float value1, float value2)
        {
            if (Operator == CompareOperator.Greater)
            {
                return value1 - value2 > float.Epsilon;
            }
            else if (Operator == CompareOperator.Less)
            {
                return value2 - value1 > float.Epsilon;
            }
            return Mathf.Abs(value1 - value2) <= float.Epsilon;
        }
    }
}