using System.Collections.Generic;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class CompareFloatNode : BehaviorCompositeNode
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

        private bool _hasInput1Value;
        private bool _hasInput2Value;
        private float _input1Value;
        private float _input2Value;
        private int _result;

        public CompareFloatNode()
        {
            Input1Type = typeof(float);
            Input2Type = typeof(float);
            Output1Type = typeof(bool);
            Output2Type = typeof(bool);
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _result = 0;
            _hasInput1Value = false;
            _hasInput2Value = false;
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _result = 0;
            _hasInput1Value = false;
            _hasInput2Value = false;
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputFloat)
            {
                var output = (IOutputFloat) parent;
                if (inputIndex == 0)
                {
                    _hasInput1Value = true;
                    _input1Value = output.GetOutputFloat();
                }
                else if (inputIndex == 1)
                {
                    _hasInput2Value = true;
                    _input2Value = output.GetOutputFloat();
                }
                
                if (_hasInput1Value && _hasInput2Value)
                {
                    _result = Compare(_input1Value, _input2Value) ? 1 : 2;
                }
                else
                {
                    _result = 3;
                }
            }
            
            if (_result > 0)
            {
                if (_result < 3)
                {
                    for (var i = 0; i < Children.Count; i++)
                    {
                        var outputPortIndex = OutputIndex[i];
                        if (outputPortIndex == _result - 1)
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
                else
                {
                    return State.RUNNING;
                }
            }

            return State.FAILURE;
        }

        public override void Clear()
        {
            base.Clear();
            _result = 0;
            _hasInput1Value = false;
            _hasInput2Value = false;
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