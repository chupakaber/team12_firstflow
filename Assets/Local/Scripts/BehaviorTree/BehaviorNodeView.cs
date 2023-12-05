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
        public Port Output1;
        public Port Output2;
        public Type Input1Type;
        public Type Input2Type;
        public Type Input3Type;
        public Type Input4Type;
        public Type Output1Type;
        public Type Output2Type;

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
            Output1Type = node.Output1Type;
            Output2Type = node.Output2Type;

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