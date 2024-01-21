using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class EventCatchNode : BehaviorCompositeNode, IOutputFloat, IOutputVector3, IOutputUnit, IOutputGameObject
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.4f, 0f, 0.1f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Event"; } }

        public enum EventTypeEnum
        {
            Start = 0,
            Update = 1,
            FixedUpdate = 2,
            AddItem = 3,
            RemoveItem = 4,
            ConstructionComplete = 5,
            EnterTrigger = 6,
            ExitTrigger = 7,
            MovementInput = 8,
            RollBagOfTries = 9,
            Init = 10,
            Assignment = 11,
        }

        public EventTypeEnum EventType = EventTypeEnum.Start;

        private float _output2;
        private float _output3;
        private Vector3 _output4;
        private Unit _output5;
        private GameObject _output6;

        public EventCatchNode()
        {
            Output1Type = typeof(bool);
            Output2Type = typeof(float);
            Output3Type = typeof(float);
            Output4Type = typeof(Vector3);
            Output5Type = typeof(Unit);
            Output6Type = typeof(GameObject);
        }

        public float GetOutputFloat(int index = 0)
        {
            return index == 1 ? _output3 : _output2;
        }
        
        public Vector3 GetOutputVector3()
        {
            return _output4;
        }

        public Unit GetOutputUnit()
        {
            return _output5;
        }

        public GameObject GetOutputGameObject()
        {
            return _output6;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var success = false;

            switch(EventType)
            {
                case EventTypeEnum.Start:
                    if (currentEvent is StartEvent)
                    {
                        success = true;
                    }
                    break;
                case EventTypeEnum.Update:
                    if (currentEvent is UpdateEvent)
                    {
                        success = true;
                    }
                    break;
                case EventTypeEnum.FixedUpdate:
                    if (currentEvent is FixedUpdateEvent)
                    {
                        success = true;
                    }
                    break;
                case EventTypeEnum.ConstructionComplete:
                    if (currentEvent is ConstructionCompleteEvent)
                    {
                        var constructionCompleteEvent = (ConstructionCompleteEvent) currentEvent;
                        _output5 = constructionCompleteEvent.Building;
                        success = true;
                    }
                    break;
                case EventTypeEnum.AddItem:
                    if (currentEvent is AddItemEvent)
                    {
                        var addItemEvent = (AddItemEvent) currentEvent;
                        _output2 = (float) addItemEvent.ItemType;
                        _output3 = addItemEvent.Count;
                        _output4 = addItemEvent.FromPosition;
                        _output5 = addItemEvent.Unit;
                        success = true;
                    }
                    break;
                case EventTypeEnum.RemoveItem:
                    if (currentEvent is RemoveItemEvent)
                    {
                        var removeItemEvent = (RemoveItemEvent) currentEvent;
                        _output2 = (float) removeItemEvent.ItemType;
                        _output3 = removeItemEvent.Count;
                        _output5 = removeItemEvent.Unit;
                        success = true;
                    }
                    break;
                case EventTypeEnum.Init:
                    if (currentEvent is InitEvent)
                    {
                        success = true;
                    }
                    break;
                case EventTypeEnum.EnterTrigger:
                    if (currentEvent is EnterTriggerEvent)
                    {
                        var enterTriggerEvent = (EnterTriggerEvent) currentEvent;
                        _output2 = (float) enterTriggerEvent.Character.CharacterType;
                        _output5 = enterTriggerEvent.Character;
                        _output6 = enterTriggerEvent.Trigger.gameObject;
                        success = true;
                    }
                    break;
                case EventTypeEnum.ExitTrigger:
                    if (currentEvent is ExitTriggerEvent)
                    {
                        var exitTriggerEvent = (ExitTriggerEvent) currentEvent;
                        _output2 = (float) exitTriggerEvent.Character.CharacterType;
                        _output5 = exitTriggerEvent.Character;
                        _output6 = exitTriggerEvent.Trigger.gameObject;
                        success = true;
                    }
                    break;
                case EventTypeEnum.Assignment:
                    if (currentEvent is AssignmentEvent)
                    {
                        var assignmentEvent = (AssignmentEvent) currentEvent;
                        _output5 = assignmentEvent.Character;
                        _output6 = assignmentEvent.Building.gameObject;
                        success = true;
                    }
                    break;
            }

            if (success)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var child = Children[i];
                    var childInputIndex = InputTargetIndex[i];
                    child.Run(this, childInputIndex, internalState, currentEvent);
                }
            }
            
            return State.SUCCESS;
        }
    }
}