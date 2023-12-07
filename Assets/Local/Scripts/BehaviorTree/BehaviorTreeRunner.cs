using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree Tree;
        public IBehaviorState InternalState;

        public void EventCatch(IEvent currentEvent)
        {
            foreach (var node in Tree.Nodes)
            {
                if (node is EventCatchNode)
                {
                    var eventNode = (EventCatchNode) node;
                    if ($"{eventNode.EventType}Event" == currentEvent.GetType().Name)
                    {
                        Tree.RootNode = eventNode;
                        Tree.Run(InternalState, currentEvent);
                    }
                }
            }
        }
    }
}