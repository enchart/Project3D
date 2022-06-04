namespace Project3D
{
    public abstract class VertexShader
    {
        protected Vertexd Vertexd;
        protected VertexdInData VertexdIn;
        
        public VertexShader(Vertexd vertexd, VertexdInData vertexdIn)
        {
            Vertexd = vertexd;
            VertexdIn = vertexdIn;
        }

        public abstract VertexdOutData ProcessVertex();
    }
}
