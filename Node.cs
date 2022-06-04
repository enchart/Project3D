using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Project3D
{
    public class Node
    {
        public string Name;
        
        public Vector3d Position = Vector3d.Zero;
        public Vector3d Scale = Vector3d.One;
        public Quaterniond Rotation = Quaterniond.Identity;

        public Vector3dSequence PositionSequence;
        public Vector3dSequence ScaleSequence;
        public QuaterniondSequence RotationSequence;

        public Vector3d Color = Vector3d.One;

        public Vertexd[] Vertices;
        public int[] Indices;

        public List<Node> Children = new List<Node>();

        public Matrix4d Model;

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
