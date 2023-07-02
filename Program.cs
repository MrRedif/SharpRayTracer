using Newtonsoft.Json;

namespace SharpRayTracer
{
    public class RayTracer
    {
        public static Color TraceRay(Ray ray, int bounces, double weight, double indexOfRefraction, Hit hit)
        {
            Color col = new Color();
            foreach (var light in Program.scene.lights)
            {
                var worldPos = hit.t * ray.direction + ray.origin;
                int inObject = hit.normal.Dot(ray.direction) > 0 ? -1 : 1;

                //Reflections
                if (hit.material.reflectiveColor != Color.black && bounces > 0)
                {
                    var dir = Vector4.MirrorVector(hit.normal, ray.direction);
                    Ray bounceRay = new Ray(worldPos, dir);
                    Hit bHit = new Hit(PhongMaterial.DEF_MAT);
                    Program.scene.group.Intersect(bounceRay, 0.00001, ref bHit);
                    
                    col += Color.MultiplyChannels(hit.material.reflectiveColor,
                        TraceRay(bounceRay, bounces - 1,
                        weight * hit.material.reflectiveColor.Magnitude / Math.Sqrt(3),
                        inObject == 1 ? 1 : hit.material.indexOfRefraction, bHit) * weight);
                }

                //Shadows
                Ray posToLight = new Ray(worldPos, -1 * ((DirectionalLight)light).direction);
                Hit lHit = new Hit(PhongMaterial.DEF_MAT);
                Program.scene.group.Intersect(posToLight, 0.00001, ref lHit);

                if (hit.material.transparentColor != Color.black && bounces > 0)
                {
                    //Refraction
                    var refractionDir =
                        Vector4.TransmittedVector(inObject * hit.normal, ray.direction, indexOfRefraction, hit.material.indexOfRefraction);
                    Ray refractRay = new Ray(worldPos, refractionDir);
                    Hit bHit = new Hit(PhongMaterial.DEF_MAT);
                    Program.scene.group.Intersect(refractRay, 0.00001, ref bHit);

                    col += Color.MultiplyChannels(hit.material.transparentColor,
                        TraceRay(refractRay, bounces - 1,
                        weight * hit.material.transparentColor.Magnitude / Math.Sqrt(3),
                        inObject == 1 ? 1 : hit.material.indexOfRefraction, bHit) * weight);
                }


                if (!lHit.isHitObject)
                {
                    //Final
                    col += Color.MultiplyChannels(hit.material.Shade(ray, hit, light), hit.material.transparentColor);
                }
                



            }
            Color ambient =
                    Color.MultiplyChannels(Program.scene.ambient, hit.material.diffuseColor);
            
            return (col + ambient).Clamp01Channels();
        }
    }

    class Program
    {
        public static Scene scene;
        const string OUTPUT_PATH = @"out-final2.ppm";
        const string OUTPUT_PATH_DEPTH = @"outputDepth.ppm";
        static string OUT_TEXT_HEADER = "P3\n" + WIDTH + " " + HEIGHT + "\n255\n";
        const int WIDTH = 300, HEIGHT = 300;
        const double FAR = 8, NEAR = 3;
        static string outText = OUT_TEXT_HEADER;
        static string outTextDepth = OUT_TEXT_HEADER;
        public static void Main(String[] args)
        {

            //Json
            var sceneJson = File.ReadAllText(@"scene4_reflective_sphere.json");
            var json = JsonConvert.DeserializeObject<dynamic>(sceneJson);
            scene = new Scene(json);
            double camPlane = FAR - NEAR;

            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    Hit hit = new Hit(PhongMaterial.DEF_MAT);
                    var ray = scene.cam.GenerateRay(j / (double)WIDTH, 1 - i / (double)HEIGHT);
                    scene.group.Intersect(ray, 0.00001, ref hit);

                    //Light
                    var finalColor = hit.isHitObject ? RayTracer.TraceRay(ray, 3, 0.8, 1, hit) : scene.backgroundColor;


                    finalColor = finalColor * 255;
                    finalColor.r = Math.Clamp(finalColor.r, 0, 255);
                    finalColor.g = Math.Clamp(finalColor.g, 0, 255);
                    finalColor.b = Math.Clamp(finalColor.b, 0, 255);

                    outText += finalColor.ToString() + "\n";

                    int depth = 0;
                    if (hit.t < FAR)
                    {
                        depth = (int)((FAR - hit.t) / camPlane * 255);
                    }


                    outTextDepth += depth + " " + depth + " " + depth + "\n";
                }
                Console.WriteLine("%" + ((float)i / HEIGHT * 100f));
            }
            //Save
            File.WriteAllText(OUTPUT_PATH, outText);
            File.WriteAllText(OUTPUT_PATH_DEPTH, outTextDepth);


        }
    }
}