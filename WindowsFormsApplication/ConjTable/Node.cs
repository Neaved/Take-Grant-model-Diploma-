using System.Collections.Generic;
using System.Drawing;
using Entity.entity;

namespace System.Windows.Forms
{
    public class Node : IDrawable, IDragable
    {
        public List<Node> Linked { get; set; } = new List<Node>();

        public string Label;
        private bool isObject;
        private bool isOwner;
        private bool isAdministrator;
        public float Radius = 50f;
        public PointF Location;

        public void Drag(PointF offset)
        {
            Location = Location.Add(offset);
        }

        public void EndDrag() { }

        public bool Hit(PointF point)
        {
            var p = point.Sub(Location);
            return Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) <= Radius * Radius;
        }

        public void Paint(Graphics gr)
        {
            foreach (var item in Linked)
            {
                gr.DrawLink(Location, item.Location, Radius, getPenName(item));
            }

            var state = gr.Save();
            gr.TranslateTransform(Location.X, Location.Y);
            if (IsObject)
            {
                gr.DrawCircle(Radius, Pens.Black);
            }
            else
            {
                gr.DrawCircle(Radius, Pens.Gray);
            }
            if (!string.IsNullOrEmpty(Label))
            {
                gr.DrawCenteredString(
                    Label,
                    SystemFonts.CaptionFont,
                    SystemBrushes.ControlDarkDark,
                    Radius);
            }
            gr.Restore(state);
        }

        private string getPenName(Node item)
        {
            string pen;
            if (item.IsOwner && this.IsOwner)
            {
                pen = "bluePen";
            }
            else if (item.IsAdministrator && this.IsAdministrator)
            {
                pen = "redPen";
            }
            else
            {
                pen = "blackPen";
            }
            return pen;
        }

        public IDragable StartDrag(PointF p)
        {
            return this;
        }

        public override string ToString()
        {
            return string.Format("Node: {0}", Label);
        }


        public bool IsOwner
        {
            get
            {
                return isOwner;
            }

            set
            {
                isOwner = value;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return isAdministrator;
            }

            set
            {
                isAdministrator = value;
            }
        }

        public bool IsObject
        {
            get
            {
                return isObject;
            }

            set
            {
                isObject = value;
            }
        }
    }
}
