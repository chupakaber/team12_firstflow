#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Scripts.BehaviorTree
{
    public class BehaviorNodeView : Node
    {
        public BehaviorNode Node;
        public Port Input1;
        public Port Input2;
        public Port Input3;
        public Port Input4;
        public Port Input5;
        public Port Input6;
        public Port Output1;
        public Port Output2;
        public Port Output3;
        public Port Output4;
        public Port Output5;
        public Port Output6;
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

        public Action<BehaviorNodeView> OnNodeSelected;

        public BehaviorNodeView(BehaviorNode node)
        {
            Node = node;
            title = node.Name;
            viewDataKey = node.Guid;

            style.left = Node.Position.x;
            style.top = Node.Position.y;

            Input1Type = node.Input1Type;
            Input2Type = node.Input2Type;
            Input3Type = node.Input3Type;
            Input4Type = node.Input4Type;
            Input5Type = node.Input5Type;
            Input6Type = node.Input6Type;
            Output1Type = node.Output1Type;
            Output2Type = node.Output2Type;
            Output3Type = node.Output3Type;
            Output4Type = node.Output4Type;
            Output5Type = node.Output5Type;
            Output6Type = node.Output6Type;

            CreateInputPorts();
            CreateOutputPorts();
        }

        protected virtual void CreateOutputPorts()
        {
            if (Node is BehaviorActionNode || Node is BehaviorCompositeNode || Node is BehaviorDecoratorNode)
            {
                if (Input1Type != null)
                {
                    Input1 = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Input1Type);
                }

                if (Input2Type != null)
                {
                    Input2 = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Input2Type);
                }

                if (Input3Type != null)
                {
                    Input3 = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Input3Type);
                }

                if (Input4Type != null)
                {
                    Input4 = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Input4Type);
                }

                if (Input5Type != null)
                {
                    Input5 = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Input5Type);
                }

                if (Input6Type != null)
                {
                    Input6 = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, Input6Type);
                }
            }

            if (Input1 != null)
            {
                if (Node.Input1Name != "")
                {
                    Input1.portName = Node.Input1Name;
                }
                else
                {
                    Input1.portName = Input1Type.Name;
                }
                inputContainer.Add(Input1);
            }

            if (Input2 != null)
            {
                if (Node.Input2Name != "")
                {
                    Input2.portName = Node.Input2Name;
                }
                else
                {
                    Input2.portName = Input2Type.Name;
                }
                inputContainer.Add(Input2);
            }

            if (Input3 != null)
            {
                if (Node.Input3Name != "")
                {
                    Input3.portName = Node.Input3Name;
                }
                else
                {
                    Input3.portName = Input3Type.Name;
                }
                inputContainer.Add(Input3);
            }

            if (Input4 != null)
            {
                if (Node.Input4Name != "")
                {
                    Input4.portName = Node.Input4Name;
                }
                else
                {
                    Input4.portName = Input4Type.Name;
                }
                inputContainer.Add(Input4);
            }

            if (Input5 != null)
            {
                if (Node.Input5Name != "")
                {
                    Input5.portName = Node.Input5Name;
                }
                else
                {
                    Input5.portName = Input5Type.Name;
                }
                inputContainer.Add(Input5);
            }

            if (Input6 != null)
            {
                if (Node.Input6Name != "")
                {
                    Input6.portName = Node.Input6Name;
                }
                else
                {
                    Input6.portName = Input6Type.Name;
                }
                inputContainer.Add(Input6);
            }
        }

        protected virtual void CreateInputPorts()
        {
            if (Node is BehaviorActionNode || Node is BehaviorCompositeNode || Node is BehaviorDecoratorNode)
            {
                if (Output1Type != null)
                {
                    Output1 = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Output1Type);
                }

                if (Output2Type != null)
                {
                    Output2 = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Output2Type);
                }

                if (Output3Type != null)
                {
                    Output3 = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Output3Type);
                }

                if (Output4Type != null)
                {
                    Output4 = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Output4Type);
                }

                if (Output5Type != null)
                {
                    Output5 = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Output5Type);
                }

                if (Output6Type != null)
                {
                    Output6 = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Output6Type);
                }
            }

            if (Output1 != null)
            {
                if (Node.Output1Name != "")
                {
                    Output1.portName = Node.Output1Name;
                }
                else
                {
                    Output1.portName = Output1Type.Name;
                }
                inputContainer.Add(Output1);
            }

            if (Output2 != null)
            {
                if (Node.Output2Name != "")
                {
                    Output2.portName = Node.Output2Name;
                }
                else
                {
                    Output2.portName = Output2Type.Name;
                }
                inputContainer.Add(Output2);
            }

            if (Output3 != null)
            {
                if (Node.Output3Name != "")
                {
                    Output3.portName = Node.Output3Name;
                }
                else
                {
                    Output3.portName = Output3Type.Name;
                }
                inputContainer.Add(Output3);
            }

            if (Output4 != null)
            {
                if (Node.Output4Name != "")
                {
                    Output4.portName = Node.Output4Name;
                }
                else
                {
                    Output4.portName = Output4Type.Name;
                }
                inputContainer.Add(Output4);
            }

            if (Output5 != null)
            {
                if (Node.Output5Name != "")
                {
                    Output5.portName = Node.Output5Name;
                }
                else
                {
                    Output5.portName = Output5Type.Name;
                }
                inputContainer.Add(Output5);
            }

            if (Output6 != null)
            {
                if (Node.Output6Name != "")
                {
                    Output6.portName = Node.Output6Name;
                }
                else
                {
                    Output6.portName = Output6Type.Name;
                }
                inputContainer.Add(Output6);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }
    }
}
#endif