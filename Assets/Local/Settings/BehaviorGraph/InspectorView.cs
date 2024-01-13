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
        private BehaviorLabelView _currentLabelView;

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

        internal void UpdateLabelSelection(BehaviorLabelView labelView)
        {
            Clear();

            if (_editor != null)
            {
                _container.onGUIHandler -= OnGUI;
                _container = null;
                UnityEngine.Object.DestroyImmediate(_editor);
            }

            _currentLabelView = labelView;

            _editor = Editor.CreateEditor(labelView.Label);
            _container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            _container.onGUIHandler += OnGUI;

            OnGUI();

            Add(_container);
        }

        private void OnGUI()
        {
            if (_currentNodeView != null && _currentNodeView.name != _currentNodeView.Node.Name)
            {
                _currentNodeView.Node.name = _currentNodeView.Node.Name;
                _currentNodeView.name = _currentNodeView.Node.Name;
                _currentNodeView.title = _currentNodeView.Node.Name;
            }
            else if (_currentLabelView != null)
            {
                if (_currentLabelView.name != _currentLabelView.Label.Name)
                {
                    _currentLabelView.Label.name = _currentLabelView.Label.Name;
                    _currentLabelView.name = _currentLabelView.Label.Name;
                    _currentLabelView.title = _currentLabelView.Label.Name;
                }
                else if (_currentLabelView.contents.CompareTo(_currentLabelView.Label.Contents) != 0)
                {
                    _currentLabelView.contents = _currentLabelView.Label.Contents;
                }
                else if (!_currentLabelView.style.backgroundColor.Equals(_currentLabelView.Label.BackgroundColor))
                {
                    _currentLabelView.style.backgroundColor = _currentLabelView.Label.BackgroundColor;
                }
                else if (!_currentLabelView.style.color.Equals(_currentLabelView.Label.TextColor))
                {
                    _currentLabelView.style.color = new StyleColor(_currentLabelView.Label.TextColor);
                    var children = _currentLabelView.Children();
                    foreach (var child in children)
                    {
                        child.style.color = new StyleColor(_currentLabelView.Label.TextColor);
                        var children2 = child.Children();
                        foreach (var child2 in children2)
                        {
                            child2.style.color = new StyleColor(_currentLabelView.Label.TextColor);
                        }
                    }
                }
            }
        }
    }
}
#endif