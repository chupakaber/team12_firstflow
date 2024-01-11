using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Scripts.BehaviorTree
{
    [CreateAssetMenu(fileName = "BehaviorTree", menuName = "Team 12/Behavior Tree", order = 10000)]
    public class BehaviorTree : ScriptableObject
    {
        public BehaviorNode.State TreeState = BehaviorNode.State.RUNNING;
        public List<BehaviorNode> Nodes = new List<BehaviorNode>();

        public BehaviorNode.State Run(BehaviorNode rootNode, IBehaviorState internalState, IEvent currentEvent)
        {
            // if (TreeState == BehaviorNode.State.RUNNING)
            // {
            rootNode.Clear();
            TreeState = rootNode.Run(null, 0, internalState, currentEvent);
            // }
            
            return TreeState;
        }

#if UNITY_EDITOR
        public BehaviorNode CreateNode(Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as BehaviorNode;
            
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();

            Nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(BehaviorNode node)
        {
            Nodes.Remove(node);

            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(BehaviorNode parent, int outputPortIndex, BehaviorNode child, int inputPortIndex)
        {
            var decoratorNode = parent as BehaviorDecoratorNode;
            if (decoratorNode != null)
            {
                decoratorNode.Child = child;
                decoratorNode.InputTargetIndex = inputPortIndex;
            }

            var compositeNode = parent as BehaviorCompositeNode;
            if (compositeNode != null)
            {
                compositeNode.Children.Add(child);
                compositeNode.InputTargetIndex.Add(inputPortIndex);
                compositeNode.OutputIndex.Add(outputPortIndex);
            }
        }

        public void RemoveChild(BehaviorNode parent, BehaviorNode child)
        {
            var decoratorNode = parent as BehaviorDecoratorNode;
            if (decoratorNode != null)
            {
                decoratorNode.Child = null;
            }

            var compositeNode = parent as BehaviorCompositeNode;
            if (compositeNode != null)
            {
                var childIndex = compositeNode.Children.IndexOf(child);
                if (childIndex < compositeNode.InputTargetIndex.Count)
                {
                    compositeNode.InputTargetIndex.RemoveAt(childIndex);
                }
                if (childIndex < compositeNode.OutputIndex.Count)
                {
                    compositeNode.OutputIndex.RemoveAt(childIndex);
                }
                compositeNode.Children.Remove(child);
            }
        }

        public List<BehaviorNode> GetChildren(BehaviorNode parent)
        {
            var list = new List<BehaviorNode>();

            var decoratorNode = parent as BehaviorDecoratorNode;
            if (decoratorNode != null && decoratorNode.Child != null)
            {
                list.Add(decoratorNode.Child);
            }

            var compositeNode = parent as BehaviorCompositeNode;
            if (compositeNode != null)
            {
                foreach (var child in compositeNode.Children)
                {
                    list.Add(child);
                }
            }

            return list;
        }
#endif
    }
}