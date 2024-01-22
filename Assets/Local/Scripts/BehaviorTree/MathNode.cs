using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class MathValueNode : BehaviorCompositeNode, IOutputFloat
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.3f, 0.3f, 0.3f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Math"; } }

        public enum MathOperationEnum {
            APPEND = 0,
            SUBSTRACT = 1,
            MULTIPLY = 2,
            DIVIDE = 3,
            POW = 4,
            MIN = 5,
            MAX = 6,
        }

        public MathOperationEnum MathOperation;

        public float Value;

        private float _output;

        public MathValueNode()
        {
            Input1Type = typeof(float);
            Output1Type = typeof(float);
        }

        public float GetOutputFloat(int index = 0)
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = 0f;

            var inputValue = (parent as IOutputFloat).GetOutputFloat();

            switch (MathOperation)
            {
                case MathOperationEnum.APPEND:
                _output = inputValue + Value;
                break;
                case MathOperationEnum.SUBSTRACT:
                _output = inputValue - Value;
                break;
                case MathOperationEnum.MULTIPLY:
                _output = inputValue * Value;
                break;
                case MathOperationEnum.DIVIDE:
                if (Mathf.Abs(Value) > float.Epsilon)
                {
                    _output = inputValue / Value;
                }
                else
                {
                    _output = float.MaxValue;
                }
                break;
                case MathOperationEnum.POW:
                _output = Mathf.Pow(inputValue, Value);
                break;
                case MathOperationEnum.MIN:
                _output = Mathf.Min(inputValue, Value);
                break;
                case MathOperationEnum.MAX:
                _output = Mathf.Max(inputValue, Value);
                break;
            }
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var childInputIndex = InputTargetIndex[i];
                child.Run(this, childInputIndex, internalState, currentEvent);
            }

            return State.SUCCESS;
        }
    }
}