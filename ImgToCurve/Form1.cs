using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ImgToCurve
{
    public partial class Form1 : Form
    {
        Color[] canvas = new Color[] { Color.FromArgb(255, 255, 255), Color.FromArgb(192, 192, 192) };   // Цвета 
        public Form1()
        {
            InitializeComponent();
            MemoBox_tb.Text = "Выберите файл с рисунком для отображения";
        }

        Bitmap SourceImage;

        private void Browse_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    pictureBox1.Image = Image.FromFile(file);
                    SourceImage = new Bitmap(pictureBox1.Image);
                    MemoBox_tb.Text = "Режим просмотра. Щелкните на рисунке по нужной кривой для её оцифровки. После щелчка, оцифрованная кривая будет отображена таким-то цветом.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        const int range = 6;
        List<Point> points = new List<Point>();


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int w = bmp.Width;
            int h = bmp.Height;
            int x = e.X;
            int y = e.Y;
            Color[,] pixels = new Color[w, h];
            Color CurveColor = bmp.GetPixel(x, y); //отладка
            if ((bmp.GetPixel(x, y) == canvas[0]) || (bmp.GetPixel(x, y) == canvas[1]))
            {
                for (int i = e.X - (range / 2); i < e.X + (range / 2); ++i) // если кликнули не по кривой, то ищем точку кривой в области квадрата с размерами - range x range
                {
                    bool flag = false;
                    for (int i2 = e.Y - (range / 2); i2 < e.Y + (range / 2); ++i2)
                    {
                        if ((bmp.GetPixel(i, i2) != canvas[0]) && (bmp.GetPixel(i, i2) != canvas[1]))
                        {
                            x = i;
                            y = i2;
                            CurveColor = bmp.GetPixel(x, y);
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }



            int j = 1;

            bool FoundPointDwn = true;
            bool FoundPointUp = true;

            if ((bmp.GetPixel(x, y) != canvas[0]) && (bmp.GetPixel(x, y) != canvas[1]))
            {
                points.Add(new Point(x, y));
                while (((0 < y - j) && (y + j < h)) && (FoundPointDwn || FoundPointUp)) // пока мы не достигли границ picturebox и на предыдущих горизонталях точки были найдены цикл работает
                {
                    if (FoundPointUp)  // если точки на предыдущей горизонтали не найдены, значит мы достигли верхнего конца кривой
                    {
                        if (bmp.GetPixel(x, y - j) == CurveColor)   // если пиксель "выше" подходящий
                        {
                            points.Add(new Point(x, y - j));
                            FoundPointDwn = true;
                        }
                        else
                        {
                            int i = 1;
                            FoundPointUp = false;
                            while ((0 < x - i) || (x + i < w))  // останавливаемся, когда вышли за обе границы
                            {
                                if ((0 < x - i) && (bmp.GetPixel(x - i, y - j) == CurveColor))   // влево
                                {
                                    FoundPointUp = true;
                                    points.Add(new Point(x - i, y - j));
                                    x = x - i;
                                    break;
                                }

                                if ((x + i < w) && (bmp.GetPixel(x + i, y - j) == CurveColor)) // вправо
                                {
                                    FoundPointUp = true;
                                    points.Add(new Point(x + i, y - j));
                                    x = x + i;
                                    break;
                                }
                                ++i;
                            }
                        }
                    }

                    if (FoundPointDwn)  // если точки на предыдущей горизонтали не найдены, значит мы достигли нижнего конца кривой
                    {
                        if (bmp.GetPixel(x, y + j) == CurveColor)   // если пиксель "ниже" подходящий
                        {
                            points.Add(new Point(x, y + j));
                            FoundPointDwn = true;
                        }
                        else
                        {
                            int i = 1;
                            FoundPointDwn = false;
                            while ((0 < x - i) || (x + i < w))  // останавливаемся, когда вышли за обе границы
                            {

                                if ((0 < x - i) && (bmp.GetPixel(x - i, y + j) == CurveColor))   // влево
                                {
                                    FoundPointDwn = true;
                                    points.Add(new Point(x - i, y + j));
                                    x = x - i;
                                    break;
                                }

                                if ((x + i < w) && (bmp.GetPixel(x + i, y + j) == CurveColor))  // вправо
                                {
                                    FoundPointDwn = true;
                                    points.Add(new Point(x + i, y + j));
                                    x = x + i;
                                    break;
                                }
                                ++i;
                            }
                        }
                    }
                    ++j;
                }
                BuildCurve(points);
            }
        }

        private void BuildCurve(List<Point> points)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int i = 0; i < points.Count; ++i)
            {
                bmp.SetPixel(points[i].X, points[i].Y, Color.Black);
            }
            pictureBox1.Image = bmp;

        }
        public static Bitmap image;

        private void Clear_btn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = SourceImage;
        }
    }
}
