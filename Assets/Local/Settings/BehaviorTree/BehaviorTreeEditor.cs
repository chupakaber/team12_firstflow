#if UNITY_EDITOR
using Scripts.BehaviorTree;
using Scripts.BehaviorGraph;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

public class BehaviorTreeEditor : EditorWindow
{
    public BehaviorGraphView TreeView;
    public InspectorView InspectorView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Team 12/Behavior Tree Editor")]
    public static void ShowWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        wnd.OnSelectionChange();
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            ShowWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Local/Settings/BehaviorTree/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);

        var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Local/Settings/BehaviorTree/BehaviorTreeEditor.uss");
        root.styleSheets.Add(stylesheet);

        TreeView = root.Q<BehaviorGraphView>();
        InspectorView = root.Q<InspectorView>();

        TreeView.OnNodeSelected = OnNodeSelectionChanged;
        TreeView.OnLabelSelected = OnLabelSelectionChanged;
    }

    public void OnSelectionChange()
    {
        var tree = Selection.activeObject as BehaviorTree;
        if (tree != null)
        {
            TreeView.PopulateView(tree);
        }
    }

    private void OnNodeSelectionChanged(BehaviorNodeView nodeView)
    {
        InspectorView.UpdateSelection(nodeView);
    }
    
    private void OnLabelSelectionChanged(BehaviorLabelView labelView)
    {
        InspectorView.UpdateLabelSelection(labelView);
    }
}
#endif