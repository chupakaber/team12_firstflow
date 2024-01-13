using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class FindGameObjectNode : BehaviorCompositeNode, IOutputGameObject
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Object"; } }

        public string GameObjectName;

        private GameObject _output;

        public FindGameObjectNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(GameObject);
        }

        public GameObject GetOutputGameObject()
        {
            return _output;
        }

        public override void Clear()
        {
            base.Clear();
            _output = null;
        }

        protected override void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            _output = GameObject.Find(GameObjectName);

            if (_output != null)
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