using OpenTK.Mathematics;
using PAPrefabToolkit.Data;
using System;
using System.Collections.Generic;

namespace Project3D
{
    public class Renderer
    {
        private Node _rootNode;
        private PrefabObject _transformObj;

        private Vector3d[] _theme;

        private Matrix4d _view = Matrix4d.CreateTranslation(0f, 0f, -4f);
        private Matrix4d _projection = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(41.3), 16d / 9d, 1.26d, 3.54d);

        private Vector3d _lightDir = Vector3d.Normalize(new Vector3d(-1f, -0.5f, -0.5f));

        public Renderer(Node rootNode, Vector3d[] theme, PrefabObject transformObj)
        {
            _rootNode = rootNode;
            _theme = theme;
            _transformObj = transformObj;
        }

        public void Render(Prefab prefab, List<PrefabObject> objects, double time, Node camera)
        {
            TransformSceneRecursively(_rootNode, Matrix4d.Identity);
            _view = Matrix4d.Invert(camera.Model);
            int index = 0;
            RecursivelyRenderScene(prefab, _rootNode, Matrix4d.Identity, objects, time, objects.Count == 0, ref index);
        }
        
        private void TransformSceneRecursively(Node node, Matrix4d parent)
        {
            Matrix4d local=
                Matrix4d.Scale(node.Scale) *
                Matrix4d.CreateFromQuaternion(node.Rotation) *
                Matrix4d.CreateTranslation(node.Position);
            
            node.Model = local * parent;
            
            foreach (Node child in node.Children)
            {
                TransformSceneRecursively(child, node.Model);
            }
        }

        private void RecursivelyRenderScene(Prefab prefab, Node node, Matrix4d parentTransform, List<PrefabObject> objects, double time, bool createNew, ref int index)
        {
            if (node.Vertices != null && node.Indices != null)
            {
                VertexdInData vsIn = new VertexdInData
                {
                    Model = node.Model,
                    ModelViewProjection = node.Model * _view * _projection,
                    LightDirection = _lightDir,
                    Color = node.Color
                };
            
                ProcessedTriangle[] triangles = RenderGeometry(node.Vertices, node.Indices, vsIn);

                for (int i = 0; i < triangles.Length; i++)
                {
                    ProcessedTriangle triangle = triangles[i];

                    PrefabObject prefabObject;
                    if (createNew)
                    {
                        prefabObject = new PrefabObject(prefab, "Tri-" + i, parent: _transformObj)
                        {
                            AutoKillType = PrefabObjectAutoKillType.Fixed,
                            AutoKillOffset = 10.0f,
                            ParentType = (true, true, true),
                            Origin = (PrefabObjectOriginX.Right, PrefabObjectOriginY.Top),
                            Shape = PrefabObjectShape.Triangle,
                            ShapeOption = 2,
                            Depth = triangle.Depth
                        };
                        objects.Add(prefabObject);
                    }
                    else
                    {
                        prefabObject = objects[index];
                    }
                    index++;

                    prefabObject.Events.PositionKeyframes.Add(new PrefabObject.ObjectEvents.PositionKeyframe()
                    {
                        Time = (float)time,
                        Value = new System.Numerics.Vector2((float)triangle.Position.X, (float)triangle.Position.Y),
                        Easing = PrefabObjectEasing.Instant
                    });

                    prefabObject.Events.ScaleKeyframes.Add(new PrefabObject.ObjectEvents.ScaleKeyframe()
                    {
                        Time = (float)time,
                        Value = new System.Numerics.Vector2((float)triangle.Scale.X, (float)triangle.Scale.Y),
                        Easing = PrefabObjectEasing.Instant
                    });

                    var rotKfs = prefabObject.Events.RotationKeyframes;
                    double lastRot = 0d;
                    for (int j = 0; j < rotKfs.Count; j++)
                    {
                        lastRot += rotKfs[j].Value;
                    }
                    rotKfs.Add(new PrefabObject.ObjectEvents.RotationKeyframe()
                    {
                        Time = (float)time,
                        Value = (float)MathHelper.RadiansToDegrees(triangle.Rotation) - (float)lastRot,
                        Easing = PrefabObjectEasing.Instant
                    });

                    prefabObject.Events.ColorKeyframes.Add(new PrefabObject.ObjectEvents.ColorKeyframe()
                    {
                        Time = (float)time,
                        Value = triangle.Color,
                        Easing = PrefabObjectEasing.Instant
                    });
                }
            }

            foreach (Node child in node.Children)
                RecursivelyRenderScene(prefab, child, node.Model, objects, time, createNew, ref index);
        }

        private ProcessedTriangle[] RenderGeometry(Vertexd[] vertices, int[] indices, VertexdInData shaderData)
        {
            int trianglesCount = indices.Length / 3;

            ProcessedTriangle[] triangles = new ProcessedTriangle[trianglesCount * 2];
            for (int i = 0; i < trianglesCount; i++)
            {
                int indexOffset = i * 3;

                //initialize vertex shaders
                VertexShader vs1 = new DefaultVertexShader(vertices[indices[indexOffset + 0]], shaderData);
                VertexShader vs2 = new DefaultVertexShader(vertices[indices[indexOffset + 1]], shaderData);
                VertexShader vs3 = new DefaultVertexShader(vertices[indices[indexOffset + 2]], shaderData);

                //run vertex shaders
                VertexdOutData vsOut1 = vs1.ProcessVertex();
                VertexdOutData vsOut2 = vs2.ProcessVertex();
                VertexdOutData vsOut3 = vs3.ProcessVertex();

                //get output positions from vertex shaders
                Vector4d pos1 = vsOut1.Position;
                Vector4d pos2 = vsOut2.Position;
                Vector4d pos3 = vsOut3.Position;

                //perspective divide
                Vector3d ssPos1 = pos1.Xyz / pos1.W;
                Vector3d ssPos2 = pos2.Xyz / pos2.W;
                Vector3d ssPos3 = pos3.Xyz / pos3.W;

                //calculate averages
                double avgDepth = (ssPos1.Z + ssPos2.Z + ssPos3.Z) / 3f;
                Vector3d avgColor = (vsOut1.Color + vsOut2.Color + vsOut3.Color) / 3f;

                Triangle triangle = new Triangle(
                    new Vector2d(ssPos1.X, ssPos1.Y),
                    new Vector2d(ssPos2.X, ssPos2.Y),
                    new Vector2d(ssPos3.X, ssPos3.Y));

                //convert to right triangles
                triangle.ToRightAngledTriangles(out Triangle rTri1, out Triangle rTri2);

                //convert to pa transform
                rTri1.GetPositionScaleRotation(out var rPos1, out var sca1, out double rot1);
                rTri2.GetPositionScaleRotation(out var rPos2, out var sca2, out double rot2);

                int paCol = GetThemeColorIndex(avgColor);
                int paDepth = GetIntDepth(avgDepth);

                int triIndexOffset = i * 2;
                triangles[triIndexOffset + 0] = new ProcessedTriangle
                {
                    Position = rPos1,
                    Scale = sca1,
                    Rotation = rot1,
                    Color = paCol,
                    Depth = paDepth
                };

                triangles[triIndexOffset + 1] = new ProcessedTriangle
                {
                    Position = rPos2,
                    Scale = sca2,
                    Rotation = rot2,
                    Color = paCol,
                    Depth = paDepth
                };
            }

            return triangles;
        }

        private int GetIntDepth(double depthFloat)
        {
            return (int)(depthFloat * 64f);
        }

        private int GetThemeColorIndex(Vector3d color)
        {
            int index = 0;
            double minDelta = 4f;

            for (int i = 0; i < _theme.Length; i++)
            {
                double delta = DiffColor(_theme[i], color);

                if (delta < minDelta)
                {
                    minDelta = delta;
                    index = i;
                }
            }

            return index;
        }
        
        private double DiffColor(Vector3d a, Vector3d b)
        {
            return Math.Max(Math.Abs(a.X - b.X), Math.Max(Math.Abs(a.Y - b.Y), Math.Abs(a.Z - b.Z)));
        }
    }
}
