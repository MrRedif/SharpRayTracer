using Newtonsoft.Json;

namespace SharpRayTracer
{
    public class Scene
    {
        public Camera cam;
        public Color backgroundColor;
        public Color ambient;
        public List<Light> lights = new List<Light>();
        public ObjectGroup group;

        public Scene(dynamic s)
        {
            if (s.orthocamera != null) cam = new OrthographicCamera(s.orthocamera);
            else if (s.perspectivecamera != null) cam = new PerspectiveCamera(s.perspectivecamera);

            if(s.materials != null)
            {
                MaterialList.AddNewMaterials(s.materials);
            }

            if (s.background != null)
            {
                backgroundColor = new Color(s.background.color);
                ambient = new Color(s.background.ambient);
            }

            if(s.light != null)
            {
                var lightDirection = new Vector4(s.light.direction);
                var lightColor = new Color(s.light.color);
                lights.Add(new DirectionalLight(lightColor,lightDirection));
            }
            if (s.lights != null)
            {
                foreach (var light in s.lights)
                {
                    var lightDirection = new Vector4(light["directionalLight"]["direction"]);
                    var lightColor = new Color(light["directionalLight"]["color"]);
                    lights.Add(new DirectionalLight(lightColor, lightDirection));
                }
            }

            if (s.group != null)
            {
                group = new ObjectGroup(s.group);
            }
        }
    }


}

