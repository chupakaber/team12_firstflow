using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public interface IGameObjectWithState
    {
        int ObjectID { get; }
        List<float> States { get; }
        GameObject GetGameObject();
        void OnLoad();
        void OnInteract();
        void OnActivate(IQueuedEvent queuedEvent);
    }
}