using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace CadApp
{
    public class DrawableLine : Drawable
    {
        private Point _endPoint;
        public DrawableLine(Point mousePosition)
        {
            Shape = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            StartPoint = mousePosition;
            _endPoint = mousePosition;
        }
        public DrawableLine(Line shape) : base(shape){ }
        public override void followMousePosition_Draw(Point startPoint, Point mousePosition)
        {
            ((Line)Shape).X1 = startPoint.X;
            ((Line)Shape).Y1 = startPoint.Y;
            ((Line)Shape).X2 = mousePosition.X;
            ((Line)Shape).Y2 = mousePosition.Y;

            _endPoint = mousePosition;
        }
        public override void extrude(HelixViewport3D helixViewport)
        {
            Vector direction = new Vector(_endPoint.X - StartPoint.X, _endPoint.Y - StartPoint.Y);
            Point3D center = new Point3D((_endPoint.X + StartPoint.X) / 2, (_endPoint.Y + StartPoint.Y) / 2, 0);

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

        

        
    }
}
