using Newtonsoft.Json;

namespace SharpRayTracer
{
    class Program
    {
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
            var sceneJson = File.ReadAllText(@"scene7_squashed_rotated_sphere.json");
            var json = JsonConvert.DeserializeObject<dynamic>(sceneJson);
            Scene scene = new Scene(json);

            double camPlane = FAR - NEAR;

            var irlDirection = -1 * scene.lightDirection;

            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    Hit hit = new Hit(scene.backgroundColor);
                    var ray = scene.cam.GenerateRay(j / (double)WIDTH, 1 - i / (double)HEIGHT);
                    scene.group.Intersect(ray, 0, ref hit);

                    //Light
                    var finalColor = Color.MultiplyChannels(hit.color, scene.ambient) +
                        Math.Max(irlDirection.Dot(hit.normal),0) * 
                        Color.MultiplyChannels(hit.color,scene.lightColor);


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