using System.Windows;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;

namespace CadApp
{
    public class DrawableRect : Drawable
    {
        private Point _topLeft;
        private Point _bottomRight;


        public DrawableRect(Point topLeft)
        {
            Shape = new System.Windows.Shapes.Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Width = 0,
                Height = 0
            };

            Canvas.SetLeft(Shape, topLeft.X);
            Canvas.SetTop(Shape, topLeft.Y);

            _topLeft = topLeft;
            _bottomRight = topLeft;
        }
        public DrawableRect(Rectangle shape) : base(shape){}

        public override void extrude(HelixViewport3D helixViewport)
        {
            Point bottomLeft = new Point(_topLeft.X, _bottomRight.Y);
            Point topRight = new Point(_bottomRight.X, _topLeft.Y);

            InsertWall(_topLeft, topRight, helixViewport);
            InsertWall(_topLeft, bottomLeft, helixViewport);
            InsertWall(bottomLeft, _bottomRight, helixViewport);
            InsertWall(topRight, _bottomRight, helixViewport);
        }

        private void InsertWall(Point topLeft, Point bottomRight, HelixViewport3D helixViewport)
        {
            Vector direction = new Vector(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
            Point3D center = new Point3D((bottomRight.X + topLeft.X) / 2, (bottomRight.Y + topLeft.Y) / 2, 0);

            // Convert 2D line to 3D wall (extrude vertically)
            var wall = new BoxVisual3D
            {
                Width = 10,  // Width of the wall
                Height = 30,  // Fixed wall height
                Length = direction.Length,  // Length of the wall
                Center = center,
                Fill = Brushes.Green
            };

            direction.Normalize();

            double angle = Math.Atan2(direction.Y, direction.X) * (180 / Math.PI);  // Convert to degrees
            var rotation = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle), center  // Rotate around the Z-axis
            );

            wall.Transform = rotation;

            // Add the wall to the 3D viewport
            helixViewport.Children.Add(wall);
        }

        public override void followMousePosition_Draw(Point startPosition, Point mousePosition)
        {
            ((Rectangle)Shape).Width = Math.Abs(mousePosition.X - startPosition.X);
            if (mousePosition.X <= startPosition.X)
            {
                Canvas.SetLeft(Shape, mousePosition.X);
                _topLeft.X = mousePosition.X;
                _bottomRight.X = startPosition.X;
            }
            else
            {
                _bottomRight.X = mousePosition.X;
            }

            ((Rectangle)Shape).Height = Math.Abs(mousePosition.Y - startPosition.Y);
            if (mousePosition.Y <= startPosition.Y)
            {
                Canvas.SetTop(Shape, mousePosition.Y);
                _topLeft.Y = mousePosition.Y;
                _bottomRight.Y = startPosition.Y;
            }
            else
            {
                _bottomRight.Y = mousePosition.Y; ;
            }
        }


    }
}
