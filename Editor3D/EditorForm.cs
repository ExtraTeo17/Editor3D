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
        private const int FRAMES_PER_SECOND = 25;
        private const Shading SHADING = Shading.Flat;

        private Bitmap bitmap;
        private List<Camera> cameras = new List<Camera>();
        private List<Light> lights = new List<Light>();
        private int currentCameraIndex;
        private double[,] zBuffor;
        private List<Cuboid> cuboids = new List<Cuboid>();
        private List<Ball> balls = new List<Ball>();
        private PipelineInfo info;

        public EditorForm()
        {
            InitializeComponent();
            PrepareCameras();
            PrepareLights();
            GeneratePipelineInfo();
            PrepareScene();
            RenderGraphics();
            //UpdateScenePeriodically();
        }

        private void PrepareCameras()
        {
            Vector cameraPosition = new Vector(0, 0, 40, 1);
            Vector observedPosition = new Vector(0, 0, 0, 1);
            double nearPlane = 20; // 15
            double farPlane = 80; // 45
            double fieldOfView = Math.PI * 40 / 180;
            double aspect = (double)pictureBox1.Width / (double)pictureBox1.Height;
            cameras.Add(new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect));
            currentCameraIndex = 0;
        }

        private void PrepareLights()
        {
            //lights.Add(new Light(Color.White, Color.White, new Vector(20, 20, 20, 1)));
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
            for (int i = 0; i < pictureBox1.Width; ++i)
                for (int j = 0; j < pictureBox1.Height; ++j)
                    zBuffor[i, j] = 1;
        }

        private void UpdateScenePeriodically()
        {
            Timer timer = new Timer();
            int millisecondsInOneSecond = 1000;
            timer.Interval = millisecondsInOneSecond / FRAMES_PER_SECOND;
            timer.Tick += UpdateScene;
            timer.Start();
        }

        private void UpdateScene(object sender, EventArgs e)
        {
            //balls[0].Translate(0.1, 0, 0);
            //balls[1].Translate(-0.1, 0, 0);
            //cuboids[0].Translate(0, 1, 0);
            RenderGraphics();
        }

        private void RenderGraphics()
        {
            PrepareBitmap();
            foreach (Ball ball in balls)
            {
                ball.Render(this, info);
            }
            foreach (Cuboid cuboid in cuboids)
            {
                cuboid.Render(this, info);
            }
            // TODO: Add other objects
            pictureBox1.Refresh();
        }

        private void PrepareScene()
        {
            //AddBall(3, Color.Red, 11, 0, 10);
            //AddBall(5, Color.Red, -15, 0, -5);
            AddBall(10, Color.Cyan, -20, 0, -20);
            //AddCuboid(12, 3, 24, Color.Pink, 0, 0, 0);
            //PrepareRoomWalls();
            //AddCuboid(100, 100, 100, Color.Red, 100, 0, 100);
        }

        private void PrepareRoomWalls()
        {
            //AddCuboid(1, 200, 300, Color.Cyan, -1, 0, 0);
            //AddCuboid(300, 200, 1, Color.Cyan, 0, 0, -1);
            AddCuboid(250, 1, 250, Color.Brown, 0, -1, 0);
            //AddCuboid(300, 1, 300, Color.Gray, 0, 200, 0);
        }

        private void AddBall(double radius, Color color,
            double transX, double transY, double transZ)
        {
            Ball ball = new Ball(radius, 8, 12, color);
            ball.Translate(transX, transY, transZ);
            balls.Add(ball);
        }

        private void AddCuboid(double x, double y, double z, Color color,
            double transX, double transY, double transZ)
        {
            Cuboid cuboid = new Cuboid(x, y, z, color);
            cuboid.Translate(transX, transY, transZ);
            cuboids.Add(cuboid);
        }

        private void GeneratePipelineInfo()
        {
            Camera currentCamera = cameras[currentCameraIndex];
            info = new PipelineInfo(currentCamera.GetViewMatrix(),
                currentCamera.GetProjectionMatrix(), pictureBox1.Width, pictureBox1.Height,
                currentCamera.GetForwardDirection(), SHOULD_RENDER_LINES, lights,
                currentCamera.GetPosition());
        }

        public void Display(int x, int y, double z, Color color)
        {
            if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
            {
                if (z <= zBuffor[x, y] + 0.01)
                {
                    bitmap.SetPixel(x, y, color);
                    zBuffor[x, y] = z;
                }
            }
        }

        public Shading GetShading()
        {
            return SHADING;
        }
    }
}
