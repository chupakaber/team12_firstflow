using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class BehaviorLabel : ScriptableObject
    {
        [HideInInspector]
        public string Guid;
        [HideInInspector]
        public Vector2 Position;
        [HideInInspector]
        public Vector2 Size = new Vector2(300f, 90f);

        public string Name;
        public string Contents;
        public Color BackgroundColor = new Color(0.2f, 0.235f, 0.294f, 1f);
        public Color TextColor = new Color(0.487f, 0.723f, 0.867f, 1f);
    }
}