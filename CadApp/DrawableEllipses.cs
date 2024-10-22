using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using HelixToolkit.Wpf;

namespace CadApp
{
    public class DrawableEllipses : Drawable
    {

        public DrawableEllipses(Point topLeft)
        {
            Shape = new Ellipse
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Width = 0,
                Height = 0
            };

            Canvas.SetLeft(Shape, topLeft.X);
            Canvas.SetTop(Shape, topLeft.Y);
        }
        public DrawableEllipses(Ellipse shape) : base(shape)
        {
        }

        public override void extrude(HelixViewport3D helixViewport)
        {
            throw new NotImplementedException();
        }

        public override void followMousePosition_Draw(Point startPosition, Point mousePosition)
        {
            ((Ellipse)Shape).Width = Math.Abs(mousePosition.X - startPosition.X);
            if (mousePosition.X <= startPosition.X)
                Canvas.SetLeft(Shape, mousePosition.X);
            ((Ellipse)Shape).Height = Math.Abs(mousePosition.Y - startPosition.Y);
            if (mousePosition.Y <= startPosition.Y)
                Canvas.SetTop(Shape, mousePosition.Y);
        }
    }    
}
