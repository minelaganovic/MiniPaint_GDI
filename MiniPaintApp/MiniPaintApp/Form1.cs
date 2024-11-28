using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MiniPaint
{
    public partial class Form1 : Form
    {
        Cursor bucketCursor;
        Cursor pencilCursor;
        Cursor crossCursor;

        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White, 10);
        int index;
        int x, y, cX, cY, sX, sY;
        ColorDialog cd = new ColorDialog();
        Color new_color;

        public Form1()
        {
            InitializeComponent();
            this.Icon = new Icon("D:\\MiniPaintApp-master\\MiniPaintApp\\MiniPaintApp\\Resources\\imgp.ico");
            this.Width = 920;
            this.Height = 600;
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pictureBox1.Image = bm;

            bucketCursor = Cursors.Hand;
            pencilCursor = Cursors.Default; 
            crossCursor = Cursors.Cross;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            if (index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            if (index == 4)
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }
            if (index == 5)
            {
                g.DrawLine(p, cX, cY, x, y);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py = e.Location;

            cX = e.X;
            cY = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if (index == 1) // Pencil
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2) // Eraser
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            pictureBox1.Refresh();
            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
            this.Cursor = crossCursor;
        }

        private void btn_rect_Click(object sender, EventArgs e)
        {
            index = 4;
            this.Cursor = crossCursor;
        }

        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            index = 3;
            this.Cursor = crossCursor;
        }

        private void button_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            color_show.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
            this.Cursor = pencilCursor;
        }

        static Point set_point(PictureBox pb, Point pt)
        {
            float px = 1f * pb.Image.Width / pb.Width;
            float py = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * px), (int)(pt.Y * py));
        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(color_picker, e.Location);
            color_show.BackColor = ((Bitmap)color_picker.Image).GetPixel(point.X, point.Y);
            new_color = color_show.BackColor;
            p.Color = color_show.BackColor;
        }

        int number;
        private void writetext()
        {
            Random r1 = new Random();
            number = r1.Next(100, 1000);
            var font = new Font("Calibri", 17, FontStyle.Italic, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bm);
            graphics.DrawString("Welcome to MiniPaintApp " + number.ToString(), font, Brushes.Red, new Point(40, 40));
            this.pictureBox1.Image = bm;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writetext();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void validate(Bitmap bm, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            if (x >= 0 && y >= 0 && x < bm.Width && y < bm.Height)
            {
                Color cx = bm.GetPixel(x, y);
                if (cx == old_color)
                {
                    sp.Push(new Point(x, y));
                    bm.SetPixel(x, y, new_color);
                }
            }
        }

        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            if (old_color == new_clr) return;

            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);

            while (pixel.Count > 0)
            {
                Point pt = pixel.Pop();

                validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 7)
            {
                Point point = set_point(pictureBox1, e.Location);
                Fill(bm, point.X, point.Y, new_color);
                pictureBox1.Refresh();
            }
        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 7;
            this.Cursor = bucketCursor;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog();
            save.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*";

            if (save.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), bm.PixelFormat);
                btm.Save(save.FileName, ImageFormat.Jpeg);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pictureBox1.Image = bm;
            index = 0;
        }
    }
}
