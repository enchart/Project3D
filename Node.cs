using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Project3D
{
    public class Node
    {
        public string Name;
        
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Rotation = Quaternion.Identity;

        public Vector3Sequence PositionSequence;
        public Vector3Sequence ScaleSequence;
        public QuaternionSequence RotationSequence;

        public Vector3 Color = Vector3.One;

        public Vertex[] Vertices;
        public int[] Indices;

        public List<Node> Children = new List<Node>();

        public Matrix4 Model;

        public Node FindNode(string name)
        {
            if (name == Name)
            {
                return this;
            }

            foreach (var child in Children)
            {
                var node = child.FindNode(name);
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }
    }
}
