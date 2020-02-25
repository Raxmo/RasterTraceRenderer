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
using System.IO;
using System.Drawing.Imaging;

namespace RasterTraceRenderer
{
	public partial class Form1 : Form
	{
		double FT = 0;
		Stopwatch sw = new Stopwatch();
		static int FPS = 60;
		double deltat = 0;
		bool done = false;
		long tpf = TimeSpan.TicksPerSecond / FPS;

		Random rand = new Random();

		public struct Tri
		{
			public Utils.Vec[] verts;
		}

		public struct Mesh
		{
			public Tri[] tris;
		}

		public void Updog()
		{

		}

		Tri tester = new Tri();

		public Bitmap Draw()
		{
			Bitmap drawBuffer = new Bitmap(img.Width, img.Height);
			BitmapData dat = drawBuffer.LockBits(new Rectangle(0, 0, drawBuffer.Width, drawBuffer.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			IntPtr Scan0 = dat.Scan0;
			int stride = dat.Stride;
			int h = drawBuffer.Height;
			int w = drawBuffer.Width;

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;
				int offset = stride - drawBuffer.Width * 4;
				for (int y = 0; y < h; ++y)
				{
					for (int x = 0; x < w; ++x)
					{
						//Blue
						p[0] = (byte)rand.Next(255);

						//Green
						p[1] = (byte)rand.Next(255);

						//Red
						p[2] = (byte)rand.Next(255);

						//Next Pixel
						p += 4;
					}
					p += offset;
				}
			}

			drawBuffer.UnlockBits(dat);
			return drawBuffer;
		}
		
		public async void GameLoop()
		{
			while (!done)
			{
				deltat = (double)(sw.ElapsedTicks) / Stopwatch.Frequency;
				sw.Restart();
				FT += deltat;

				Text = $"FPS: {(int)(1 / Math.Max(0.00001, deltat))}";

				Updog();
				
				img.Image = await Task.Run(() => Draw());
				
				img.Refresh();
				
				//TimeSpan ts = new TimeSpan(Math.Max(0, (tpf - sw.ElapsedTicks)));
				//Thread.Sleep(ts);
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

		private void Form1_Load(object sender, EventArgs e)
		{
			tester.verts = new Utils.Vec[3];
			tester.verts[0] = new Utils.Vec(new double[] { 20, 20 });
			tester.verts[1] = new Utils.Vec(new double[] { 50, 50 });
			tester.verts[2] = new Utils.Vec(new double[] { 50, 20 });
			GameLoop();
		}
	}
}
