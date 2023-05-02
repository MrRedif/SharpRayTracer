using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRayTracer
{
    public abstract class Material
    {
        public Color diffuseColor;
        public Color reflectiveColor;
        public Color transparentColor;
        public int indexOfRefraction;

        public Material(Color diffuseColor, Color reflectiveColor, Color transparentColor, int indexOfRefraction)
        {
            this.diffuseColor = diffuseColor;
            this.reflectiveColor = reflectiveColor;
            this.transparentColor = transparentColor;
            this.indexOfRefraction = indexOfRefraction;
        }

        public virtual Color Shade(Ray ray,Hit hit,Light light)
        {
            return diffuseColor;
        } 
    }

    public class PhongMaterial : Material
    {
        public Color specularColor;
        public double exponent;
        public PhongMaterial(Color diffuseColor, Color reflectiveColor,
            Color transparentColor, int indexOfRefraction, Color specularColor, double exponent)
            : base(diffuseColor, reflectiveColor, transparentColor, indexOfRefraction)
        {
            this.specularColor = specularColor;
            this.exponent = exponent;
        }

        public override Color Shade(Ray ray, Hit hit, Light light)
        {
            return base.Shade(ray, hit, light);
        }
    }
}
