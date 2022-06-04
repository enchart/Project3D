using OpenTK.Mathematics;

namespace Project3D
{
    public class DefaultVertexShader : VertexShader
    {
        public DefaultVertexShader(Vertexd vertexd, VertexdInData vertexdIn) : base(vertexd, vertexdIn)
        {
        }

        public override VertexdOutData ProcessVertex()
        {
            return new VertexdOutData
            {
                Position = new Vector4d(Vertexd.Position, 1f) * VertexdIn.ModelViewProjection,
                Color = VertexdIn.Color
            };
        }
    }
}
