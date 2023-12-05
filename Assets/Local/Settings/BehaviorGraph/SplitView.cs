#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace Scripts.BehaviorGraph
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> {}
        
    }
}
#endif