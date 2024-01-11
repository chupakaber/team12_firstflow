using System;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public abstract class BehaviorNode : ScriptableObject
    {
        public Type Input1Type;
        public Type Input2Type;
        public Type Input3Type;
        public Type Input4Type;
        public Type Input5Type;
        public Type Input6Type;
        public Type Output1Type;
        public Type Output2Type;
        public Type Output3Type;
        public Type Output4Type;
        public Type Output5Type;
        public Type Output6Type;
        public virtual string Input1Name { get { return ""; } }
        public virtual string Input2Name { get { return ""; } }
        public virtual string Input3Name { get { return ""; } }
        public virtual string Input4Name { get { return ""; } }
        public virtual string Input5Name { get { return ""; } }
        public virtual string Input6Name { get { return ""; } }
        public virtual string Output1Name { get { return ""; } }
        public virtual string Output2Name { get { return ""; } }
        public virtual string Output3Name { get { return ""; } }
        public virtual string Output4Name { get { return ""; } }
        public virtual string Output5Name { get { return ""; } }
        public virtual string Output6Name { get { return ""; } }

        public string Name;

        [HideInInspector]
        public enum State
        {
            RUNNING = 0,
            FAILURE = 1,
            SUCCESS = 2
        }
        
        [HideInInspector]
        public State CurrentState;
        [HideInInspector]
        public bool Started;
        [HideInInspector]
        public string Guid;
        [HideInInspector]
        public Vector2 Position;
        [HideInInspector]
        public virtual Color DefaultColor { get { return new Color(0.23f, 0.23f, 0.23f, 1f); } }

        public State Run(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent)
        {
            if (!Started)
            {
                OnStart(parent, inputIndex, internalState, currentEvent);
                Started = true;
            }

            CurrentState = OnUpdate(parent, inputIndex, internalState, currentEvent);

            if (CurrentState == State.FAILURE || CurrentState == State.SUCCESS)
            {
                OnStop(parent, inputIndex, internalState, currentEvent);
                Started = false;
            }

            return CurrentState;
        }

        public virtual void Clear()
        {
            if (CurrentState != State.RUNNING)
            {
                CurrentState = State.RUNNING;
                Started = false;
            }
        }

        protected abstract void OnStart(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent);
        protected abstract void OnStop(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent);
        protected abstract State OnUpdate(BehaviorNode parent, int inputIndex, IBehaviorState internalState, IEvent currentEvent);
    }
}