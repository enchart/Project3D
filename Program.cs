using Assimp;
using OpenTK.Mathematics;
using PAPrefabToolkit;
using PAPrefabToolkit.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace Project3D
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Node node = LoadModel("triangulated_torus.fbx");

            Prefab prefab = new Prefab("3d-testing", PrefabType.Misc_1);
            List<PrefabObject> prefabObjects = new List<PrefabObject>();

            PrefabObject transform = new PrefabObject(prefab, "Transform")
            {
                ObjectType = PrefabObjectType.Empty
            };

            transform.Events.ScaleKeyframes.Add(new PrefabObject.ObjectEvents.ScaleKeyframe()
            {
                Value = new System.Numerics.Vector2(71.1111111111f, 40f)
            });

            Vector3[] theme = new Vector3[]
            {

            };

            Renderer renderer = new Renderer(node, theme, transform);

            renderer.Render(prefab, prefabObjects, 0f, true);
            for (int i = 0; i < 240; i++)
            {
                float time = i / 24f;

                node.Rotation = OpenTK.Mathematics.Quaternion.FromAxisAngle(Vector3.UnitY, MathF.Sin(time) * 0.5f);
                renderer.Render(prefab, prefabObjects, time, false);

                Console.WriteLine("rendering frame " + i);
            }

            File.WriteAllText("3d-testing.lsp", PrefabBuilder.BuildPrefab(prefab));
        }

        private static Node LoadModel(string path)
        {
            AssimpContext context = new AssimpContext();
            Scene scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            Assimp.Node root = scene.RootNode;
            return ProcessNodeRecursively(scene, root);
        }

        private static Node ProcessNodeRecursively(Scene scene, Assimp.Node node)
        {
            node.Transform.Decompose(out var scale, out var rotation, out var translation);

            Node pNode = new Node();
            pNode.Position = new Vector3(translation.X, translation.Y, translation.Z);
            pNode.Scale = new Vector3(scale.X, scale.Y, scale.Z);
            pNode.Rotation = new OpenTK.Mathematics.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);

            if (node.HasMeshes)
            {
                Mesh mesh = scene.Meshes[node.MeshIndices[0]];

                Vertex[] vertices = new Vertex[mesh.VertexCount];
                int[] indices = mesh.GetIndices();

                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3D vertex = mesh.Vertices[i];
                    Vector3D normal = mesh.Normals[i];

                    vertices[i] = new Vertex
                    {
                        Position = new Vector3(vertex.X, vertex.Y, vertex.Z),
                        Normal = new Vector3(normal.X, normal.Y, normal.Z)
                    };
                }

                pNode.Vertices = vertices;
                pNode.Indices = indices;
            }

            foreach (Assimp.Node child in node.Children)
                pNode.Children.Add(ProcessNodeRecursively(scene, child));

            return pNode;
        }
    }
}
