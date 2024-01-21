using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class GameObjectToBuildingNode : BehaviorCompositeNode, IOutputBuilding
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Convert"; } }

        private Building _output;

        public GameObjectToBuildingNode()
        {
            Input1Type = typeof(GameObject);
            Output1Type = typeof(Building);
        }

        public Building GetOutputBuilding()
        {
            return _output;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = null;
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = null;
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (parent is IOutputGameObject)
            {
                var gameObject = ((IOutputGameObject) parent).GetOutputGameObject();
                if (gameObject.TryGetComponent<Building>(out var building))
                {
                    _output = building;
                }
            }
            
            if (_output == null)
            {
                return State.FAILURE;
            }

            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var childInputIndex = InputTargetIndex[i];
                child.Run(this, childInputIndex, internalState, currentEvent);
            }

            return State.FAILURE;
        }

        public override void Clear()
        {
            base.Clear();
            _output = null;
        }
    }
}