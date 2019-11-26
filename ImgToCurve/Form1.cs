using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgToCurve
{
    public partial class Form1 : Form
    {
        Color[] canvas = new Color[] { Color.FromArgb(255,255,255), Color.FromArgb(192, 192, 192) };   // Цвета 
        public Form1()
        {
            InitializeComponent();
        }

        private void Browse_btn_Click(object sender, EventArgs e)
        {
            int Rows = -1;
            int Cols = -1;
            string text = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    pictureBox1.Image = Image.FromFile(file);
                    /*
                    string[] lines = text.Split('\n');
                    Rows = lines.Count();
                    if (Rows == 0) throw new FileLoadException("File is empty");
                    M_tb.Text = Convert.ToString(Rows);
                    Cols = lines[0].Split(';').Count();
                    N_tb.Text = Convert.ToString(Cols - 1);
                    GetCoordinates(lines, Rows, Cols);
                    */
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        const int range = 120; 

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int step = 10;
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int w = bmp.Width;
            int h = bmp.Height;
            Color[,] pixels = new Color[w, h];
            Color CurveColor = bmp.GetPixel(e.X, e.Y); //отладка
            int ax = e.X - range;
            if (ax < 0)
                ax = 0;
            int bx = e.X + range;
            if (bx > w)
                bx = w;
            int ay = 0;
            int by = 0;
            if ((CurveColor != canvas[0]) && (CurveColor != canvas[1]))
            {
                bool IsCurveStarted = false;
                //bool IsCurveEnded = false;
                List<Point> points = new List<Point>();
                for (int y = h - 1; y > 0; --y) // пробегаем по строкам (снизу вверх)
                {
                    var min = -1;
                    var max = -1;
                    //IsCurveEnded = true;   // предполагаем, что достигли конца кривой
                    for (int x = ax; x <= bx; ++x)
                    {
                        var pixel = bmp.GetPixel(x, y) == CurveColor;
                        if (pixel)
                        {
                            IsCurveStarted = true;
                            if (ay == 0)
                            {
                                ay = y; // y координата начала нашей кривой
                            }
                            //IsCurveEnded = false;   // опровергаем достижение конца кривой

                            if (min == -1)
                            {
                                max = min = x;
                            }
                            else if (max < x )
                                max = x;
                        }
                    }
                    if ((min != -1) && (max != -1))
                    {
                        /*
                        for (int a = min; a <= max; ++a)
                        {
                            points.Add(new Point(a, y));
                        }
                        */
                        int tmp = (max + min) / 2;
                        points.Add(new Point(tmp, y));
                        
                    }
                    if ((IsCurveStarted) && (bmp.GetPixel(ax ,y) == Color.FromArgb(160, 160, 160)))
                    {
                        by = y;
                        break;
                    }
                    /*
                    if ((IsCurveStarted) && (IsCurveEnded))
                    {
                        by = y;
                        break;
                    }
                    */
                }

                BuildCurve(points);

                /*
                    int i = e.Y; i < bmp.Height; ++i)
                {
                    int leftBound_X = e.X - step;
                    if (leftBound_X < 0)
                        leftBound_X = 0;
                    int rightBound_X = e.X - step;
                    if (rightBound_X > bmp.Width)
                        rightBound_X = bmp.Width;
                    for (int j = leftBound_X; j < rightBound_X; ++j)
                    {
                        
                    }

                }
                */
            }

            /*
            for (int i = x; i < bmp.Width; ++i)
            {
                
                for (int j = y-step; j < y+step; ++j)
                {
                    Color tmp = bmp.GetPixel(i, j);
                    pixels[i, j] = tmp;
                }
            }
            */
        }
        private void BuildCurve(List<Point> points)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int i = 0; i < points.Count; ++i)
            {
                bmp.SetPixel(points[i].X, points[i].Y, Color.Black);
            }
            Result res = new Result(); //Создаем экземпляр формы
            res.WindowState = FormWindowState.Maximized;
            image = new Bitmap(bmp);
            res.ShowDialog(); //Или так

            
        }
        public static Bitmap image;
    }
}
