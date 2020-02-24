using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RasterTraceRenderer
{
	public class Utils
	{
		public static void Log(dynamic input)
		{
			Console.WriteLine(input);
		}

		public struct Vec
		{
			private double[] _v;

			public double this[int i]
			{
				get
				{
					return _v[i];
				}
				set
				{
					_v[i] = value;
				}
			}

			public double x
			{
				get
				{
					return _v[0];
				}
				set
				{
					_v[0] = value;
				}
			}
			public double y
			{
				get
				{
					return _v[1];
				}
				set
				{
					_v[1] = value;
				}
			}
			public double z
			{
				get
				{
					return _v[2];
				}
				set
				{
					_v[2] = value;
				}
			}
			public double w
			{
				get
				{
					return _v[3];
				}
				set
				{
					_v[3] = value;
				}
			}
			public double v
			{
				get
				{
					return _v[4];
				}
				set
				{
					_v[4] = value;
				}
			}
			public double u
			{
				get
				{
					return _v[5];
				}
				set
				{
					_v[5] = value;
				}
			}
			public double t
			{
				get
				{
					return _v[6];
				}
				set
				{
					_v[6] = value;
				}
			}

			public int dims
			{
				get
				{
					return _v.Length;
				}
			}
			public string Print
			{
				get
				{
					string output = $"< {_v[0]}";
					for (int i = 1; i < dims; i++)
					{
						output += $" {_v[i]}";
					}
					output += " >";
					return output;
				}
			}
			public double magsqrd
			{
				get
				{
					return this * this;
				}
			}
			public double mag
			{
				get
				{
					return Math.Sqrt(magsqrd);
				}
			}

			public Vec(int dims)
			{
				_v = new double[dims];
			}
			public Vec(double[] set)
			{
				_v = set;
			}

			public static double operator *(Vec a, Vec b)
			{
				double c = 0;
				for (int i = 0; i < a.dims; i++)
				{
					c += a[i] * b[i];
				}
				return c;
			}
			public static Vec operator *(Vec a, double b)
			{
				Vec c = a;
				for (int i = 0; i < c.dims; i++)
				{
					c[i] = a[i] * b;
				}
				return c;
			}
			public static Vec operator *(double a, Vec b)
			{
				return b * a;
			}
			public static Vec operator /(Vec a, double b)
			{
				return a * (1 / b);
			}
			public static Vec operator +(Vec a, Vec b)
			{
				Vec c = a;
				for (int i = 0; i < c.dims; i++)
				{
					c[i] += b[i];
				}
				return c;
			}
			public static Vec operator -(Vec a, Vec b)
			{
				Vec c = a;
				for (int i = 0; i < c.dims; i++)
				{
					c[i] -= b[i];
				}
				return c;
			}

		}
	}
}
