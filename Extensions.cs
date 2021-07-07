using OpenTK.Mathematics;
using System;
using System.Linq;

namespace Project3D
{
    public static class Extensions
    {
        private struct AnglePointPair
        {
            public Vector2 Position;
            public float Angle;

            public AnglePointPair(Vector2 position, float angle)
            {
                Position = position;
                Angle = angle;
            }
        }

        public static void ToRightAngledTriangles(this Triangle source, out Triangle tri0, out Triangle tri1)
        {
            AnglePointPair[] pairs = new AnglePointPair[]
            {
                new AnglePointPair(source[0], AngleBetween(source[1] - source[0], source[2] - source[0])), //bac
                new AnglePointPair(source[1], AngleBetween(source[2] - source[1], source[0] - source[1])), //cba
                new AnglePointPair(source[2], AngleBetween(source[1] - source[2], source[0] - source[2]))  //bca
            };

            pairs = pairs.OrderByDescending(x => x.Angle).ToArray();

            float tanB = MathF.Tan(pairs[1].Angle);

            Vector2 vecBC = pairs[2].Position - pairs[1].Position;
            float BC = vecBC.Length;
            float AH = 2f * source.Area / BC;

            float BH = AH / tanB;
            Vector2 H = Vector2.Lerp(pairs[1].Position, pairs[2].Position, BH / BC);

            tri0 = new Triangle(H, pairs[0].Position, pairs[1].Position);
            tri1 = new Triangle(H, pairs[0].Position, pairs[2].Position);
        }

        public static void GetPositionScaleRotation(this Triangle triangle, out Vector2 position, out Vector2 scale, out float rotation)
        {
            Vector2 AB = triangle[1] - triangle[0];
            Vector2 AC = triangle[2] - triangle[0];

            Vector2 ABdir = Vector2.Normalize(AB);
            Vector2 ACdir = Vector2.Normalize(AC);

            position = triangle[0];
            rotation = MathF.Atan2(ABdir.Y, ABdir.X); //rotate the triangle according to AB

            scale = new Vector2(0f, 0f);

            Vector2 rotatedACdir = RotateVector2(ACdir, -rotation); //rotate AC
            if (rotatedACdir.Y < 0f)
            {
                scale.Y = -AC.Length;
            }
            else
            {
                scale.Y = AC.Length;
            }

            scale.X = AB.Length;
        }

        private static Vector2 RotateVector2(Vector2 vec, float radians)
        {
            return new Vector2(
                vec.X * MathF.Cos(radians) - vec.Y * MathF.Sin(radians),
                vec.X * MathF.Sin(radians) + vec.Y * MathF.Cos(radians)
            );
        }

        private static float AngleBetween(Vector2 vec0, Vector2 vec1)
        {
            return MathF.Acos(Vector2.Dot(vec0, vec1) / (vec0.Length * vec1.Length));
        }
    }
}
