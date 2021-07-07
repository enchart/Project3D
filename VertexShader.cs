namespace Project3D
{
    public abstract class VertexShader
    {
        protected Vertex Vertex;
        protected VertexInData VertexIn;
        
        public VertexShader(Vertex vertex, VertexInData vertexIn)
        {
            Vertex = vertex;
            VertexIn = vertexIn;
        }

        public abstract VertexOutData ProcessVertex();
    }
}
