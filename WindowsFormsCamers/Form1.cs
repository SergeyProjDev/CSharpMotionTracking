using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using AForge.Imaging;

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
            return new Bitmap(bmp, new Size(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height));

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

            //сделать серым для дальнейшей работы
            BitmapData objectsData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            bmp.UnlockBits(objectsData);
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.ProcessImage(grayImage);

            //получение координат
            blobCounter.MinWidth = 50;
            blobCounter.MinHeight = 50;
            blobCounter.FilterBlobs = true;
            blobCounter.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            foreach (Rectangle recs in rects)
            {
                //TODO координатами и мышью
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new System.Drawing.Point(recs.X, recs.Y);
                    Cursor.Clip = new Rectangle(this.Location, this.Size);
                //TODO

                /*
                if (rects.Length > 0) //обводить в квадарат
                {
                    Rectangle objectRect = rects[0];
                    Graphics g = Graphics.FromImage(bmp);
                    using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }
                    g.Dispose();
                }
                */
            }
            pictureBox1.Image = bmp; //рисовать
        }
    }
}
