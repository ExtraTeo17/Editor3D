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
        public static double NEAR_PLANE = 23;
        public static double FAR_PLANE = 74;

        private const bool SHOULD_RENDER_LINES = true;
        private const int FRAMES_PER_SECOND = 20;
        private Shading SHADING = Shading.Phong;
        private bool IS_FOG = false;
        private Camera CURRENT_CAMERA;

        private Bitmap bitmap;
        private List<Light> lights = new List<Light>();
        private double[,] zBuffor;
        private List<Cuboid> cuboids = new List<Cuboid>();
        private List<Ball> balls = new List<Ball>();
        private double sign = 1;
        private double spotSign = 1;
        private Camera staticCamera, spyingCamera, movingCamera;
        private Light light, spotlight;

        public EditorForm()
        {
            Debug();
            //Release();
        }

        private void Debug()
        {
            InitializeComponent();
            PrepareScene();
            PrepareCameras();
            PrepareLights();
            //RenderGraphics();
            UpdateScenePeriodically();
        }

        private void Release()
        {
            InitializeComponent();
            PrepareTrackCameras();
            PrepareTrackLights();
            PrepareTrackScene();
            RenderGraphics();
            //UpdateTrackScenePeriodically();
        }

        private void PrepareTrackLights()
        {
        }

        private void PrepareTrackCameras()
        {
            PrepareTrackStaticCamera();
            PrepareTrackSpyingCamera();
            PrepareTrackMovingCamera();
            CURRENT_CAMERA = staticCamera;
        }

        private void PrepareTrackMovingCamera()
        {
            //throw new NotImplementedException();
        }

        private void PrepareTrackSpyingCamera()
        {
            //throw new NotImplementedException();
        }

        private void PrepareTrackStaticCamera()
        {
            //Vector cameraPosition = new Vector(600, 500, 900, 1);
            Vector cameraPosition = new Vector(500, 800, 1000, 1);
            Vector observedPosition = new Vector(0, 0, 0, 1);
            double nearPlane = 1; // 15
            double farPlane = 4000; // 45
            double fieldOfView = Math.PI * 45 / 180;
            double aspect = (double)pictureBox1.Width / (double)pictureBox1.Height;
            staticCamera = new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect);
        }

        private void PrepareCameras()
        {
            PrepareStaticCamera();
            PrepareSpyingCamera();
            PrepareMovingCamera();
            CURRENT_CAMERA = staticCamera;
        }

        private void PrepareMovingCamera()
        {
            Vector observedPosition = balls[0].GetWorldPosition();
            Vector cameraPosition = observedPosition.Translate(new Vector(0, 0, 40, 1));
            double nearPlane = NEAR_PLANE; // 15
            double farPlane = FAR_PLANE; // 45
            double fieldOfView = Math.PI * 45 / 180;
            double aspect = (double)pictureBox1.Width / (double)pictureBox1.Height;
            movingCamera = new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect);
        }

        private void PrepareSpyingCamera()
        {
            Vector cameraPosition = new Vector(0, 0, 0, 1);
            Vector observedPosition = balls[0].GetWorldPosition();
            double nearPlane = NEAR_PLANE; // 15
            double farPlane = FAR_PLANE; // 45
            double fieldOfView = Math.PI * 45 / 180;
            double aspect = (double)pictureBox1.Width / (double)pictureBox1.Height;
            spyingCamera = new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect);
        }

        private void PrepareStaticCamera()
        {
            Vector cameraPosition = new Vector(0, 0, 0, 1);
            Vector observedPosition = new Vector(0, 0, -40, 1);
            double nearPlane = NEAR_PLANE; // 15
            double farPlane = FAR_PLANE; // 45
            double fieldOfView = Math.PI * 45 / 180;
            double aspect = (double)pictureBox1.Width / (double)pictureBox1.Height;
            staticCamera = new Camera(cameraPosition, observedPosition,
                nearPlane, farPlane, fieldOfView, aspect);
        }

        private void PrepareLights()
        {
            Vector lightPos = new Vector(-20, -6, -25, 1);
            AddLight(Color.White, lightPos);
            Vector spotlightPos = new Vector(20, 6, -25, 1);
            AddSpotlight(Color.White, spotlightPos, spotlightPos.DirectionTo(new Vector(0, 0, -40, 1)).Normalize(),
                0.96, 1);
        }

        private void AddLight(Color color, Vector vector)
        {
            AddBall(1, Color.Yellow, vector.x, vector.y, vector.z);
            this.light = new Light(color, color, vector, false, balls.Last());
            lights.Add(light);
        }

        private void AddSpotlight(Color color, Vector vector, Vector spotlightDirection,
            double spotlightRange, double spotlightExponent)
        {
            AddBall(1, Color.Red, vector.x, vector.y, vector.z);
            this.spotlight = new Light(color, color, vector, true, balls.Last(), spotlightDirection,
                spotlightRange, spotlightExponent);
            lights.Add(spotlight);
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

        private void UpdateTrackScenePeriodically()
        {
            Timer timer = new Timer();
            int millisecondsInOneSecond = 1000;
            timer.Interval = millisecondsInOneSecond / FRAMES_PER_SECOND;
            timer.Tick += UpdateTrackScene;
            timer.Start();
        }

        private void UpdateScene(object sender, EventArgs e)
        {
            balls[0].Translate(0.5 * sign, 0, 0);
            if (balls[0].GetWorldPosition().x > 20)
                sign = -1;
            if (balls[0].GetWorldPosition().x < -20)
                sign = 1;
            balls[0].Rotate(1, Axis.Y);
            if (spotlight.lightBall.GetWorldPosition().x < 14)
                spotSign = -1;
            if (spotlight.lightBall.GetWorldPosition().x > 20)
                spotSign = 1;
            spotlight.Translate(-0.5 * spotSign, -0.5 * spotSign, 0);
            UpdateCameras();
            RenderGraphics();
        }

        private void UpdateTrackScene(object sender, EventArgs e)
        {
            UpdateCameras();
            RenderGraphics();
        }

        private void UpdateCameras()
        {
            staticCamera.UpdateAspect((double)pictureBox1.Width / (double)pictureBox1.Height);
            spyingCamera.UpdateAspect((double)pictureBox1.Width / (double)pictureBox1.Height);
            spyingCamera.UpdateObservedPoint(balls[0].GetWorldPosition());
            movingCamera.UpdateAspect((double)pictureBox1.Width / (double)pictureBox1.Height);
            Vector movingCameraObservedPoint = balls[0].GetWorldPosition();
            movingCamera.UpdateObservedPoint(balls[0].GetWorldPosition());
            movingCamera.UpdateCameraPosition(movingCameraObservedPoint.Translate(new Vector(0, 0, 40, 1)));
        }

        private void RenderGraphics()
        {
            PrepareBitmap();
            foreach (Ball ball in balls)
            {
                ball.Render(this, GeneratePipelineInfo());
            }
            foreach (Cuboid cuboid in cuboids)
            {
                cuboid.Render(this, GeneratePipelineInfo());
            }
            pictureBox1.Refresh();
        }

        private void PrepareScene()
        {
            AddBall(10, Color.Green, -20, 0, -40);
            //AddCuboid(10, 10, 10, Color.Green, -20, -10, -40);
            for (int i = -20; i <= 20; i += 4)
                AddCuboid(2, 2, 2, Color.Cyan, i, 12, -40);
            for (int i = -20, j = 0; i <= 20; i += 4, j += 2)
                AddCuboid(2, 2, 2, Color.Red, i, 4, -52 - j);
            for (int i = -20; i <= 20; i += 4)
                AddCuboid(2, 2, 2, Color.Orchid, i, -4, -28);
        }

        private void PrepareTrackScene()
        {
            PrepareTrack();
        }

        public bool IsFog()
        {
            return IS_FOG;
        }

        private void PrepareTrack()
        {
            Color trackColor = Color.Cyan;
            /*AddCuboid(100, 10, 300, trackColor, -50, 0, -150);
            AddCuboid(100, 10, 300, trackColor, -50, 0, 150);
            AddCuboid(300, 10, 100, trackColor, 120.71068, 0, 520.71068);*/
            double xDiff = 92;
            double zDiff = 220;
            int degreeDiff = -45;
            double trackWidth = 300, trackBreadth = 100, trackThickness = 10;

            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, 0, 0, 0);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, 0, 0, trackWidth);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, xDiff, 0, trackWidth + zDiff, degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, xDiff + zDiff, 0, trackWidth + zDiff + xDiff, 2 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, xDiff + 2 * zDiff, 0, trackWidth + zDiff, 3 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, 2 * xDiff + 2 * zDiff, 0, trackWidth, 4 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, xDiff + 2 * zDiff, 0, trackWidth - zDiff, 5 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, Color.Pink, xDiff + zDiff + 20, 80, 0, 6 * degreeDiff, Axis.Y);
            cuboids.Last().Rotate(-30, Axis.Z);

            AddCuboid(trackBreadth, trackThickness, 400, Color.Pink, 0, 155, 0, 2 * degreeDiff, Axis.Y);

            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, 0, 0, 0);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, 0, 0, -trackWidth);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, -xDiff, 0, -(trackWidth + zDiff), degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, -(xDiff + zDiff), 0, -(trackWidth + zDiff + xDiff), 2 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, -(xDiff + 2 * zDiff), 0, -(trackWidth + zDiff), 3 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, -(2 * xDiff + 2 * zDiff), 0, -trackWidth, 4 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, trackColor, -(xDiff + 2 * zDiff), 0, -(trackWidth - zDiff), 5 * degreeDiff, Axis.Y);
            AddCuboid(trackBreadth, trackThickness, trackWidth, Color.Pink, -(xDiff + zDiff + 20), 80, 0, 6 * degreeDiff, Axis.Y);
            cuboids.Last().Rotate(30, Axis.Z);
        }

        private void PrepareRoomWalls()
        {
            AddCuboid(1, 200, 300, Color.Cyan, -1, 0, 0);
            AddCuboid(300, 200, 1, Color.Cyan, 0, 0, -1);
            AddCuboid(300, 1, 300, Color.Brown, 0, -1, 0);
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

        private void AddCuboid(double x, double y, double z, Color color,
            double transX, double transY, double transZ,
            int degrees, Axis axis)
        {
            Cuboid cuboid = new Cuboid(x, y, z, color);
            cuboid.Rotate(degrees, axis);
            cuboid.Translate(transX, transY, transZ);
            cuboids.Add(cuboid);
        }

        private PipelineInfo GeneratePipelineInfo()
        {
            return new PipelineInfo(CURRENT_CAMERA.GetViewMatrix(),
                CURRENT_CAMERA.GetProjectionMatrix(), pictureBox1.Width, pictureBox1.Height,
                CURRENT_CAMERA.GetForwardDirection(), SHOULD_RENDER_LINES, lights,
                CURRENT_CAMERA.GetPosition());
        }

        public void Display(int x, int y, double z, Color color)
        {
            if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
            {
                if (z <= zBuffor[x, y])
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.SHADING = Shading.Flat;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.CURRENT_CAMERA = staticCamera;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.CURRENT_CAMERA = spyingCamera;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.CURRENT_CAMERA = movingCamera;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                light.on = true;
                light.lightBall.visible = true;
            }
            else
            {
                light.on = false;
                light.lightBall.visible = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            spotlight.RotateSpotlight(5, Axis.Y);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            spotlight.RotateSpotlight(5, Axis.X);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            spotlight.RotateSpotlight(-5, Axis.X);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            spotlight.RotateSpotlight(-5, Axis.Y);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                spotSign = 0;
            }
            else
            {
                spotSign = -1;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                IS_FOG = true;
            }
            else
            {
                IS_FOG = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                spotlight.on = true;
                spotlight.lightBall.visible = true;
            }
            else
            {
                spotlight.on = false;
                spotlight.lightBall.visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.SHADING = Shading.Gourand;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.SHADING = Shading.Phong;
        }
    }
}
