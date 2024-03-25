using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class GameObectWithStateView : MonoBehaviour, IGameObjectWithState
    {
        public int ObjectID { get { return objectID; } }
        public List<float> States { get { return states; } }

        [SerializeField]
        private int objectID;
        [SerializeField]
        private List<float> states = new List<float>();

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public virtual void OnLoad()
        {
        }

        public virtual void OnInteract()
        {
        }

        public virtual void OnActivate(IQueuedEvent queuedEvent)
        {
        }
    }
}