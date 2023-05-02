using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRayTracer
{
    public class Matrix4x4
    {
        public double[,] mdata = new double[4, 4];

        public Matrix4x4(double[,] m) => mdata = m;
        public Matrix4x4()
        {

        }

        public static Matrix4x4 Translate(double x, double y = 0, double z = 0)
        {
            Matrix4x4 m = new Matrix4x4();
            m.mdata[0, 0] = 1;
            m.mdata[1, 1] = 1;
            m.mdata[2, 2] = 1;
            m.mdata[3, 3] = 1;
            m.mdata[0, 3] = x;
            m.mdata[1, 3] = y;
            m.mdata[2, 3] = z;
            return m;
        }
        public static Matrix4x4 Scale(double x, double y = 0, double z = 0)
        {
            Matrix4x4 m = new Matrix4x4();
            m.mdata[0, 0] = x;
            m.mdata[1, 1] = y;
            m.mdata[2, 2] = z;
            m.mdata[3, 3] = 1;
            return m;
        }
        public static Matrix4x4 RotateX(double angle)
        {
            Matrix4x4 m = new Matrix4x4();
            double rad = Math.PI / 180 * angle;
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            m.mdata[0, 0] = 1;
            m.mdata[3, 3] = 1;
            m.mdata[1, 1] = c;
            m.mdata[2, 2] = c;
            m.mdata[2, 1] = s;
            m.mdata[1, 2] = -s;
            return m;
        }
        public static Matrix4x4 RotateY(double angle)
        {
            Matrix4x4 m = new Matrix4x4();
            double rad = Math.PI / 180 * angle;
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            m.mdata[0, 0] = c;
            m.mdata[2, 2] = c;
            m.mdata[0, 2] = s;
            m.mdata[2, 0] = -s;
            m.mdata[1, 1] = 1;
            m.mdata[3, 3] = 1;

            return m;
        }
        public static Matrix4x4 RotateZ(double angle)
        {
            Matrix4x4 m = new Matrix4x4();
            double rad = Math.PI / 180 * angle;
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);
            m.mdata[0, 0] = c;
            m.mdata[0, 1] = -s;
            m.mdata[1, 0] = s;
            m.mdata[1, 1] = c;
            m.mdata[2, 2] = 1;
            m.mdata[3, 3] = 1;
            return m;
        }

        public Matrix4x4 Inverse()
        {
            double[,] result = new double[4, 4];

            // calculate the determinant of the matrix
            double det = mdata[0, 0] * (mdata[1, 1] * mdata[2, 2] - mdata[1, 2] * mdata[2, 1])
                - mdata[0, 1] * (mdata[1, 0] * mdata[2, 2] - mdata[1, 2] * mdata[2, 0])
                + mdata[0, 2] * (mdata[1, 0] * mdata[2, 1] - mdata[1, 1] * mdata[2, 0]);

            // check if the determinant is zero
            if (det == 0)
                throw new Exception("Matrix is not invertible");

            // calculate the inverse matrix
            double invDet = 1.0 / det;
            result[0, 0] = (mdata[1, 1] * mdata[2, 2] - mdata[1, 2] * mdata[2, 1]) * invDet;
            result[0, 1] = -(mdata[0, 1] * mdata[2, 2] - mdata[0, 2] * mdata[2, 1]) * invDet;
            result[0, 2] = (mdata[0, 1] * mdata[1, 2] - mdata[0, 2] * mdata[1, 1]) * invDet;
            result[0, 3] = 0;

            result[1, 0] = -(mdata[1, 0] * mdata[2, 2] - mdata[1, 2] * mdata[2, 0]) * invDet;
            result[1, 1] = (mdata[0, 0] * mdata[2, 2] - mdata[0, 2] * mdata[2, 0]) * invDet;
            result[1, 2] = -(mdata[0, 0] * mdata[1, 2] - mdata[0, 2] * mdata[1, 0]) * invDet;
            result[1, 3] = 0;

            result[2, 0] = (mdata[1, 0] * mdata[2, 1] - mdata[1, 1] * mdata[2, 0]) * invDet;
            result[2, 1] = -(mdata[0, 0] * mdata[2, 1] - mdata[0, 1] * mdata[2, 0]) * invDet;
            result[2, 2] = (mdata[0, 0] * mdata[1, 1] - mdata[0, 1] * mdata[1, 0]) * invDet;
            result[2, 3] = 0;

            result[3, 0] = -(mdata[3, 0] * result[0, 0] + mdata[3, 1] * result[1, 0] + mdata[3, 2] * result[2, 0]);
            result[3, 1] = -(mdata[3, 0] * result[0, 1] + mdata[3, 1] * result[1, 1] + mdata[3, 2] * result[2, 1]);
            result[3, 2] = -(mdata[3, 0] * result[0, 2] + mdata[3, 1] * result[1, 2] + mdata[3, 2] * result[2, 2]);
            result[3, 3] = 1;

            return new Matrix4x4(result);
        }


        public override string ToString()
        {
            var str = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    str += mdata[i, j] + " ";
                }
                str += "\n";
            }
            return str;
        }

        #region Operators
        public static Vector4 operator *(Matrix4x4 m, Vector4 v)
        {
            double x = v.x * m.mdata[0, 0] + v.y * m.mdata[0, 1] + v.z * m.mdata[0, 2] + v.w * m.mdata[0, 3];
            double y = v.x * m.mdata[1, 0] + v.y * m.mdata[1, 1] + v.z * m.mdata[1, 2] + v.w * m.mdata[1, 3];
            double z = v.x * m.mdata[2, 0] + v.y * m.mdata[2, 1] + v.z * m.mdata[2, 2] + v.w * m.mdata[2, 3];
            double w = v.x * m.mdata[3, 0] + v.y * m.mdata[3, 1] + v.z * m.mdata[3, 2] + v.w * m.mdata[3, 3];

            return new Vector4(x, y, z, w);
        }
        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 res = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res.mdata[i, j] =
                        a.mdata[i, 0] * b.mdata[0, j] +
                        a.mdata[i, 1] * b.mdata[1, j] +
                        a.mdata[i, 2] * b.mdata[2, j] +
                        a.mdata[i, 3] * b.mdata[3, j];
                }
            }
            return res;
        }
        #endregion

    }
}
