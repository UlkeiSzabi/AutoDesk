using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CadApp
{
    public class DrawablePoint : Drawable
    {
        private Point _point;

        public DrawablePoint(Point point)
        {
            _point = point;

            Shape = new Ellipse
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Width = 15,
                Height = 15
            };

            Canvas.SetZIndex(Shape, 1);
            Canvas.SetLeft(Shape, point.X - ((Ellipse)Shape).Width / 2);
            Canvas.SetTop(Shape, point.Y - ((Ellipse)Shape).Height / 2);
        }

        public DrawablePoint(Point point, Ellipse ellipse) : base(ellipse)
        {
            _point = point;
            Canvas.SetZIndex(Shape, 1);
            Canvas.SetLeft(Shape, point.X - ((Ellipse)Shape).Width / 2);
            Canvas.SetTop(Shape, point.Y - ((Ellipse)Shape).Height / 2);
        }

        public override void extrude(HelixViewport3D helixViewport)
        {
            //throw new NotImplementedException();
        }

        public override void followMousePosition_Draw(Point pointless, Point mousePosition)
        {
            Canvas.SetLeft(Shape, mousePosition.X - ((Ellipse)Shape).Width / 2);
            Canvas.SetTop(Shape, mousePosition.Y - ((Ellipse)Shape).Height / 2);
        }

    }
}
