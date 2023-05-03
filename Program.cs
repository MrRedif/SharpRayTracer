using Newtonsoft.Json;

namespace SharpRayTracer
{
    public class RayTracer
    {
        public static Color TraceRay(Ray ray,int bounces,double weight,int indexOfRefraction,Hit hit)
        {
            Color col = new Color();
            foreach (var light in Program.scene.lights)
            {
                var worldPos = hit.t * ray.direction + Program.scene.cam.center;
                Ray posToLight = new Ray(worldPos, -1 *((DirectionalLight)light).direction);
                Hit lHit = new Hit(PhongMaterial.DEF_MAT);
                Program.scene.group.Intersect(posToLight, 0.00001, ref lHit);
                
                if(!lHit.isHitObject)
                    col += hit.material.Shade(ray, hit, light);
            }
            Color ambient =
                    Color.MultiplyChannels(Program.scene.ambient, hit.material.diffuseColor);
            return (col + ambient).Clamp01Channels();
        }
    }

    class Program
    {
        public static Scene scene;
        const string OUTPUT_PATH = @"output6.ppm";
        const string OUTPUT_PATH_DEPTH = @"outputDepth.ppm";
        static string OUT_TEXT_HEADER = "P3\n" + WIDTH + " " + HEIGHT + "\n255\n";
        const int WIDTH = 175, HEIGHT = 175;
        const double FAR = 12, NEAR = 7.5;
        static string outText = OUT_TEXT_HEADER;
        static string outTextDepth = OUT_TEXT_HEADER;
        public static void Main(String[] args)
        {

            //Json
            var sceneJson = File.ReadAllText(@"scene2_plane_sphere.json");
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
                    var finalColor = RayTracer.TraceRay(ray, 3, 0.5, 1, hit);


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
            }
            //Save
            File.WriteAllText(OUTPUT_PATH, outText);
            File.WriteAllText(OUTPUT_PATH_DEPTH, outTextDepth);


        }
    }
}