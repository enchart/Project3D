﻿using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Project3D
{
    public class Node
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Rotation = Quaternion.Identity;

        public Vertex[] Vertices;
        public int[] Indices;

        public List<Node> Children = new List<Node>();
    }
}