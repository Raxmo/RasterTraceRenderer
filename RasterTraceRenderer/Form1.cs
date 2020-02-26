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

		static byte[,,] backbuffer;

		Random rand = new Random();

		public struct Tri
		{
			private Utils.Vec[] verts;
			public Utils.Vec this[int i]
			{
				get
				{
					return verts[i];
				}
				set
				{
					verts[i] = value;
				}
			}
			public void INIT()
			{
				verts = new Utils.Vec[3];
			}

			public void FillBottom()
			{
				Utils.Vec p1 = this[0];
				Utils.Vec p2 = this[1];
				Utils.Vec p3 = this[2];

				Utils.Vec t = p3;

				if(p3.y < p2.y)
				{
					p3 = p2;
					p2 = t;
				}
				if(p2.y < p1.y)
				{
					t = p2;
					p2 = p1;
					p1 = t;
				}
				if(p3.y < p2.y)
				{
					t = p3;
					p3 = p2;
					p2 = t;
				}

				double invslope1 = (p2.x - p1.x) / (p2.y - p1.y);
				double invslope2 = (p3.x - p1.x) / (p3.y - p1.y);
				
				double curx1 = p1.x;
				double curx2 = p1.x;

				double dy = p2.y - p1.y;

				for (int scanlineY =(int)p1.y; scanlineY <= (int)p2.y; scanlineY++)
				{
					double ay = (scanlineY - p1.y) / dy;

					double z1 = (1 - ay) * p1.z + ay * p2.z;
					double z2 = (1 - ay) * p1.z + ay * p3.z;

					double dx = curx2 - curx1;

					for(int x = (int)curx1; x <= (int)curx2; x++)
					{
						double ax = (x - curx1) / dx;

						byte z = (byte)(255 - ((1 - ax) * z1 + ax * z2));

						backbuffer[x, scanlineY, 0] = z;
						backbuffer[x, scanlineY, 1] = z;
						backbuffer[x, scanlineY, 2] = z;
					}
					curx1 += invslope1;
					curx2 += invslope2;
				}
			}
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

			tester.FillBottom();

			unsafe
			{
				byte* p = (byte*)(void*)Scan0;
				int offset = stride - drawBuffer.Width * 4;
				for (int y = 0; y < h; ++y)
				{
					for (int x = 0; x < w; ++x)
					{
						//Blue
						p[0] = backbuffer[x, y, 2];

						//Green
						p[1] = backbuffer[x, y, 1];

						//Red
						p[2] = backbuffer[x, y, 0];

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
			tester.INIT();
			backbuffer = new byte[img.Width, img.Height, 3];
			tester[0] = new Utils.Vec(new double[] { 20, 160, 0 });
			tester[1] = new Utils.Vec(new double[] { 160, 160, 127 });
			tester[2] = new Utils.Vec(new double[] { 90, 20, 255 });
			GameLoop();
		}
	}
}
