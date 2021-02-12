using Editor3D.Shapes;
using Editor3D.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor3D
{
    public partial class EditorForm : Form, IDisplayer
    {
        private const int FRAMES_PER_SECOND = 5;
        private Color[] colors = { Color.Red, Color.Blue, Color.Green };
        private int currentColorIndex = 0;
        private Bitmap bitmap;
        private List<Camera> cameras = new List<Camera>();
        private int currentCameraIndex;
        private int i = 0;

        public EditorForm()
        {
            InitializeComponent();
            PrepareCameras();
            RenderGraphicsPeriodically();
            //RenderCuboid();
        }

        private void PrepareCameras()
        {
            Vector cameraPosition = new Vector(20, 20, 10, 1);
            Vector observedPosition = new Vector(0, 0, 0, 1);
            double nearPlane = 15;
            double farPlane = 45;
            double fieldOfView = Math.PI / 4;
            double aspect = pictureBox1.Width / pictureBox1.Height;
            cameras.Add(new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect));
            currentCameraIndex = 0;
        }

        private void PrepareBitmap()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;
        }

        private void RenderGraphicsPeriodically()
        {
            Timer timer = new Timer();
            int millisecondsInOneSecond = 1000;
            timer.Interval = millisecondsInOneSecond / FRAMES_PER_SECOND;
            timer.Tick += RenderGraphics;
            timer.Start();
        }

        private void RenderGraphics(object sender, EventArgs e)
        {
            PrepareBitmap();
            RenderCuboid(i++);
            // TODO: Add other objects
        }

        private void RenderCuboid(int i)
        {
            Cuboid cuboid = new Cuboid(3, 4, 5, new Vector(i, 0, 0, 1));
            cuboid.Render(this, GeneratePipelineInfo());
            pictureBox1.Refresh();
        }

        private PipelineInfo GeneratePipelineInfo()
        {
            Camera currentCamera = cameras[currentCameraIndex];
            return new PipelineInfo(currentCamera.GetViewMatrix(),
                currentCamera.GetProjectionMatrix(), bitmap.Width, bitmap.Height);
        }

        /**
         * UNUSED
         */
        private void RenderColor(object sender, EventArgs e)
        {
            for (int i = 0; i < bitmap.Width; ++i)
            {
                for (int j = 0; j < bitmap.Height; ++j)
                {
                    bitmap.SetPixel(i, j, colors[currentColorIndex]);
                }
            }
            ++currentColorIndex;
            if (currentColorIndex > 2)
            {
                currentColorIndex = 0;
            }
            pictureBox1.Refresh();
        }

        public void Display(double x, double y)
        {
            bitmap.SetPixel((int)Math.Round(x), -(int)Math.Round(y), Color.Blue); // TODO: Don't know why Y negated
            bitmap.SetPixel((int)Math.Round(x) + 1, -(int)Math.Round(y), Color.Blue);
            bitmap.SetPixel((int)Math.Round(x) - 1, -(int)Math.Round(y), Color.Blue);
            bitmap.SetPixel((int)Math.Round(x), -(int)Math.Round(y) + 1, Color.Blue);
            bitmap.SetPixel((int)Math.Round(x), -(int)Math.Round(y) - 1, Color.Blue);
            bitmap.SetPixel((int)Math.Round(x) + 1, -(int)Math.Round(y) + 1, Color.Blue);
            bitmap.SetPixel((int)Math.Round(x) + 1, -(int)Math.Round(y) - 1, Color.Blue);
            bitmap.SetPixel((int)Math.Round(x) - 1, -(int)Math.Round(y) + 1, Color.Blue);
            bitmap.SetPixel((int)Math.Round(x) - 1, -(int)Math.Round(y) - 1, Color.Blue);
        }
    }
}
