#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using Scripts.BehaviorTree;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.BehaviorGraph
{
    public class BehaviorGraphView : GraphView
    {
        public class EdgeLabel
        {
            public Edge Edge;
            public Label Label;
        }

        public new class UxmlFactory : UxmlFactory<BehaviorGraphView, UxmlTraits> {}

        public Action<BehaviorNodeView> OnNodeSelected;

        public BehaviorTree.BehaviorTree Tree;
        
        private List<EdgeLabel> EdgeLabels = new List<EdgeLabel>();

        public BehaviorGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Local/Settings/BehaviorTree/BehaviorTreeEditor.uss");
            styleSheets.Add(stylesheet);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviorActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"+ {type.Name.Replace("Node", "")}", (a) => CreateNode(type));
            }

            types = TypeCache.GetTypesDerivedFrom<BehaviorCompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"+ {type.Name.Replace("Node", "")}", (a) => CreateNode(type));
            }

            types = TypeCache.GetTypesDerivedFrom<BehaviorDecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"+ {type.Name.Replace("Node", "")}", (a) => CreateNode(type));
            }
        }

        internal void PopulateView(BehaviorTree.BehaviorTree tree)
        {
            Tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            Tree.Nodes.ForEach(n => CreateNodeView(n));

            foreach (var edgeLabel in EdgeLabels)
            {
                if (edgeLabel != null)
                {
                    if (edgeLabel.Label != null)
                    {
                        edgeLabel.Edge.Remove(edgeLabel.Label);
                    }
                }
            }
            EdgeLabels.Clear();

            Tree.Nodes.ForEach(n => {
                var children = Tree.GetChildren(n);

                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];

                    var parentView = FindNodeView(n);
                    var childView = FindNodeView(child);

                    if (parentView != null && childView != null)
                    {
                        var inputPortIndex = 0;
                        var outputPortIndex = -1;
                        if (parentView.Node is BehaviorCompositeNode)
                        {
                            var compositeNode = (BehaviorCompositeNode) parentView.Node;
                            if (i < compositeNode.InputTargetIndex.Count)
                            {
                                inputPortIndex = compositeNode.InputTargetIndex[i];
                            }
                            if (i < compositeNode.OutputIndex.Count)
                            {
                                outputPortIndex = compositeNode.OutputIndex[i];
                            }
                        }
                        else if (parentView.Node is BehaviorDecoratorNode)
                        {
                            inputPortIndex = ((BehaviorDecoratorNode) parentView.Node).InputTargetIndex;
                        }

                        var inputPort = GetPortByIndex(childView, inputPortIndex, true);

                        if (outputPortIndex == -1 && inputPort != null)
                        {
                            for (var j = 0; j < 6; j++)
                            {
                                var supposedOutputPort = GetPortByIndex(parentView, j, false);
                                if (inputPort.portType.Equals(supposedOutputPort.portType))
                                {
                                    outputPortIndex = j;
                                    j = 6;
                                }
                            }
                        }

                        if (outputPortIndex == -1 && inputPort == null)
                        {
                            for (var j = 0; j < 6; j++)
                            {
                                for (var k = 0; k < 6; k++)
                                {
                                    var supposedOutputPort = GetPortByIndex(parentView, j, false);
                                    var supposedInputPort = GetPortByIndex(childView, k, true);
                                    if (supposedInputPort.portType.Equals(supposedOutputPort.portType))
                                    {
                                        inputPortIndex = k;
                                        inputPort = supposedInputPort;
                                        outputPortIndex = j;
                                        k = 6;
                                        j = 6;
                                    }
                                }
                            }
                        }

                        var outputPort = GetPortByIndex(parentView, outputPortIndex, false);

                        if (inputPort != null && outputPort != null)
                        {
                            var edge = outputPort.ConnectTo(inputPort);

                            if (parentView.Node is BehaviorCompositeNode)
                            {
                                var compositeNode = (BehaviorCompositeNode) parentView.Node;
                                if (compositeNode.Children.Count > 1)
                                {
                                    var edgeLabel = new Label();
                                    var childIndex = 0;
                                    foreach (var child2 in compositeNode.Children)
                                    {
                                        if (child2.Equals(childView.Node))
                                        {
                                            break;
                                        }
                                        childIndex++;
                                    }
                                    edgeLabel.text = childIndex.ToString();
                                    edgeLabel.style.backgroundColor = new StyleColor() { value = new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1f) };
                                    edgeLabel.style.paddingLeft = 6;
                                    edgeLabel.style.paddingTop = 6;
                                    edgeLabel.style.paddingRight = 6;
                                    edgeLabel.style.paddingBottom = 6;
                                    edgeLabel.style.borderTopLeftRadius = 10;
                                    edgeLabel.style.borderTopRightRadius = 10;
                                    edgeLabel.style.borderBottomRightRadius = 10;
                                    edgeLabel.style.borderBottomLeftRadius = 10;
                                    EdgeLabels.Add(new EdgeLabel() { Edge = edge, Label = edgeLabel });
                                    edge.Add(edgeLabel);
                                }
                            }

                            AddElement(edge);
                        }
                    }
                }
            });
        }

        public override void HandleEvent(EventBase evt)
        {
            base.HandleEvent(evt);
            SetEdgeLabelPosition();
        }

        private void SetEdgeLabelPosition()
        {
            foreach (var label in EdgeLabels)
            {
                if (label.Edge != null && label.Edge.input != null && label.Edge.output != null && label.Label != null)
                {
                    var rect = new UnityEngine.Rect();
                    var offset = (label.Edge.input.LocalToWorld(rect).center + label.Edge.output.LocalToWorld(rect).center) / 2f;
                    offset = label.Edge.WorldToLocal(offset);
                    label.Label.style.left = offset.x + 50f;
                    label.Label.style.top = offset.y - 5f;
                }
            }
        }

        private Port GetPortByIndex(BehaviorNodeView nodeView, int index, bool isInput)
        {
            switch (index)
            {
                case 0:
                    return isInput ? nodeView.Input1 : nodeView.Output1;
                case 1:
                    return isInput ? nodeView.Input2 : nodeView.Output2;
                case 2:
                    return isInput ? nodeView.Input3 : nodeView.Output3;
                case 3:
                    return isInput ? nodeView.Input4 : nodeView.Output4;
                case 4:
                    return isInput ? nodeView.Input5 : nodeView.Output5;
                case 5:
                    return isInput ? nodeView.Input6 : nodeView.Output6;
            }

            return null;
        }

        internal BehaviorNodeView FindNodeView(BehaviorNode node)
        {
            if (node != null)
            {
                var nodeView = GetNodeByGuid(node.Guid);
                if (nodeView != null)
                {
                    return nodeView as BehaviorNodeView;
                }
            }
            return null;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    var nodeView = element as BehaviorNodeView;
                    if (nodeView != null)
                    {
                        Tree.DeleteNode(nodeView.Node);
                    }

                    var edge = element as Edge;
                    if (edge != null)
                    {
                        var parentNodeView = edge.output.node as BehaviorNodeView;
                        var childNodeView = edge.input.node as BehaviorNodeView;
                        Tree.RemoveChild(parentNodeView.Node, childNodeView.Node);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var parentNodeView = edge.output.node as BehaviorNodeView;
                    var childNodeView = edge.input.node as BehaviorNodeView;
                    var inputIndex = 0;
                    var outputIndex = 0;
                    
                    if (edge.input.Equals(childNodeView.Input2))
                    {
                        inputIndex = 1;
                    }
                    else if (edge.input.Equals(childNodeView.Input3))
                    {
                        inputIndex = 2;
                    }
                    else if (edge.input.Equals(childNodeView.Input4))
                    {
                        inputIndex = 3;
                    }
                    else if (edge.input.Equals(childNodeView.Input5))
                    {
                        inputIndex = 4;
                    }
                    else if (edge.input.Equals(childNodeView.Input6))
                    {
                        inputIndex = 5;
                    }

                    if (edge.output.Equals(parentNodeView.Output2))
                    {
                        outputIndex = 1;
                    }
                    else if (edge.output.Equals(parentNodeView.Output3))
                    {
                        outputIndex = 2;
                    }
                    else if (edge.output.Equals(parentNodeView.Output4))
                    {
                        outputIndex = 3;
                    }
                    else if (edge.output.Equals(parentNodeView.Output5))
                    {
                        outputIndex = 4;
                    }
                    else if (edge.output.Equals(parentNodeView.Output6))
                    {
                        outputIndex = 5;
                    }
                    Tree.AddChild(parentNodeView.Node, outputIndex, childNodeView.Node, inputIndex);
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(
                endPort => 
                    endPort.direction != startPort.direction && 
                    endPort.node != startPort.node && 
                    endPort.portType.Equals(startPort.portType)).ToList();
        }

        private void CreateNode(Type type)
        {
            var newNode = Tree.CreateNode(type);
            newNode.Name = type.Name.Replace("Node", "");
            newNode.name = newNode.Name;
            CreateNodeView(newNode);
        }

        private void CreateNodeView(BehaviorNode node)
        {
            var nodeView = new BehaviorNodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.style.backgroundColor = node.DefaultColor;
            AddElement(nodeView);
            //var pointerPosition = Pointer.current.position;
            //nodeView.style.left = pointerPosition.value.x;
            //nodeView.style.top = pointerPosition.value.y;
        }
    }
}
#endif