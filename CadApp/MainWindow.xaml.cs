using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Types;

namespace Types
{
    public enum ShapeType
    {
        Point,
        Line,
        Ellipses,
        Rectangle
    }
}

namespace CadApp
{

    public static class View2D
    {
        private static System.Windows.Point? _startPoint;
        private static Drawable? _currentShape;
        private static ShapeType _currentShapeType;
        private static List<Drawable> _shapes = new List<Drawable>();

        public static List<Drawable> Shapes { get => _shapes; set => _shapes = value; }

        public static void startDrawing(Types.ShapeType st, Point mousePosition, Canvas canvas)
        {
            if(st is Types.ShapeType.Line)
            {
                _startPoint = mousePosition;
                _currentShapeType = ShapeType.Line;
                _currentShape = new DrawableLine(mousePosition);

                DrawablePoint lineStartPoint = new DrawablePoint(mousePosition);
                canvas.Children.Add(lineStartPoint.Shape);
                Shapes.Add(lineStartPoint);
            }
            else if(st is ShapeType.Ellipses)
            {
                _startPoint = mousePosition;
                _currentShapeType = ShapeType.Ellipses;
                _currentShape = new DrawableEllipses(mousePosition);
            }
            else if(st is ShapeType.Rectangle)
            {
                _startPoint = mousePosition;
                _currentShapeType = ShapeType.Rectangle;
                _currentShape = new DrawableRect(mousePosition);

                DrawablePoint rectStartPoint = new DrawablePoint(mousePosition);
                canvas.Children.Add(rectStartPoint.Shape);
                Shapes.Add(rectStartPoint);
            }
            else
            {
                _startPoint = mousePosition;
                _currentShapeType = ShapeType.Point;
                _currentShape = new DrawablePoint(mousePosition);

            }
            Canvas.SetZIndex(_currentShape.Shape, 1); // Bring line to front
            canvas.Children.Add(_currentShape.Shape);
          
        }

        public static void mouseMove(Point mousePosition)
        {
            if (_startPoint.HasValue && _currentShape != null)
            {
                _currentShape.followMousePosition_Draw(_startPoint.Value, mousePosition);
            }
        }

        public static void stopDrawing(Point mousePosition, Canvas canvas)
        {
            if ((_currentShapeType is ShapeType.Line) || (_currentShapeType is ShapeType.Rectangle))
            {
                DrawablePoint endPoint = new DrawablePoint(mousePosition);
                Shapes.Add(endPoint);
                canvas.Children.Add(endPoint.Shape);
            }

            Shapes.Add(_currentShape);
            _currentShape = null;
            _startPoint = null;
        }
    }

    public static class View3D
    {
        // Convert the 2D canvas objects to 3D objects in the HelixViewport3D
        public static void ConvertTo3D(List<Drawable> shapes, HelixViewport3D helixViewport)
        {
            // Clear existing 3D children
            helixViewport.Children.Clear();

            // Add lighting
            var light = new ModelVisual3D
            {
                Content = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -1))
            };
            helixViewport.Children.Add(light);


            // Iterate over lines in the Canvas and convert them to 3D walls
            foreach (var child in shapes)
            {
                child.extrude(helixViewport);
            }
            CenterCamera(helixViewport, (PerspectiveCamera)helixViewport.Camera);
        }

        private static void CenterCamera(HelixViewport3D viewport, PerspectiveCamera camera)
        {
            Rect3D bounds = CalculateBoundingBox(viewport);
            Point3D center = new Point3D(
                bounds.X + bounds.SizeX / 2,
                bounds.Y + bounds.SizeY / 2,
                bounds.Z + bounds.SizeZ / 2
            );

            // Calculate distance to move the camera based on the size of the bounding box
            double radius = Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ)) * 1.5;

            // Position the camera at a distance from the center along the Z-axis
            camera.Position = new Point3D(center.X, center.Y + radius, center.Z + radius);
            camera.LookDirection = new Vector3D(center.X - camera.Position.X, center.Y - camera.Position.Y, center.Z - camera.Position.Z);
            camera.UpDirection = new Vector3D(0, 1, 0);  // Ensure "up" is the Y-axis
        }

        private static Rect3D CalculateBoundingBox(HelixViewport3D viewport)
        {
            Rect3D bounds = Rect3D.Empty;
            foreach (var element in viewport.Children)
            {
                if (element is ModelVisual3D modelVisual)
                {
                    Rect3D modelBounds = modelVisual.Content.Bounds;
                    bounds.Union(modelBounds);
                }
            }
            return bounds;
        }

    }

    public partial class MainWindow : Window
    {
        private Types.ShapeType _currentShape = Types.ShapeType.Point;

        public MainWindow()
        {
            InitializeComponent();
        }

        // When user presses mouse button, start drawing a line
        private void FloorPlan_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            View2D.startDrawing(_currentShape, e.GetPosition(FloorPlan), FloorPlan);
        }

        // Update the current line as the mouse moves
        private void FloorPlan_MouseMove(object sender, MouseEventArgs e)
        {
            View2D.mouseMove(e.GetPosition(FloorPlan));
        }

        // Finish drawing the line when the mouse button is released
        private void FloorPlan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            View2D.stopDrawing(e.GetPosition(FloorPlan), FloorPlan);
        }

        // Button click event to convert the 2D floor plan to 3D
        private void ConvertTo3D_Click(object sender, RoutedEventArgs e)
        {
            // Hide the 2D canvas and "Convert to 3D" button, show the 3D view and "Return to 2D" button
            FloorPlan.Visibility = Visibility.Collapsed;
            ConvertTo3DButton.Visibility = Visibility.Collapsed;
            helixViewport.Visibility = Visibility.Visible;
            ReturnTo2DButton.Visibility = Visibility.Visible;

            View3D.ConvertTo3D(View2D.Shapes, helixViewport);
        }


        // Button click event to return to the 2D view
        private void ReturnTo2D_Click(object sender, RoutedEventArgs e)
        {
            // Show the 2D canvas and "Convert to 3D" button, hide the 3D view and "Return to 2D" button
            FloorPlan.Visibility = Visibility.Visible;
            ConvertTo3DButton.Visibility = Visibility.Visible;
            helixViewport.Visibility = Visibility.Collapsed;
            ReturnTo2DButton.Visibility = Visibility.Collapsed;
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            _currentShape = Types.ShapeType.Line;
        }

        private void Ellipses_Click(object sender, RoutedEventArgs e)
        {
            _currentShape = Types.ShapeType.Ellipses;
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            _currentShape = Types.ShapeType.Rectangle;
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            _currentShape = Types.ShapeType.Point;
        }
    }
}
