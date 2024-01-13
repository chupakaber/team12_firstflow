using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class AggregateNode : BehaviorCompositeNode, IOutputFloat
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.5f, 0.2f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Aggregate"; } }

        public enum AggregateTypeEnum {
            TOTAL_WORKERS = 0,
            TOTAL_BUILDING_ITEM_REQUIREMENT = 0
        }

        public AggregateTypeEnum AggregateType;
        public ItemType ItemType;

        private float _output;

        public AggregateNode()
        {
            Input1Type = typeof(bool);
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
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = 0f;
            if (AggregateType == AggregateTypeEnum.TOTAL_WORKERS)
            {
                var totalWorkers = 0;
                foreach (var character in internalState.Characters)
                {
                    if (character.CharacterType == CharacterType.ASSISTANT || character.CharacterType == CharacterType.APPRENTICE)
                    {
                        totalWorkers++;
                    }
                }
                _output = totalWorkers;
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