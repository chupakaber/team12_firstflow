#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scripts.BehaviorTree
{
    public class BehaviorLabelView : StickyNote
    {
        public BehaviorLabel Label;

        public Action<BehaviorLabelView> OnNodeSelected;

        public BehaviorLabelView(BehaviorLabel label)
        {
            Label = label;

            title = Label.Name;
            contents = Label.Contents;
            viewDataKey = Label.Guid;

            style.left = Label.Position.x;
            style.top = Label.Position.y;

            style.width = Label.Size.x;
            style.height = Label.Size.y;

            style.backgroundColor = Label.BackgroundColor;
            style.color = Label.TextColor;

            var children = Children();
            foreach (var child in children)
            {
                child.style.color = new StyleColor(Label.TextColor);
                var children2 = child.Children();
                foreach (var child2 in children2)
                {
                    child2.style.color = new StyleColor(Label.TextColor);
                }
            }
        }

        public override void OnResized()
        {
            if (Label == null)
            {
                return;
            }

            base.OnResized();

            Label.Size.x = this.localBound.width;
            Label.Size.y = this.localBound.height;
        }

        public override void SetPosition(Rect newPos)
        {
            if (Label == null)
            {
                return;
            }

            base.SetPosition(newPos);

            Label.Position.x = newPos.xMin;
            Label.Position.y = newPos.yMin;

            //style.left = Label.Position.x;
            //style.top = Label.Position.y;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            Label.Name = title;
            Label.Contents = contents;
        }
    }
}
#endif