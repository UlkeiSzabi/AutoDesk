using System.Windows;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Input;

namespace CadApp
{
    public abstract class Drawable
    {
        private Shape? _shape;
        private System.Windows.Point? _startPoint;

        public Drawable() { }
        public Drawable(Shape shape)
        {
            this.Shape = shape;
            this.Shape.MouseLeftButtonDown += selectShape;
            this.Shape.MouseLeftButtonUp += stopMove;
        }


        public System.Windows.Point StartPoint { get => _startPoint.Value; set => _startPoint = value; }
        public Shape Shape { get => _shape; set => _shape = value; }


        public abstract void followMousePosition_Draw(System.Windows.Point startPosition, System.Windows.Point mousePosition);
        public abstract void extrude(HelixViewport3D helixViewport);
       // public abstract void followMousePosition_Move();


        public virtual List<Drawable> addIntersectingPoints(Drawable currentShape, List<Drawable> shapes)
        {
            List<Drawable> intersectingShapes = new List<Drawable>();

            foreach (Drawable child in shapes)
            {
                if (child is DrawablePoint && child.Shape != Shape)
                {
                    if (child.Shape.RenderedGeometry != null && Shape.RenderedGeometry != null &&
                        child.Shape.RenderedGeometry.Bounds.IntersectsWith(Shape.RenderedGeometry.Bounds))
                    {
                        intersectingShapes.Add(child);
                    }
                }
                else
                {

                }
            }

            return intersectingShapes;
        }
        public virtual void stopMove(object sender, MouseButtonEventArgs e)
        {
            _startPoint = null;
        }
        public void selectShape(object sender, MouseButtonEventArgs e)
        {
            this.StartPoint = e.GetPosition((Canvas)VisualTreeHelper.GetParent((Shape)sender));
        }
    }
}   
