using OpenTK.Mathematics;
using System;

namespace Project3D
{
    public class Triangle
    {
        private Vector2[] _points;

        public Vector2 this[int i]
        {
            get { return _points[i]; }
            set { _points[i] = value; }
        }

        public float Area
        {
            get
            {
                float a = (_points[0] - _points[1]).Length;
                float b = (_points[1] - _points[2]).Length;
                float c = (_points[2] - _points[0]).Length;

                float p = (a + b + c) / 2f;

                return MathF.Sqrt(p * (p - a) * (p - b) * (p - c));
            }
        }

        public Triangle()
        {
            _points = new Vector2[3];
        }

        public Triangle(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            _points = new Vector2[3];
            _points[0] = pointA;
            _points[1] = pointB;
            _points[2] = pointC;
        }

        public override string ToString()
        {
            return $"{{ {_points[0]}; {_points[1]}; {_points[2]} }}";
        }
    }
}
