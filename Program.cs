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

            using (AssetImporter importer = new AssetImporter("Ench-animated.dae"))
            {
                node = importer.LoadModel();
            }

            node.Scale = new Vector3(0.35f);

            Quaternion origRot = node.Rotation;
            node.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, 0.4f) * origRot;

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
                new Vector3(0.48515f, 0.715694f, 0.737911f),
                new Vector3(0.024f, 0.026f, 0.025f),
                new Vector3(1f, 1f, 1f),
                new Vector3(1f, 0.022f, 0.117f)
            };

            Renderer renderer = new Renderer(node, theme, transform);

            for (int i = 0; i < 48; i++)
            {
                float time = i / 24f;
                
                RecursivelyUpdateAnimation(time, node);
                renderer.Render(prefab, prefabObjects, time);

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
