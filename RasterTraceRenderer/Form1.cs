using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Threading;

namespace RasterTraceRenderer
{
	public partial class Form1 : Form
	{
		long FT = 0;
		Stopwatch sw = new Stopwatch();
		int FPS = 60;
		long deltat = 0;
		bool done = false;

		public void UpDt()
		{

		}

		public void Draw()
		{
			for(int y = 0; y < img.Height; y++)
			{
				for (int x = 0; x < img.Width; x++)
				{
					int r = (int)(x * FT / 500.0) % 256;
					int b = (int)(y * FT / 750.0) % 256;
					int g = (int)(FT / 1000.0) % 256;

					lock (img.Image)
					{
						((Bitmap)img.Image).SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
					}
				}
			}
		}
		
		public async void GameLoop()
		{
			img.Image = new Bitmap(img.Width, img.Height);

			while (!done)
			{
				deltat = sw.ElapsedMilliseconds;
				sw.Restart();
				FT += deltat;
				Console.Write($"\rFPS: {1000 / Math.Max(deltat, 1)} Delta t: {deltat}");

				UpDt();

				Draw();

				lock (img)
				{
					img.Refresh();
				}
				Thread.Sleep(Math.Max(0, (int)((1000 / FPS) - sw.ElapsedMilliseconds)));
			}
		}

		public Form1()
		{
			InitializeComponent();
		}

		public void REfRESH()
		{
			img.Refresh();
		}

		private async void Form1_Load(object sender, EventArgs e)
		{
			await Task.Run(() => GameLoop());
		}
	}
}
