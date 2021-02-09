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
        private const int FRAMES_PER_SECOND = 3;
        private Color[] colors = { Color.Red, Color.Blue, Color.Green };
        private int currentColorIndex = 0;
        private Bitmap bitmap;

        public EditorForm()
        {
            InitializeComponent();
            PrepareBitmap();
            RenderGraphicsPeriodically();
        }

        private void PrepareBitmap()
        {
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
            RenderCuboid();
        }

        private void RenderCuboid()
        {
            Cuboid cuboid = new Cuboid(3, 4, 5, new Vector(0, 0, 0, 1));
            cuboid.Render();
            pictureBox1.Refresh();
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
            throw new NotImplementedException();
        }
    }
}
