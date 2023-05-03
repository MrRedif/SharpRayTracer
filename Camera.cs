using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRayTracer
{
    public abstract class Camera
    {
        public Vector4 center;
        public static double ASPECT_RATIO;
        /// <summary>
        /// Generate ray from camera viewport.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual Ray GenerateRay(double x, double y)
        {
            return new Ray();
        }
    }

    public class OrthographicCamera : Camera
    {
        Vector4 direction;
        Vector4 up;
        double size;
        public OrthographicCamera(Vector4 center, Vector4 dir, Vector4 up, double size = 1)
        {
            this.center = center;
            this.direction = dir;
            this.up = up;
            this.size = size;
        }
        public OrthographicCamera()
        {
            center = new Vector4();
            direction = new Vector4();
            up = new Vector4();
            size = 1;
        }

        public OrthographicCamera(dynamic d)
        {
            center = new Vector4(d.center);
            direction = new Vector4(d.direction);
            up = new Vector4(d.up);
            size = d.size;
        }

        public override Ray GenerateRay(double x, double y)
        {
            Vector4 horizontal = direction.Cross(up);
            Vector4 rayOrigin = center + ((x - 0.5) * horizontal + (y - 0.5) * up) * size;
            return new Ray(rayOrigin, direction);
        }
    }

    public class PerspectiveCamera : Camera
    {
        Vector4 direction;
        Vector4 up;
        double angle;

        public PerspectiveCamera(Vector4 center, Vector4 direction, Vector4 up, double angle)
        {
            this.center = center;
            this.direction = direction;
            this.up = up;
            this.angle = angle;
        }
        public PerspectiveCamera(dynamic d)
        {
            center = new Vector4(d.center);
            direction = new Vector4(d.direction);
            up = new Vector4(d.up);
            angle = d.angle;    
        }

        public override Ray GenerateRay(double x, double y)
        {
            Vector4 horizontal = direction.Cross(up);
            Vector4 q = (1 / Math.Tan(angle / 2 * Math.PI / 180)) * direction + up * (y - 0.5) + horizontal * (x - 0.5); 
            q = q.Normalized;
            return new Ray(center, q);
        }
    }
}
