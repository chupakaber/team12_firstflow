using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class CarnivalActionNode : BehaviorCompositeNode
    {
        [HideInInspector]
        public override string Section { get { return "Action"; } }

        public enum ActionEnum
        {
            WALK_BY_PATH = 0,
            MOVE_TO_SPAWN_POINT = 1,
        }

        public ActionEnum ActionType;

        public CarnivalActionNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(bool);
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {

        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            var characters = internalState.Characters;
            foreach (var character in characters)
            {
                if (character.CharacterType == CharacterType.CARNIVAL)
                {
                    var carnival = (Carnival) character;
                    if (carnival.State != 1)
                    {
                        switch (ActionType)
                        {
                            case ActionEnum.WALK_BY_PATH:
                                carnival.State = 0;
                            break;
                            case ActionEnum.MOVE_TO_SPAWN_POINT:
                                carnival.State = 2;
                            break;
                        }
                    }
                }
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