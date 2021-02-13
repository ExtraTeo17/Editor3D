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
        private const bool SHOULD_RENDER_LINES = true;
        private const int FRAMES_PER_SECOND = 30;
        private const Shading shading = Shading.Gourand;

        private Color[] colors = { Color.Red, Color.Blue, Color.Green };
        private int currentColorIndex = 0;
        private Bitmap bitmap;
        private List<Camera> cameras = new List<Camera>();
        private List<Light> lights = new List<Light>();
        private int currentCameraIndex;
        private double i = 0;
        private Color currentColor = Color.Black;
        private double[,] zBuffor;

        public EditorForm()
        {
            InitializeComponent();
            PrepareCameras();
            PrepareLights();
            RenderGraphicsPeriodically();
            //RenderGraphics();
        }

        private void PrepareCameras()
        {
            Vector cameraPosition = new Vector(20, 20, 10, 1);
            Vector observedPosition = new Vector(0, 0, 0, 1);
            double nearPlane = 1; // 15
            double farPlane = 100; // 45
            double fieldOfView = Math.PI / 4;
            double aspect = pictureBox1.Width / pictureBox1.Height;
            cameras.Add(new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect));
            currentCameraIndex = 0;
        }

        private void PrepareLights()
        {
            lights.Add(new Light(Color.White, Color.White, new Vector(17, 18, 10, 1)));
        }

        private void PrepareBitmap()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;
            zBuffor = new double[pictureBox1.Width, pictureBox1.Height];
            //for (int i = 0; i < pictureBox1.Width; ++i)
            //    for (int j = 0; j < pictureBox1.Height; ++j)
            //        zBuffor[i, j] = double.MinValue;
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
            RenderGraphics();
        }

        private void RenderGraphics()
        {
            PrepareBitmap();
            //RenderCuboid(3, 4, 5, new Vector(i - 50, i - 30, (i / 3) - 20, 1), Color.Blue);
            PipelineInfo info = GeneratePipelineInfo();
            RenderCuboid(0.4, 0.4, 0.4, new Vector(0, 0, 10, 1), Color.Yellow, info);
            RenderCuboid(5, 5, 5, new Vector(-30 + i, 0, -5, 1), Color.Black, info);
            RenderBall(5, new Vector(-30 + i, 0, -5, 1), Color.Black, info);
            i += 0.2;
            pictureBox1.Refresh();
            // TODO: Add other objects
        }

        private void RenderBall(double radius, Vector position, Color color, PipelineInfo info)
        {
            Ball ball = new Ball(radius, position, color);
            ball.Render(this, info);
        }

        private void RenderCuboid(double x, double y, double z, Vector position, Color color, PipelineInfo info)
        {
            Cuboid cuboid = new Cuboid(x, y, z, position, color);
            cuboid.Render(this, info);
        }

        private PipelineInfo GeneratePipelineInfo()
        {
            Camera currentCamera = cameras[currentCameraIndex];
            return new PipelineInfo(currentCamera.GetViewMatrix(),
                currentCamera.GetProjectionMatrix(), bitmap.Width, bitmap.Height,
                currentCamera.GetForwardDirection(), SHOULD_RENDER_LINES, lights,
                currentCamera.GetPosition());
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

        public void Display(int x, int y, double z, Color color)
        {
            if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
            {
                if (z >= zBuffor[x, y])
                {
                    bitmap.SetPixel(x, y, color); // TODO: X and Z seem to be swapped
                    zBuffor[x, y] = z;
                }
            }
        }

        public void SetColor(Color color)
        {
            this.currentColor = color;
        }

        public Shading GetShading()
        {
            return shading;
        }
    }
}
