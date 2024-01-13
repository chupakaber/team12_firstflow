using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class Vector3DifferenceNode : BehaviorCompositeNode, IOutputVector3
    {
        [HideInInspector]
        public override string Section { get { return "Math"; } }

        public bool Normalize = false;

        private bool _hasInput1Value;
        private bool _hasInput2Value;
        private Vector3 _input1Value;
        private Vector3 _input2Value;
        private int _result;
        private Vector3 _output;

        public Vector3DifferenceNode()
        {
            Input1Type = typeof(Vector3);
            Input2Type = typeof(Vector3);
            Output1Type = typeof(Vector3);
        }

        public Vector3 GetOutputVector3()
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _result = 0;
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputVector3)
            {
                var output = (IOutputVector3) parent;
                if (inputIndex == 0)
                {
                    _hasInput1Value = true;
                    _input1Value = output.GetOutputVector3();
                }
                else if (inputIndex == 1)
                {
                    _hasInput2Value = true;
                    _input2Value = output.GetOutputVector3();
                }
                
                if (_hasInput1Value && _hasInput2Value)
                {
                    _output = _input1Value - _input2Value;
                    if (Normalize)
                    {
                        _output.Normalize();
                    }
                    _result = 1;
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
                    var children = Children;

                    foreach (var child in children)
                    {
                        var childState = child.Run(this, inputIndex, internalState, currentEvent);

                        if (childState == State.FAILURE)
                        {
                            return childState;
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
            _hasInput1Value = false;
            _hasInput2Value = false;
        }
    }
}