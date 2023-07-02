using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpRayTracer
{
    public abstract class Object3D
    {
        public Material material;
        public virtual void Intersect(Ray ray, double tmin, ref Hit hit)
        {

        }
    }

    public class Sphere : Object3D
    {
        public double radius;
        public Vector4 center = new Vector4();
        public Sphere(Vector4 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }
        public Sphere(dynamic d)
        {
            center = new Vector4((JArray)d.center);
            radius = d.radius;
            material = MaterialList.Materials[(int)d.material];
        }

        public override void Intersect(Ray ray, double tmin, ref Hit hit)
        {
            Vector4 l = center - ray.origin;
            double tca = l.Dot(ray.direction);
            double d2 = l.Dot(l) - tca * tca;
            if (d2 > radius * radius) return;

            double thc = Math.Sqrt(radius * radius - d2);
            double t0 = tca - thc;
            double t1 = tca + thc;

            double thold = t0;
            if (t0 > t1)
            {
                t0 = t1;
                t1 = thold;
            }

            if (t0 < 0)
            {
                t0 = t1;
                if (t0 < 0) return;
            }

            if (t0 > tmin && hit.t > t0)
            {
                hit.normal = (ray.origin + ray.direction * t0 - center).Normalized;
                hit.material = material;
                hit.t = t0;
                hit.isHitObject = true;
            }


        }
    }

    public class ObjectGroup : Object3D
    {
        public List<Object3D> childs = new List<Object3D>();

        public ObjectGroup(dynamic g)
        {
            childs = new List<Object3D>();
            foreach (JObject item in g)
            {
                var parentProp = (JProperty)(item.First!);
                string name = parentProp!.Name;

                switch (name)
                {
                    case "sphere":
                        childs.Add(new Sphere(item["sphere"]!));
                        break;
                    case "plane":
                        childs.Add(new Plane(item["plane"]!));
                        break;
                    case "triangle":
                        childs.Add(new Triangle(item["triangle"]!));
                        break;
                    case "transform":
                        childs.Add(new Transformation(item["transform"]!));
                        break;
                    default:
                        break;
                }
            }


        }

        public override void Intersect(Ray ray, double tmin, ref Hit hit)
        {
            foreach (var obj in childs)
            {
                obj.Intersect(ray, tmin, ref hit);
            }
        }
    }

    public class Triangle : Object3D
    {
        public Vector4 v1;
        public Vector4 v2;
        public Vector4 v3;

        public override void Intersect(Ray ray, double tmin, ref Hit hit)
        {

            //Find triangle normal
            var a = v2 - v1;
            var b = v3 - v1;
            var N = a.Cross(b);

            //Check paralelism
            double nDotDir = N.Dot(ray.direction);
            if (Math.Abs(nDotDir) < 0.0000001)
            {
                return;
            }

            var d = -N.Dot(v1);
            var t = -(N.Dot(ray.origin) + d) / nDotDir;

            if (t < 0) return;//Behind ray

            Vector4 P = ray.origin + t * ray.direction;//Intersection Point

            Vector4 C;

            //Test for v1
            var edge1 = v2 - v1;
            var vp1 = P - v1;
            C = edge1.Cross(vp1);
            if (N.Dot(C) < 0) return; // P is on the right side

            //Test for v2
            var edge2 = v3 - v2;
            var vp2 = P - v2;
            C = edge2.Cross(vp2);
            if (N.Dot(C) < 0) return; // P is on the right side

            //Test for v3
            var edge3 = v1 - v3;
            var vp3 = P - v3;
            C = edge3.Cross(vp3);
            if (N.Dot(C) < 0) return; // P is on the right side

            if (t > tmin && t < hit.t)
            {
                hit.t = t;
                hit.material = material;
                hit.normal = N;
                hit.isHitObject = true;
            }


        }

        public Triangle(dynamic d)
        {
            v1 = new Vector4((JArray)d.v1);
            v2 = new Vector4((JArray)d.v2);
            v3 = new Vector4((JArray)d.v3);
            material = MaterialList.Materials[(int)d.material];
        }
    }

    public class Plane : Object3D
    {
        Vector4 normal;
        double d;//Offset from origin

        public override void Intersect(Ray ray, double tmin, ref Hit hit)
        {
            var t = -(-d + ray.origin.Dot(normal)) / ray.direction.Dot(normal);
            if (t > tmin && t < hit.t)
            {
                hit.t = t;
                hit.material = material;
                hit.normal = normal;
                hit.isHitObject = true;
            }
        }

        public Plane(dynamic d)
        {
            normal = new Vector4(d.normal);
            this.d = d.offset;
            material = MaterialList.Materials[(int)d.material];
        }
    }

    public class Transformation : Object3D
    {
        public Matrix4x4 m;
        public Matrix4x4 mInverse;
        Object3D obj;

        public override void Intersect(Ray ray, double tmin, ref Hit hit)
        {
            ray.origin = mInverse * ray.origin;
            ray.direction = mInverse * ray.direction;
            obj.Intersect(ray, tmin, ref hit);
        }

        public Transformation(dynamic d)
        {
            m = Matrix4x4.Scale(1, 1, 1);//Identity
            if (d["object"].sphere != null) obj = new Sphere(d["object"].sphere);
            else if (d["object"].plane != null) obj = new Plane(d["object"].plane);
            else if (d["object"].triangle != null) obj = new Triangle(d["object"].triangle);

            foreach (var item in d.transformations)
            {
                if (item.scale != null)
                {
                    double[] v = ((JArray)item.scale).ToObject<double[]>()!;
                    m = m * Matrix4x4.Scale(v[0], v[1], v[2]);
                }else if(item.zrotate != null)
                {
                    m = m * Matrix4x4.RotateZ((double)item.zrotate);
                }
            }

            mInverse = m.Inverse();
        }
    }
}
