using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class MathOperationNode : BehaviorCompositeNode, IOutputFloat
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.3f, 0.3f, 0.3f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "MathOperation"; } }

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

        private float _output;
        private float _inputValue1;
        private float _inputValue2;
        private bool _hasValue1;
        private bool _hasValue2;

        public MathOperationNode()
        {
            Input1Type = typeof(float);
            Input2Type = typeof(float);
            Output1Type = typeof(float);
        }

        public float GetOutputFloat(int index = 0)
        {
            return _output;
        }

        public override void Clear()
        {
            base.Clear();
            _output = 0f;
            _hasValue1 = false;
            _hasValue2 = false;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = 0f;
            _hasValue1 = false;
            _hasValue2 = false;
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var inputValue = (parent as IOutputFloat).GetOutputFloat();

            if (inputIndex == 0)
            {
                _hasValue1 = true;
                _inputValue1 = inputValue;
            }
            else if (inputIndex == 1)
            {
                _hasValue2 = true;
                _inputValue2 = inputValue;
            }

            if (!_hasValue1 || !_hasValue2)
            {
                return State.RUNNING;
            }

            switch (MathOperation)
            {
                case MathOperationEnum.APPEND:
                _output = _inputValue1 + _inputValue2;
                break;
                case MathOperationEnum.SUBSTRACT:
                _output = _inputValue1 - _inputValue2;
                break;
                case MathOperationEnum.MULTIPLY:
                _output = _inputValue1 * _inputValue2;
                break;
                case MathOperationEnum.DIVIDE:
                if (Mathf.Abs(_inputValue2) > float.Epsilon)
                {
                    _output = _inputValue1 / _inputValue2;
                }
                else
                {
                    _output = float.MaxValue;
                }
                break;
                case MathOperationEnum.POW:
                _output = Mathf.Pow(_inputValue1, _inputValue2);
                break;
                case MathOperationEnum.MIN:
                _output = Mathf.Min(_inputValue1, _inputValue2);
                break;
                case MathOperationEnum.MAX:
                _output = Mathf.Max(_inputValue1, _inputValue2);
                break;
            }

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