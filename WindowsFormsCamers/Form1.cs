using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Drawing.Drawing2D;

namespace WindowsFormsCamers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        MJPEGStream stream;
        private void Form1_Load(object sender, EventArgs e)
        {
            stream = new MJPEGStream("http://192.168.1.196:4747/mjpegfeed?596x385"); //из телефона
            stream.NewFrame += stream_NewFrame;
            stream.Start();
        }

        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //создание стрима на весь экран
            Bitmap bmp = createBitmap(sender, eventArgs);

            
            //выделение желтого цвета
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor = new RGB(224, 209, 3); //желтый цвет
            filter.Radius = 100;
            filter.ApplyInPlace(bmp);

            //Класс считает и извлекает отдельные объекты на изображениях
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(bmp);

            Rectangle[] rects = blobCounter.GetObjectsRectangles(); //центр обьекта

            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0) //обводить в квадарат
                {
                    //DrowPicture(rects, bmp);  // + pictureBox1.Image = bmp;

                    //  OR

                    MoveCoursor(recs.X, recs.Y); // + Form1_Shown -> Application.Exit();
                }
           
               
               
            }
            //pictureBox1.Image = bmp; //рисовать
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Application.Exit();
        }


        Bitmap createBitmap(object sender, NewFrameEventArgs eventArgs)
        {
            int koef = 10;
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipX); //зеркально
            bmp = new Bitmap(bmp, new Size(SystemInformation.VirtualScreen.Width / koef, SystemInformation.VirtualScreen.Height / koef));
            using (bmp)
            {
                var bmp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (var g = Graphics.FromImage(bmp2))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(bmp, new Rectangle(Point.Empty, bmp2.Size));
                    return bmp2;
                }
            }
        }

        void DrowPicture(Rectangle[] rectangles, Bitmap bitmap)
        {
            Rectangle objectRect = rectangles[0];
            Graphics g = Graphics.FromImage(bitmap);
            using (Pen pen = new Pen(Color.FromArgb(255, 0, 0), 5))
            {
                g.DrawRectangle(pen, objectRect);
            }
            g.Dispose();
        }

        void MoveCoursor(int X, int Y)
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new System.Drawing.Point(X, Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
        }
        
    }
}