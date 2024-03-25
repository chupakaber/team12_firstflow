using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class ActivateInteractiveObjectNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override string Section { get { return "Action"; } }

        public ActivateInteractiveObjectNode()
        {
            Input1Type = typeof(GameObject);
            Output1Type = typeof(bool);
        }

        public override void Clear()
        {
            base.Clear();
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputGameObject)
            {
                var gameObject = ((IOutputGameObject) parent).GetOutputGameObject();
                if (gameObject.TryGetComponent<IGameObjectWithState>(out var obj))
                {
                    ((ScenarioState) internalState).EventsQueue.AddLast(new ActivateInteractiveObjectEvent() { TargetObject = obj });

                    for (var i = 0; i < Children.Count; i++)
                    {
                        var child = Children[i];
                        var childInputIndex = InputTargetIndex[i];
                        child.Run(this, childInputIndex, internalState, currentEvent);
                    }

                    return State.SUCCESS;
                }
            }

            return State.RUNNING;
        }
    }
}