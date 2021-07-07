using OpenTK.Mathematics;

namespace Project3D
{
    public class DefaultVertexShader : VertexShader
    {
        public DefaultVertexShader(Vertex vertex, VertexInData vertexIn) : base(vertex, vertexIn)
        {
        }

        public override VertexOutData ProcessVertex()
        {
            return new VertexOutData
            {
                Position = new Vector4(Vertex.Position, 1f) * VertexIn.ModelViewProjection,
                Brightness = MathHelper.Clamp(
                    Vector3.Dot(
                        Vertex.Normal * new Matrix3(Matrix4.Transpose(Matrix4.Invert(VertexIn.Model))),
                        VertexIn.LightDirection
                    ), 0f, 1f)
            };
        }
    }
}
