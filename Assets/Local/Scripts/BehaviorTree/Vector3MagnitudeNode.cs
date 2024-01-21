using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class Vector3MagnitudeNode : BehaviorCompositeNode, IOutputFloat
    {
        [HideInInspector]
        public override string Section { get { return "Math"; } }

        private Vector3 _input1Value;
        private float _output;

        public Vector3MagnitudeNode()
        {
            Input1Type = typeof(Vector3);
            Output1Type = typeof(float);
        }

        public float GetOutputFloat(int index = 0)
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputVector3)
            {
                var output = (IOutputVector3) parent;
                _input1Value = output.GetOutputVector3();
                _output = _input1Value.magnitude;

                var childIndex = 0;
                foreach (var child in Children)
                {
                    child.Run(this, InputTargetIndex[childIndex], internalState, currentEvent);
                    childIndex++;
                }
                
                return State.SUCCESS;
            }
            
            return State.FAILURE;
        }
    }
}