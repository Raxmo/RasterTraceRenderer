using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;

namespace RasterTraceRenderer
{
	public partial class Form1 : Form
	{
		static double FT = 0;
		Stopwatch sw = new Stopwatch();
		static int FPS = 60;
		double deltat = 0;
		bool done = false;
		long tpf = TimeSpan.TicksPerSecond / FPS;

		static byte[,,] backbuffer;

		static Random rand = new Random();

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

			public void Fill()
			{
				Utils.Vec p1 = this[0];
				Utils.Vec p2 = this[1];
				Utils.Vec p3 = this[2];

				Utils.Vec t = p3;

				if (p3.y < p2.y)
				{
					p3 = p2;
					p2 = t;
				}
				if (p2.y < p1.y)
				{
					t = p2;
					p2 = p1;
					p1 = t;
				}
				if (p3.y < p2.y)
				{
					t = p3;
					p3 = p2;
					p2 = t;
				}

				double q = 0;
				double p = 0;

				double dy1 = p2.y - p1.y;
				double dy2 = p3.y - p1.y;
				double dy3 = p3.y - p2.y;

				if (p1.y != p2.y)
				{
					for (int y = (int)p1.y; y <= (int)p2.y; y++)
					{
						double ay1 = (y - p1.y) / dy1;
						double ay2 = (y - p1.y) / dy2;

						double x1 = (1 - ay1) * p1.x + ay1 * p2.x;
						double x2 = (1 - ay2) * p1.x + ay2 * p3.x;

						q = Math.Min(x1, x2);
						p = Math.Max(x1, x2);

						x1 = q;
						x2 = p;
						
						double z1 = (1 - ay1) * p1.z + ay1 * p2.z;
						double z2 = (1 - ay2) * p1.z + ay2 * p3.z;

						double dx = x2 - x1;

						for (int x = (int)x1; x <= (int)x2; x++)
						{
							double ax = (x - x1) * (1 / dx);

							double z = 255 - ((1 - ax) * z1 + ax * z2);

							backbuffer[x, y, 0] = (byte)z;
							backbuffer[x, y, 1] = (byte)z;
							backbuffer[x, y, 2] = (byte)z;
						}
					}
				}
				if (p2.y != p3.y)
				{
					for (int y = (int)p2.y; y <= (int)p3.y; y++)
					{
						double ay1 = (y - p2.y) / dy3;
						double ay2 = (y - p1.y) / dy2;

						double x1 = (1 - ay1) * p2.x + ay1 * p3.x;
						double x2 = (1 - ay2) * p1.x + ay2 * p3.x;

						q = Math.Min(x1, x2);
						p = Math.Max(x1, x2);

						x1 = q;
						x2 = p;

						double z1 = (1 - ay1) * p2.z + ay1 * p3.z;
						double z2 = (1 - ay2) * p1.z + ay2 * p3.z;

						double dx = x2 - x1;

						for (int x = (int)x1; x <= (int)x2; x++)
						{
							double ax = (x - x1) * (1 / dx);

							double z = 255 - ((1 - ax) * z1 + ax * z2);

							backbuffer[x, y, 0] = (byte)z;
							backbuffer[x, y, 1] = (byte)z;
							backbuffer[x, y, 2] = (byte)z;
						}
					}
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
			BitmapData dat = drawBuffer.LockBits(new Rectangle(0, 0, drawBuffer.Width, drawBuffer.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
			IntPtr Scan0 = dat.Scan0;
			int stride = dat.Stride;
			int h = drawBuffer.Height;
			int w = drawBuffer.Width;

			tester.Fill();

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
			tester[0] = new Utils.Vec(new double[] { 20, 20, 0 });
			tester[1] = new Utils.Vec(new double[] { 170, 70, 127 });
			tester[2] = new Utils.Vec(new double[] { 70, 170, 255 });
			GameLoop();
		}
	}
}
