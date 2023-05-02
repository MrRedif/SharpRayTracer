using System;
using System.Collections.Generic;

namespace SharpRayTracer
{
    public abstract class Light
    {
        public Color color;

        public Light(Color color)
        {
            this.color = color;
        }
    }

    public class DirectionalLight : Light
    {
        public Vector4 direction;
        public DirectionalLight(Color color, Vector4 direction) : base(color)
        {
            this.direction = direction;
        }
    }
}
