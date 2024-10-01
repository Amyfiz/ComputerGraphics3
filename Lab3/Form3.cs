using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace graph3_3
{
    public partial class Form3 : Form
    {
        private List<Point> trianglePoints = new List<Point>();
        private Color color1 = Color.Red;
        private Color color2 = Color.Green;
        private Color color3 = Color.Blue;
        public Form3()
        {
            InitializeComponent();
            panel1.BackColor = color1;
            panel2.BackColor = color2;
            panel3.BackColor = color3;
            pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (trianglePoints.Count < 3)
            {
                trianglePoints.Add(e.Location);
                pictureBox1.Invalidate();
            }
            //MessageBox.Show(e.Location.ToString());
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (trianglePoints.Count == 3)
            {
                // Задаем цвета для вершин
                Color c1 = color1;
                Color c2 = color2;
                Color c3 = color3;
                e.Graphics.DrawLine(Pens.Black, trianglePoints[0], trianglePoints[1]);
                e.Graphics.DrawLine(Pens.Black, trianglePoints[1], trianglePoints[2]);
                e.Graphics.DrawLine(Pens.Black, trianglePoints[0], trianglePoints[2]);
                // Рисуем треугольник с градиентным окрашиванием
                DrawGradientTriangle(e.Graphics, trianglePoints[0], c1, trianglePoints[1], c2, trianglePoints[2], c3);
            }
        }

        private void DrawGradientTriangle(Graphics g, Point p1, Color c1, Point p2, Color c2, Point p3, Color c3)
        {
            // Сортируем вершины по y-координате
            if (p2.Y < p1.Y) { SwapPoint(ref p1, ref p2); SwapColor(ref c1, ref c2); }
            if (p3.Y < p2.Y) { SwapPoint(ref p2, ref p3); SwapColor(ref c2, ref c3); }
            if (p3.Y < p1.Y) { SwapPoint(ref p1, ref p3); SwapColor(ref c1, ref c3); }
            // Растеризация треугольника
            for (int y = p1.Y; y <= p3.Y; y++)
            {
                if (y < p2.Y)
                {
                    DrawScanline(g, y, p1, c1, p3, c3, p1, c1, p2, c2);
                }
                else
                {
                    DrawScanline(g, y, p1, c1, p3, c3, p2, c2, p3, c3);
                }
            }
        }
        private void DrawScanline(Graphics g, int y, Point pa, Color ca, Point pb, Color cb, Point pc, Color cc, Point pd, Color cd)
        {
            float gradient1 = pa.Y != pb.Y ? (float)(y - pa.Y) / (pb.Y - pa.Y) : 1;
            float gradient2 = pc.Y != pd.Y ? (float)(y - pc.Y) / (pd.Y - pc.Y) : 1;


            int x1 = InterpolatePoint(pa, pb, y);
            int x2 = InterpolatePoint(pc, pd, y);

            Color c1 = InterpolateColor(ca, cb, pa.Y, pb.Y, y);
            Color c2 = InterpolateColor(cc, cd, pc.Y, pd.Y, y);

            if (x1 > x2) { Swap(ref x1, ref x2); SwapColor(ref c1, ref c2); }

            for (int x = x1; x <= x2; x++)
            {
                Color color = InterpolateColor(c1, c2, x1, x2, x);
                g.FillRectangle(new SolidBrush(color), x, y, 1, 1);
            }
        }

        private int InterpolatePoint(Point p1, Point p2, int y)
        {
            if (p1.Y == p2.Y) return p1.X;
            return p1.X + (y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y);
        }

        private Color InterpolateColor(Color c1, Color c2, int y1, int y2, int y)
        {
            float t = (float)(y - y1) / (y2 - y1);
            int r = Clamp((int)(c1.R + t * (c2.R - c1.R)), 0, 255);
            int g = Clamp((int)(c1.G + t * (c2.G - c1.G)), 0, 255);
            int b = Clamp((int)(c1.B + t * (c2.B - c1.B)), 0, 255);
            return Color.FromArgb(r, g, b);
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
        private void SwapPoint(ref Point p1, ref Point p2)
        {
            Point temp = p1;
            p1 = p2;
            p2 = temp;
        }
        private void SwapColor(ref Color c1, ref Color c2)
        {
            Color temp = c1;
            c1 = c2;
            c2 = temp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    color1 = colorDialog.Color;
                    panel1.BackColor = color1;
                    pictureBox1.Invalidate();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    color2 = colorDialog.Color;
                    panel2.BackColor = color2;
                    pictureBox1.Invalidate();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    color3 = colorDialog.Color;
                    panel3.BackColor = color3;
                    pictureBox1.Invalidate();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            try
            {
                int x1 = int.Parse(textBox1.Text);
                int y1 = int.Parse(textBox2.Text);
                int x2 = int.Parse(textBox3.Text);
                int y2 = int.Parse(textBox4.Text);
                int x3 = int.Parse(textBox5.Text);
                int y3 = int.Parse(textBox6.Text);

                // Проверка допустимого диапазона координат
                if (x1 < 0 || x1 > pictureBox1.Width || y1 < 0 || y1 > pictureBox1.Height ||
                    x2 < 0 || x2 > pictureBox1.Width || y2 < 0 || y2 > pictureBox1.Height ||
                    x3 < 0 || x3 > pictureBox1.Width || y3 < 0 || y3 > pictureBox1.Height)
                {
                    MessageBox.Show("Coordinates must be within the bounds of the PictureBox.");
                    return;
                }
                Point p1 = new Point(int.Parse(textBox1.Text), int.Parse(textBox2.Text));
                Point p2 = new Point(int.Parse(textBox3.Text), int.Parse(textBox4.Text));
                Point p3 = new Point(int.Parse(textBox5.Text), int.Parse(textBox6.Text));


                trianglePoints.Clear();
                trianglePoints.Add(p1);
                trianglePoints.Add(p2);
                trianglePoints.Add(p3);
                pictureBox1.Invalidate(); // Перерисовываем PictureBox

            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid integer coordinates.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            trianglePoints.Clear();
            pictureBox1.Invalidate();
        }
    }
}
