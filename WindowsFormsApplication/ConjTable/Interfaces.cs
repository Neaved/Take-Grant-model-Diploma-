using System.Drawing;
using Entity.entity;
using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    /// Умеет себя рисовать
    /// </summary>
    public interface IDrawable
    {
        void Paint(Graphics gr);
        //void Paint(Graphics gr, List<MatrixElement> ownerElemens, List<MatrixElement> adminElemens);
    }

    /// <summary>
    /// Умеет себя перемещать
    /// </summary>
    public interface IDragable
    {
        bool Hit(PointF point);
        void Drag(PointF offset);
        IDragable StartDrag(PointF p);
        void EndDrag();
    }
}
