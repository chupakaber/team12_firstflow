using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class DebugLogNode : BehaviorCompositeNode
    {
        public string Message;

        public DebugLogNode()
        {
            Input1Type = typeof(bool);
            Input2Type = typeof(float);
            Input3Type = typeof(Vector3);
            Input4Type = typeof(SmartCharacter);
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (inputIndex == 0)
            {
                Debug.Log($"{Message}");
            }
            else if (inputIndex == 1 && parent is IOutputFloat)
            {
                var parentNode = (IOutputFloat) parent;
                var value = parentNode.GetOutputFloat();
                Debug.Log($"{Message} | Value: {value}");
            }
            else if (inputIndex == 2 && parent is IOutputVector3)
            {
                var parentNode = (IOutputVector3) parent;
                var value = parentNode.GetOutputVector3();
                Debug.Log($"{Message} | Value: {value}");
            }
            else if (inputIndex == 3 && parent is IOutputCharacter)
            {
                var parentNode = (IOutputCharacter) parent;
                var value = parentNode.GetOutputCharacter();
                Debug.Log($"{Message} | Value: {value.name}");
            }
            else
            {
                return State.SUCCESS;
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