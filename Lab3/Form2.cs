using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form2 : Form
    {
        Bitmap bitmap;
        List<Point> points;

        public Form2()
        {
            InitializeComponent();

            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;

            points = new List<Point>();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Point point = pictureBox1.PointToClient(Cursor.Position);

            using (Graphics graphic = pictureBox1.CreateGraphics())
                if (points.Count < 2 && !points.Contains(point))
                {
                    graphic.FillRectangle(Brushes.Purple, point.X, point.Y, 2, 2);
                    points.Add(point);
                }

            if (radioButton1.Checked && (points.Count == 2))
            {
                int x0 = points[points.Count - 2].X;
                int y0 = points[points.Count - 2].Y;
                int x1 = points[points.Count - 1].X;
                int y1 = points[points.Count - 1].Y;

                Bresenham(x0, y0, x1, y1);
                pictureBox1.Image = bitmap;
            }

            if (radioButton2.Checked && (points.Count == 2))
            {
                int x0 = points[points.Count - 2].X;
                int y0 = points[points.Count - 2].Y;
                int x1 = points[points.Count - 1].X;
                int y1 = points[points.Count - 1].Y;

                Wu(x0, y0, x1, y1);
                pictureBox1.Image = bitmap;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            points.Clear();
            Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                points.Clear();
                Clear();
            }
            else if (radioButton2.Checked)
            {
                points.Clear();
                Clear();
            }
        }

        private void Clear()
        {
            var graphic = Graphics.FromImage(pictureBox1.Image);
            graphic.Clear(pictureBox1.BackColor);

            pictureBox1.Image = pictureBox1.Image;
        }

        private void Bresenham(int x0, int y0, int x1, int y1)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;

            var deltaX = Math.Abs(dx);
            var deltaY = Math.Abs(dy);

            var dirX = dx < 0 ? -1 : 1;
            var dirY = dy < 0 ? -1 : 1;

            var dist = deltaX;
            if (deltaY >= deltaX)
                dist = deltaY;

            var errorX = 0;
            var errorY = 0;

            var dst = dist + 1;
            while (dst-- > 0)
            {
                bitmap.SetPixel(x0, y0, Color.Purple);

                errorX += deltaX;
                errorY += deltaY;

                if (errorX >= dist)
                {
                    x0 += dirX;
                    errorX -= dist;
                }

                if (errorY >= dist)
                {
                    y0 += dirY;
                    errorY -= dist;
                }
            }
        }

        private void Wu(int x0, int y0, int x1, int y1)
        {
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int dx = x1 - x0;
            int dy = y1 - y0;

            double gradient;

            if (dx == 0)
                gradient = 1;
            else if (dy == 0)
                gradient = 0;
            else
                gradient = dy / (double)dx;

            int step = 1;
            double xi = x0;
            double yi = y0;

            if (Math.Abs(gradient) > 1)
            {
                gradient = 1 / gradient;

                if (gradient < 0)
                {
                    xi = x1;
                    step = -1;
                    Swap(ref y0, ref y1);
                }

                for (yi = y0; yi <= y1; yi += 1)
                {
                    int help;

                    if (gradient < 0)
                        help = (int)(255 * (xi - (int)xi));
                    else
                        help = 255 - (int)(255 * (xi - (int)xi));

                    bitmap.SetPixel((int)xi, (int)yi, Color.FromArgb(255 - help, 255 - help, 255 - help));
                    bitmap.SetPixel((int)xi + step, (int)yi, Color.FromArgb(help, help, help));

                    xi += gradient;
                }
            }
            else
            {
                if (gradient < 0)
                    step = -1;

                for (xi = x0; xi <= x1; xi += 1)
                {
                    int help;

                    if (gradient < 0)
                        help = (int)(255 * (yi - (int)yi));
                    else
                        help = 255 - (int)(255 * (yi - (int)yi));

                    bitmap.SetPixel((int)xi, (int)yi, Color.FromArgb(255 - help, 255 - help, 255 - help));
                    bitmap.SetPixel((int)xi, (int)yi + step, Color.FromArgb(help, help, help));

                    yi += gradient;
                }
            }
        }

        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
