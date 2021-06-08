using System;
using System.Drawing;
using System.Windows.Forms;

namespace FFBardMusicPlayer
{
    public static class FormExtensions
    {
        public static void Invoke<TControlType>(this TControlType control, Action<TControlType> del)
            where TControlType : Control
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => del(control)));
            }
            else
            {
                del(control);
            }
        }

        public static double GetDistance(this PointF p1, PointF p2) =>
            Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

        public static void DrawCircle(this Graphics g, Pen pen,
            float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
            float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                radius + radius, radius + radius);
        }

        public static Point FormRelativeLocation(this Control control, Form form = null)
        {
            if (form == null)
            {
                form = control.FindForm();
                if (form == null)
                {
                    throw new Exception("Form not found.");
                }
            }

            var cScreen = control.PointToScreen(control.Location);
            var fScreen = form.Location;
            var cFormRel = new Point(cScreen.X - fScreen.X, cScreen.Y - fScreen.Y);

            return cFormRel;
        }
    }
}