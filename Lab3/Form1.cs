using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Lab3
{
    public partial class Form1 : Form
    {
        static string filename = "../../images/ФРУКТЫ.jpg";
        static Bitmap _bmFillPicture = new Bitmap(filename);

        Bitmap _bm;
        Graphics _g;
        List<Point> _points;

        Pen _pen = new Pen(Color.Black);
        Pen _fillPen = new Pen(Color.Green);
        Color edgeColor = Color.Cyan;

        private Color _penColor;
        private Color _fillColor;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _bm = new Bitmap(pictureBox1.Size.Width - 1, pictureBox1.Size.Height - 1);
            pictureBox1.Image = _bm;
            Clear();

            _points = new List<Point>();
            _g = Graphics.FromImage(_bm);

            pictureBox1.MouseDown += OnMouseDown;
            pictureBox1.MouseMove += OnMouseMove;

            _penColor = _pen.Color;
            _fillColor = _fillPen.Color;
        }

        void ColorFill(object sender, MouseEventArgs e)
        {
            ColorFill_(e.X, e.Y);
        }

        private bool ColorsEqual(Color c1, Color c2) => c1.ToArgb() == c2.ToArgb();

        void ColorFill_(int x, int y)
        {
            if (x >= 0 && x < _bm.Width && y >= 0 && y < _bm.Height &&
                !ColorsEqual(_bm.GetPixel(x, y), _fillColor) && !ColorsEqual(_bm.GetPixel(x, y), _penColor))
            {
                Color oldColor = _bm.GetPixel(x, y);
                int leftX = x;
                while (leftX > 0 && ColorsEqual(_bm.GetPixel(leftX, y), oldColor)) leftX -= 1;
                int rightX = x;
                while (rightX < _bm.Width - 1 && ColorsEqual(_bm.GetPixel(rightX, y), oldColor)) rightX += 1;

                if (leftX == 0) leftX = -1;
                if (rightX == _bm.Width - 1) rightX = _bm.Width;

                _g.DrawLine(_fillPen, leftX + 1, y, rightX - 1, y);
                pictureBox1.Image = _bm;
                for (int i = leftX + 1; i <= rightX - 1; i++)
                    ColorFill_(i, y + 1);
                for (int i = leftX + 1; i <= rightX - 1; i++)
                    ColorFill_(i, y - 1);
            }
        }

        void PictureFill(object sender, MouseEventArgs e)
        {
            PictureFill_(e.X, e.Y, e.X, e.Y);
        }

        Point GetLocalPoint(int x, int y, int originX, int originY)
        {
            // 1. Вычисляем смещение относительно центральной точки изображения-заполнителя
            int xOffset = x - originX;
            int yOffset = y - originY;

            // 2. Прибавляем половину ширины и высоты изображения-заполнителя,
            // чтобы получить координаты относительно центра
            int localX = _bmFillPicture.Width / 2 + xOffset;
            int localY = _bmFillPicture.Height / 2 + yOffset;

            // 3. Обрабатываем переполнение по краям изображения-заполнителя
            // с помощью оператора modulo (%)
            localX %= _bmFillPicture.Width;
            localY %= _bmFillPicture.Height;

            // 4. Исправляем отрицательные координаты, чтобы они были в диапазоне 
            // от 0 до ширины/высоты изображения-заполнителя
            while (localX < 0) localX += _bmFillPicture.Width;
            while (localY < 0) localY += _bmFillPicture.Height;

            // 5. Возвращаем полученные координаты
            return new Point(localX, localY);
        }

        void PictureFill_(int x, int y, int originX, int originY)
        {
            if (x >= 0 && x < _bm.Width && y >= 0 && y < _bm.Height &&
                !ColorsEqual(_bm.GetPixel(x, y), _penColor))
            {
                Color oldColor = _bm.GetPixel(x, y);
                Point temp = GetLocalPoint(x, y, originX, originY);
                if (ColorsEqual(_bmFillPicture.GetPixel(temp.X, temp.Y), oldColor)) return;

                int leftX = x;
                while (leftX > 0 && ColorsEqual(_bm.GetPixel(leftX, y), oldColor)) leftX -= 1;
                int rightX = x;
                while (rightX < _bm.Width - 1 && ColorsEqual(_bm.GetPixel(rightX, y), oldColor)) rightX += 1;

                if (leftX == 0) leftX = -1;
                if (rightX == _bm.Width - 1) rightX = _bm.Width;

                for (int i = leftX + 1; i <= rightX - 1; i++)
                {
                    Point localP = GetLocalPoint(i, y, originX, originY);
                    _bm.SetPixel(i, y, _bmFillPicture.GetPixel(localP.X, localP.Y));
                }

                pictureBox1.Image = _bm;

                for (int i = leftX + 1; i <= rightX - 1; i++)
                    PictureFill_(i, y + 1, originX, originY);
                for (int i = leftX + 1; i <= rightX - 1; i++)
                    PictureFill_(i, y - 1, originX, originY);
            }
        }

        Point GetNeighboor(int x, int y, int dir)
        {
            switch (dir)
            {
                case 0: return new Point(x, y - 1);
                case 1: return new Point(x + 1, y - 1);
                case 2: return new Point(x + 1, y);
                case 3: return new Point(x + 1, y + 1);
                case 4: return new Point(x, y + 1);
                case 5: return new Point(x - 1, y + 1);
                case 6: return new Point(x - 1, y);
                case 7: return new Point(x - 1, y - 1);
                default: return new Point(x, y);
            }
        }

        void SelectEdge(object sender, MouseEventArgs e)
        {
            LinkedList<Point> edge = SelectEdge_(e.X, e.Y);
            if (edge.Count > 0)
            {
                foreach (Point p in edge)
                    _bm.SetPixel(p.X, p.Y, edgeColor);
                pictureBox1.Image = _bm;
            }
        }

        LinkedList<Point> SelectEdge_(int startX, int startY)
        {
            LinkedList<Point> edge = new LinkedList<Point>();
            edge.AddLast(new Point(startX, startY));
            Color c = _bm.GetPixel(startX, startY);

            int curX = startX;
            int curY = startY;
            int dir = 4;

            while (true)
            {
                dir += 2;
                if (dir > 7) dir -= 8;
                Point p;

                for (int i = 0; i < 8; i++)
                {
                    p = GetNeighboor(curX, curY, dir);
                    if (!(p.X >= 0 && p.X < _bm.Width && p.Y >= 0 && p.Y < _bm.Height)) continue;
                    if (ColorsEqual(_bm.GetPixel(p.X, p.Y), c)) goto a;
                    dir--;
                    if (dir < 0) dir += 8;
                }
                return new LinkedList<Point>();
            a:
                if (p.X == startX && p.Y == startY) break;

                edge.AddLast(p);
                curX = p.X;
                curY = p.Y;
            }
            return edge;
        }

        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _points.Clear();
            _points.Add(e.Location);
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _points.Add(e.Location);

            if (_points.Count < 2)
                return;

            _g.DrawLines(_pen, _points.ToArray());
            pictureBox1.Image = _bm;
        }

        private void Clear()
        {
            var _g = Graphics.FromImage(pictureBox1.Image);
            _g.Clear(pictureBox1.BackColor);
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
        }
    

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) //Рисовать
            {
                pictureBox1.MouseDown -= ColorFill;
                pictureBox1.MouseDown -= PictureFill;
                pictureBox1.MouseDown -= SelectEdge;

                pictureBox1.MouseDown += OnMouseDown;
                pictureBox1.MouseMove += OnMouseMove;
            }
            else if (radioButton2.Checked) //Залить цветом
            {
                pictureBox1.MouseDown -= OnMouseDown;
                pictureBox1.MouseMove -= OnMouseMove;
                pictureBox1.MouseDown -= PictureFill;
                pictureBox1.MouseDown -= SelectEdge;

                pictureBox1.MouseDown += ColorFill;
            }
            else if (radioButton3.Checked) //Залить картинкой
            {
                pictureBox1.MouseDown -= OnMouseDown;
                pictureBox1.MouseMove -= OnMouseMove;
                pictureBox1.MouseDown -= ColorFill;
                pictureBox1.MouseDown -= SelectEdge;

                pictureBox1.MouseDown += PictureFill;
            }
            else if (radioButton4.Checked) //Выделить границу
            {
                pictureBox1.MouseDown -= OnMouseDown;
                pictureBox1.MouseMove -= OnMouseMove;
                pictureBox1.MouseDown -= ColorFill;
                pictureBox1.MouseDown -= PictureFill;

                pictureBox1.MouseDown += SelectEdge;
            }
        }
    }
}
