using Newtonsoft.Json;

namespace SharpRayTracer
{
    public class Scene
    {
        public Camera cam;
        public Color backgroundColor;
        public Color ambient;

        public Vector4 lightDirection;
        public Color lightColor;

        public ObjectGroup group;

        public Scene(dynamic s)
        {
            if (s.orthocamera != null) cam = new OrthographicCamera(s.orthocamera);
            else if (s.perspectivecamera != null) cam = new PerspectiveCamera(s.perspectivecamera);

            if (s.background != null)
            {
                backgroundColor = new Color(s.background.color);
                ambient = new Color(s.background.ambient);
            }

            if(s.light != null)
            {
                lightDirection = new Vector4(s.light.direction);
                lightColor = new Color(s.light.color);
            }

            if (s.group != null)
            {
                group = new ObjectGroup(s.group);
            }
        }
    }


}

