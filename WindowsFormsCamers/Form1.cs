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

        Bitmap createBitmap(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            bmp = new Bitmap(bmp, new Size(SystemInformation.VirtualScreen.Width/7, SystemInformation.VirtualScreen.Height/7));
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



        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //создание стрима на весь экран
            Bitmap bmp = createBitmap(sender, eventArgs);
           
            //выделение желтого цвета
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor = new RGB(220, 220, 0); //желтый цвет
            filter.Radius = 120;
            filter.ApplyInPlace(bmp);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipX); //зеркально

            BlobCounter blobCounter = new BlobCounter();
            //получение координат
            blobCounter.MinWidth = 15;
            blobCounter.MinHeight = 15;
            blobCounter.FilterBlobs = true;
            blobCounter.ProcessImage(bmp);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0) //обводить в квадарат
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point(recs.X, recs.Y);
                    Cursor.Clip = new Rectangle(this.Location, this.Size);

                    //Rectangle objectRect = rects[0];
                    //Graphics g = Graphics.FromImage(bmp);
                    //using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5))
                    //{
                    //   g.DrawRectangle(pen, objectRect);
                    //}
                   //g.Dispose();
                }
               
               
               
            }
            //pictureBox1.Image = bmp; //рисовать
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}