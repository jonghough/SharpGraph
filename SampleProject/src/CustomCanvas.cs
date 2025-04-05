using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace SampleProject
{
    public class CustomCanvas : Control
    {
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var pen = new Pen(Brushes.Red, 2);
            var brush = Brushes.Blue;

            // Draw a line
            context.DrawLine(pen, new Point(50, 50), new Point(200, 200));

            // Draw a circle
            context.DrawEllipse(brush, pen, new Point(300, 150), 50, 50);
        }
    }
}
