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
    public partial class EditorForm : Form
    {
        private const int FRAMES_PER_SECOND = 3;
        private Color[] colors = { Color.Red, Color.Blue, Color.Green };
        private int currentColorIndex = 0;
        private int bitmapWidth;
        private int bitmapHeight;
        private Bitmap bitmap;

        public EditorForm()
        {
            InitializeComponent();
            PrepareBitmap();
            RenderGraphicsPeriodically();
        }

        private void PrepareBitmap()
        {
            bitmapWidth = pictureBox1.Width;
            bitmapHeight = pictureBox1.Height;
            bitmap = new Bitmap(bitmapWidth, bitmapHeight);
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
            for (int i = 0; i < bitmapWidth; ++i)
            {
                for (int j = 0; j < bitmapHeight; ++j)
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
    }
}
