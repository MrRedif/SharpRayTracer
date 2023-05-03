using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRayTracer
{
    public class Ray
    {
        public Vector4 origin;
        public Vector4 direction;

        public Ray()
        {
            origin = new Vector4();
            direction = new Vector4();
        }

        public Ray(Vector4 origin, Vector4 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }

    public class Hit
    {
        public Material material;
        public double t = double.MaxValue;
        public Vector4 normal = new Vector4();
        public bool isHitObject;
        public Hit(Material material)
        {
            this.material = material;
        }
    }
}
