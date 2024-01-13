using Scripts.Enums;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class RandomCharacterNode : BehaviorCompositeNode, IOutputCharacter
    {
        [HideInInspector]
        public override Color DefaultColor { get { return new Color(0.35f, 0.5f, 0.2f, 1f); } }
        [HideInInspector]
        public override string Section { get { return "Object"; } }

        public CharacterType CharacterType = CharacterType.NONE;

        private SmartCharacter _output;

        public RandomCharacterNode()
        {
            Input1Type = typeof(bool);
            Output1Type = typeof(SmartCharacter);
        }

        public SmartCharacter GetOutputCharacter()
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
            var matchCount = 0;
            foreach (var character in internalState.Characters)
            {
                if (character is SmartCharacter)
                {
                    var smartCharacter = (SmartCharacter) character;
                    if (CharacterType == CharacterType.NONE || smartCharacter.CharacterType == CharacterType)
                    {
                        matchCount++;
                    }
                }
            }
            var randomIndex = Random.Range(0, matchCount);
            var i = 0;
            foreach (var character in internalState.Characters)
            {
                if (character is SmartCharacter)
                {
                    var smartCharacter = (SmartCharacter) character;
                    if (CharacterType == CharacterType.NONE || smartCharacter.CharacterType == CharacterType)
                    {
                        if (i >= randomIndex)
                        {
                            _output = (SmartCharacter) character;
                            return;
                        }
                        i++;
                    }
                }
            }
        }

        protected override void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            
        }

        protected override State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
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

            return State.SUCCESS;
        }
    }
}