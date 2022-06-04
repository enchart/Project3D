using OpenTK.Mathematics;
using PAPrefabToolkit;
using PAPrefabToolkit.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Assimp;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Project3D
{
    internal class Program
    {
        private static List<PrefabObject> prefabObjects = new List<PrefabObject>();

        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            
            Node node;
            Node camera;

            using (AssetImporter importer = new AssetImporter("polygon.dae"))
            {
                node = importer.LoadModel();
                camera = importer.GetCamera();
            }

            Prefab prefab = new Prefab("polygon", PrefabType.Misc_1);

            PrefabObject transform = new PrefabObject(prefab, "Transform")
            {
                ObjectType = PrefabObjectType.Empty
            };

            transform.Events.ScaleKeyframes.Add(new PrefabObject.ObjectEvents.ScaleKeyframe()
            {
                Value = new System.Numerics.Vector2(71.1111111111f, 40f)
            });

            Vector3d[] theme = new Vector3d[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(0.125f, 0.125f, 0.125f),
                new Vector3(0.25f, 0.25f, 0.25f),
                new Vector3(0.375f, 0.375f, 0.375f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.625f, 0.625f, 0.625f),
                new Vector3(0.75f, 0.75f, 0.75f),
                new Vector3(0.875f, 0.875f, 0.75f),
                new Vector3(1f, 1f, 1f)
            };

            Renderer renderer = new Renderer(node, theme, transform);

            for (int i = 0; i < 140; i++)
            {
                double time = i / 24d;
                
                RecursivelyUpdateAnimation(time, node);
                renderer.Render(prefab, prefabObjects, time, camera);

                Console.WriteLine("rendering frame " + i);
            }

            File.WriteAllText("polygon.lsp", PrefabBuilder.BuildPrefab(prefab));
        }

        private static void RecursivelyUpdateAnimation(double time, Node node)
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
