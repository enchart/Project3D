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
        private static List<PrefabObject> prefabObjects = new List<PrefabObject>();

        private static void Main(string[] args)
        {
            Node node;

            using (AssetImporter importer = new AssetImporter("EnchLeader-animated.fbx"))
            {
                node = importer.LoadModel();
            }

            Prefab prefab = new Prefab("3d-testing", PrefabType.Misc_1);

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
                new Vector3(0.947307f, 0.028426f, 0.119539f),
                Vector3.One,
                new Vector3(0.024158f, 0.026241f, 0.025187f)
            };

            Renderer renderer = new Renderer(node, theme, transform);

            renderer.Render(prefab, prefabObjects, 0f, true);
            for (int i = 0; i < 240; i++)
            {
                float time = i / 24f;
                
                RecursivelyUpdateAnimation(time, node);
                renderer.Render(prefab, prefabObjects, time, false);

                Console.WriteLine("rendering frame " + i);
            }

            File.WriteAllText("3d-testing.lsp", PrefabBuilder.BuildPrefab(prefab));
        }

        private static void RecursivelyUpdateAnimation(float time, Node node)
        {
            if (node.PositionSequence != null)
            {
                node.Position = node.PositionSequence.GetValue(time);
            }

            if (node.ScaleSequence != null)
            {
                node.Scale = node.ScaleSequence.GetValue(time);
            }

            if (node.RotationSequence != null)
            {
                node.Rotation = node.RotationSequence.GetValue(time);
            }

            foreach (Node child in node.Children)
                RecursivelyUpdateAnimation(time, child);
        }
    }
}
