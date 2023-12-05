#if UNITY_EDITOR
using Scripts.BehaviorTree;
using UnityEditor;
using UnityEngine.UIElements;

namespace Scripts.BehaviorGraph
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {}
        
        private IMGUIContainer _container;
        private Editor _editor;
        private BehaviorNodeView _currentNodeView;

        public InspectorView()
        {
            
        }

        internal void UpdateSelection(BehaviorNodeView nodeView)
        {
            Clear();

            if (_editor != null)
            {
                _container.onGUIHandler -= OnGUI;
                _container = null;
                UnityEngine.Object.DestroyImmediate(_editor);
            }

            _currentNodeView = nodeView;

            _editor = Editor.CreateEditor(nodeView.Node);
            _container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            _container.onGUIHandler += OnGUI;

            OnGUI();

            Add(_container);
        }

        private void OnGUI()
        {
            if (_currentNodeView.name != _currentNodeView.Node.Name)
            {
                _currentNodeView.Node.name = _currentNodeView.Node.Name;
                _currentNodeView.name = _currentNodeView.Node.Name;
                _currentNodeView.title = _currentNodeView.Node.Name;
            }
        }
    }
}
#endif