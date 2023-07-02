using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpRayTracer
{
    public class Vector4
    {
        public double x, y, z, w;

        public double Magnitude => Math.Sqrt(x * x + y * y + z * z);
        public Vector4 Normalized
        {
            get
            {
                var mag = Magnitude;
                return new Vector4(x / mag, y / mag, z / mag);
            }
        }

        public static Vector4 Up = new Vector4(0, 1, 0, 0);
        public static Vector4 Right = new Vector4(1, 0, 0, 0);
        public static Vector4 Foward = new Vector4(0, 0, 1, 0);
        public static Vector4 Zero = new Vector4();

        public Vector4(double x, double y, double z, double w = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4()
        {
            x = y = z = w = 0;
        }

        //Json Constructer
        public Vector4(JArray jArray) : this(jArray.ToObject<double[]>()!) { }


        public Vector4(double[] d)
        {
            x = d[0];
            y = d[1];
            z = d[2];
        }
        public Vector4(int[] d)
        {
            x = d[0];
            y = d[1];
            z = d[2];
        }

        public Vector4 Cross(Vector4 other)
        {
            return new Vector4(
            y * other.z - z * other.y,
            z * other.x - x * other.z,
            x * other.y - y * other.x);
        }

        public double Dot(Vector4 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public static Vector4 Normalize(Vector4 v)
        {
            var mag = v.Magnitude;
            return new Vector4(v.x / mag, v.y / mag, v.z / mag);
        }

        public static Vector4 MirrorVector(Vector4 normal, Vector4 incoming)
        {
            return incoming - 2 * normal.Dot(incoming) * normal;
        }

        public static Vector4 TransmittedVector(Vector4 normal, Vector4 incoming, double index_i, double index_t)
        {
            double nr = index_i / index_t;
            double ndi = normal.Dot(incoming);
            double sqr = 1 - nr*nr*(1 - ndi*ndi);

            return ((nr * ndi - sqr) * normal - nr * incoming).Normalized;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2} w:{3}", x, y, z, w);
        }

        #region Operators
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector4 operator *(int a, Vector4 b)
        {
            return new Vector4(a * b.x, a * b.y, a * b.z);
        }
        public static Vector4 operator *(Vector4 b, int a)
        {
            return new Vector4(a * b.x, a * b.y, a * b.z);
        }
        public static Vector4 operator *(Vector4 b, double a)
        {
            return new Vector4(a * b.x, a * b.y, a * b.z);
        }
        public static Vector4 operator *(double a, Vector4 b)
        {
            return new Vector4(a * b.x, a * b.y, a * b.z);
        }
        public static Vector4 operator /(Vector4 b, double a)
        {
            return new Vector4(b.x / a, b.y / a, b.z / a);
        }
        public static explicit operator Color(Vector4 v)  // explicit byte to digit conversion operator
        {
            Color c = new Color(v);
            return c;
        }
        #endregion

    }

    public class Color
    {
        public double r, g, b;
        public readonly static Color black = new Color(0, 0, 0);

        public double Magnitude => Math.Sqrt(r * r + g * g + b * b);
        public Color(double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public Color(Vector4 v)
        {
            r = v.x;
            g = v.y;
            b = v.z;
        }
        public Color(double[] d)
        {
            r = d[0];
            g = d[1];
            b = d[2];
        }
        public Color(int[] d)
        {
            r = d[0];
            g = d[1];
            b = d[2];
        }

        public Color()
        {
            r = g = b = 0;
        }
        public Color(JArray jArray) : this(jArray.ToObject<double[]>()!) { }
        public static Color MultiplyChannels(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b);
        }

        public static explicit operator Vector4(Color c)  // explicit byte to digit conversion operator
        {
            Vector4 v = new Vector4(c.r, c.g, c.b);
            return v;
        }
        public static Color operator *(Color c, double a)
        {
            return new Color(c.r * a, c.g * a, c.b * a);
        }
        public static Color operator *(double a, Color c)
        {
            return new Color(c.r * a, c.g * a, c.b * a);
        }
        public static Color operator *(Color c, int a)
        {
            return new Color(c.r * a, c.g * a, c.b * a);
        }
        public static Color operator *(int a, Color c)
        {
            return new Color(c.r * a, c.g * a, c.b * a);
        }
        public static Color operator +(Color a, Color b)
        {
            return new Color(a.r + b.r, a.g + b.g, a.b + b.b);
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b;
        }

        public static bool operator !=(Color a, Color b)
        {
            return a.r != b.r || a.g != b.g || a.b != b.b;
        }

        public override string ToString()
        {
            return Math.Floor(r) + " " + Math.Floor(g) + " " + Math.Floor(b);
        }

        public Color Clamp01Channels()
        {
            return new Color(Math.Min(r, 1), Math.Min(g, 1), Math.Min(b, 1));
        }
    }
}
