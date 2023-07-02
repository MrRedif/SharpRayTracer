using Newtonsoft.Json.Linq;
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
        public double indexOfRefraction;

        public Material(Color diffuseColor, Color reflectiveColor, Color transparentColor, double indexOfRefraction)
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
        public static PhongMaterial DEF_MAT = new PhongMaterial(new Color(), new Color(), new Color(), 0, new Color(), 0);

        public Color specularColor;
        public double exponent;
        public PhongMaterial(Color diffuseColor, Color reflectiveColor,
            Color transparentColor, double indexOfRefraction, Color specularColor, double exponent)
            : base(diffuseColor, reflectiveColor, transparentColor, indexOfRefraction)
        {
            this.specularColor = specularColor;
            this.exponent = exponent;
        }

        public override Color Shade(Ray ray, Hit hit, Light light)
        {
            DirectionalLight dirLight = (DirectionalLight)light;
            Vector4 reflection = hit.normal * 2 * hit.normal.Dot(dirLight.direction) - dirLight.direction;

            Color diffuse =
                Color.MultiplyChannels(hit.material.diffuseColor,dirLight.color) * Math.Max((-1 * dirLight.direction).Dot(hit.normal), 0);
            
            Color specular =
                 Color.MultiplyChannels(hit.material.reflectiveColor,dirLight.color) * Math.Pow(Math.Max(ray.direction.Dot(reflection), 0),exponent);
            
            return (diffuse + specular).Clamp01Channels();
        }
    }

    public static class MaterialList
    {
        public static List<Material> Materials = new List<Material>();

        public static void AddNewMaterials(dynamic d)
        {
            foreach (JObject item in d)
            {
                var parentProp = (JProperty)(item.First!);
                string name = parentProp!.Name;

                switch (name)
                {
                    case "phongMaterial":
                        Color specularCol = new Color();
                        Color reflectiveCol = new Color();
                        Color transparentCol = new Color(1,1,1);
                        double ex = 0;
                        double index = 0;
                        if (item["phongMaterial"]["specularColor"] != null)
                            specularCol = new Color(item["phongMaterial"]["specularColor"].Values<double>().ToArray());
                        
                        if (item["phongMaterial"]["reflectiveColor"] != null)
                            reflectiveCol = new Color(item["phongMaterial"]["reflectiveColor"].Values<double>().ToArray());

                        if (item["phongMaterial"]["transparentColor"] != null)
                            transparentCol = new Color(item["phongMaterial"]["transparentColor"].Values<double>().ToArray());

                        if (item["phongMaterial"]["exponent"] != null)
                            ex = item["phongMaterial"]["exponent"].ToObject<double>();

                        if (item["phongMaterial"]["indexOfRefraction"] != null)
                            index = item["phongMaterial"]["indexOfRefraction"].ToObject<double>();

                        var diffuseCol = new Color(item["phongMaterial"]["diffuseColor"].Values<double>().ToArray());

                        Materials.Add(new PhongMaterial(diffuseCol,reflectiveCol,transparentCol,index,specularCol,ex));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
