using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree Tree;
        public IBehaviorState InternalState;

        public Dictionary<EventCatchNode.EventTypeEnum, Type> _eventTypes = new Dictionary<EventCatchNode.EventTypeEnum, Type>();

        public void Awake()
        {
            _eventTypes.Add(EventCatchNode.EventTypeEnum.Start, typeof(StartEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.Update, typeof(UpdateEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.FixedUpdate, typeof(FixedUpdateEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.AddItem, typeof(AddItemEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.RemoveItem, typeof(RemoveItemEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.ConstructionComplete, typeof(ConstructionCompleteEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.EnterTrigger, typeof(EnterTriggerEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.ExitTrigger, typeof(ExitTriggerEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.MovementInput, typeof(MovementInputEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.RollBagOfTries, typeof(RollBagOfTriesEvent));
            _eventTypes.Add(EventCatchNode.EventTypeEnum.Init, typeof(InitEvent));
        }

        public void EventCatch(IEvent currentEvent)
        {
            foreach (var node in Tree.Nodes)
            {
                if (node is EventCatchNode)
                {
                    var eventNode = (EventCatchNode) node;
                    if (_eventTypes.TryGetValue(eventNode.EventType, out var type))
                    {
                        if (type.Equals(currentEvent.GetType()))
                        {
                            Tree.Run(eventNode, InternalState, currentEvent);
                        }
                    }
                }
            }
        }
    }
}