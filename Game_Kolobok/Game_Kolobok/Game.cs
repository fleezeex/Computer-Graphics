using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace KolobokGame
{
    public class Game : GameWindow
    {
        private Vector3 _cameraOffset = new Vector3(0, 5, 10);

        private Vector3 _kolobokPosition = Vector3.Zero;
        private float _kolobokRadius = 0.5f;
        private float _movementSpeed = 0.5f;
        private Vector3 _startPosition = Vector3.Zero;

        private List<Tree> _trees = new List<Tree>();
        private Random _random = new Random();

        private Texture _groundTexture;
        private readonly float[] _groundVertices = {
            // x,    y, z,       normal,       texCoords
            -50f, 0f, -600f,    0, 1, 0,     0, 60,
            -50f, 0f,  10f,      0, 1, 0,     0, 0,
             50f, 0f,  10f,      0, 1, 0,     1, 0,
             50f, 0f, -600f,    0, 1, 0,     1, 60
        };

        private readonly uint[] _groundIndices = { 0, 1, 2, 0, 2, 3 };
        private int _groundVAO, _groundVBO, _groundEBO;

        private List<GrassBlade> _grassBlades = new List<GrassBlade>();
        private Shader _grassShader;
        private int _grassVAO, _grassVBO;
        private const int GrassCount = 100000;

        private Shader _shader;
        private Shader _kolobokShader;
        private Shader _treeShader;

        public Game(int width, int height) : base
            (GameWindowSettings.Default, NativeWindowSettings.Default)
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.5f, 0.7f, 1f, 1f);
            GL.Enable(EnableCap.DepthTest);

            _shader = new Shader("default.vert", "default.frag");
            _kolobokShader = new Shader("simple.vert", "simple.frag");
            _grassShader = new Shader("grass.vert", "grass.frag");
            _treeShader = new Shader("simple.vert", "simple.frag");


            _groundVAO = GL.GenVertexArray();
            _groundVBO = GL.GenBuffer();
            _groundEBO = GL.GenBuffer();

            GL.BindVertexArray(_groundVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _groundVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _groundVertices.Length * sizeof(float), _groundVertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _groundEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _groundIndices.Length * sizeof(uint), _groundIndices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _groundTexture = new Texture("ground.jpg");
            GenerateGrass();
            GenerateTrees(200);

            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);

            _shader.Use();
            _shader.SetMatrix4("projection", projection);

            _kolobokShader.Use();
            _kolobokShader.SetMatrix4("projection", projection);

            _grassShader.Use();
            _grassShader.SetMatrix4("projection", projection);

            _treeShader.Use();
            _treeShader.SetMatrix4("projection", projection);
        }

        private void GenerateTrees(int count)
        {
            _trees.Clear();
            float roadLength = 600f;
            float roadWidth = 60f;

            for (int i = 0; i < count; i++)
            {
                float z = -(i * (roadLength / count) + (float)_random.NextDouble() * 10f);

                _trees.Add(new Tree
                {
                    Position = new Vector3(
                        (float)(_random.NextDouble() * roadWidth - roadWidth / 2),
                        0,
                        z
                    ),
                    TrunkHeight = (float)(_random.NextDouble() * 0.5 + 1.8f),
                    TrunkRadius = (float)(_random.NextDouble() * 0.1 + 0.25f),
                    FoliageRadius = (float)(_random.NextDouble() * 0.3 + 1.2f)
                });
            }
        }

        private void GenerateGrass()
        {
            _grassBlades.Clear();
            float fieldWidth = 90f;
            float startZ = 10.0f; 
            float endZ = -600f;

            for (int i = 0; i < GrassCount; i++)
            {
                float x = (float)(_random.NextDouble() * fieldWidth - fieldWidth / 2);
                float z = (float)(_random.NextDouble() * (endZ - startZ) + startZ);

                float height = (float)(_random.NextDouble() * 0.5 + 0.3);
                float width = (float)(_random.NextDouble() * 0.08 + 0.05);
                float sway = (float)(_random.NextDouble() * 0.3f + 0.1f);

                _grassBlades.Add(new GrassBlade
                {
                    Position = new Vector3(x, 0, z),
                    Height = height,
                    Width = width,
                    Sway = sway,
                    Color = new Vector3(
                        (float)(_random.NextDouble() * 0.2 + 0.3),
                        (float)(_random.NextDouble() * 0.3 + 0.6),
                        (float)(_random.NextDouble() * 0.1 + 0.2)
                    )
                });
            }

            _grassVAO = GL.GenVertexArray();
            _grassVBO = GL.GenBuffer();

            GL.BindVertexArray(_grassVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _grassVBO);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var cameraPos = _kolobokPosition + _cameraOffset;
            var view = Matrix4.LookAt(cameraPos, _kolobokPosition, Vector3.UnitY);

           
            _shader.Use();
            _shader.SetMatrix4("view", view);
            GL.BindVertexArray(_groundVAO);
            _groundTexture.Use(TextureUnit.Texture0);
            _shader.SetInt("texture1", 0);
            _shader.SetMatrix4("model", Matrix4.Identity);
            GL.DrawElements(PrimitiveType.Triangles, _groundIndices.Length, DrawElementsType.UnsignedInt, 0);


            _grassShader.Use();
            _grassShader.SetMatrix4("view", view);
            RenderGrass();


            _treeShader.Use();
            _treeShader.SetMatrix4("view", view);
            foreach (var tree in _trees)
            {

                _treeShader.SetVector3("objectColor", new Vector3(0.5f, 0.3f, 0.1f));
                var trunkModel = Matrix4.CreateScale(tree.TrunkRadius, tree.TrunkHeight, tree.TrunkRadius) *
                               Matrix4.CreateTranslation(tree.Position + new Vector3(0, tree.TrunkHeight / 2 + tree.YOffset, 0));
                _treeShader.SetMatrix4("model", trunkModel);
                DrawCylinder(8);


                _treeShader.SetVector3("objectColor", new Vector3(0.1f, 0.6f, 0.2f));
                var foliageModel = Matrix4.CreateScale(tree.FoliageRadius) *
                                  Matrix4.CreateTranslation(tree.Position + new Vector3(0, tree.TrunkHeight + tree.FoliageRadius / 2 + tree.YOffset, 0));
                _treeShader.SetMatrix4("model", foliageModel);
                DrawSphere(tree.FoliageRadius / 2, 10, 10);
            }

            _kolobokShader.Use();
            _kolobokShader.SetMatrix4("view", view);
            var model = Matrix4.CreateTranslation(_kolobokPosition);
            _kolobokShader.SetMatrix4("model", model);
            _kolobokShader.SetVector3("objectColor", new Vector3(1.0f, 0.9f, 0.2f));
            DrawSphere(_kolobokRadius, 20, 20);

            SwapBuffers();
        }

        private void DrawCylinder(int segments)
        {
            float height = 1.0f;
            float radius = 1.0f;

            GL.Begin(PrimitiveType.TriangleStrip);
            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                float x = (float)Math.Cos(angle) * radius;
                float z = (float)Math.Sin(angle) * radius;

                GL.Vertex3(x, height, z);
                GL.Vertex3(x, 0, z);
            }
            GL.End();
        }

        private void RenderGrass()
        {
            GL.BindVertexArray(_grassVAO);

            foreach (var grass in _grassBlades)
            {
                if (Math.Abs(grass.Position.Z - _kolobokPosition.Z) > 40) continue;

                float swayFactor = (float)Math.Sin(grass.Position.X + grass.Position.Z + GLFW.GetTime()) * grass.Sway;

                float[] vertices = {
                    grass.Position.X - grass.Width/2, grass.Position.Y, grass.Position.Z, grass.Color.X, grass.Color.Y, grass.Color.Z,
                    grass.Position.X + grass.Width/2, grass.Position.Y, grass.Position.Z, grass.Color.X, grass.Color.Y, grass.Color.Z,
                    grass.Position.X + swayFactor, grass.Position.Y + grass.Height, grass.Position.Z, grass.Color.X*0.9f, grass.Color.Y*0.9f, grass.Color.Z*0.9f,

                    grass.Position.X - grass.Width/2, grass.Position.Y, grass.Position.Z, grass.Color.X, grass.Color.Y, grass.Color.Z,
                    grass.Position.X + swayFactor, grass.Position.Y + grass.Height, grass.Position.Z, grass.Color.X*0.9f, grass.Color.Y*0.9f, grass.Color.Z*0.9f,
                    grass.Position.X + grass.Width/2, grass.Position.Y, grass.Position.Z, grass.Color.X, grass.Color.Y, grass.Color.Z
                };

                GL.BindBuffer(BufferTarget.ArrayBuffer, _grassVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }

        private void DrawSphere(float radius, int segments, int rings)
        {
            for (int i = 0; i <= rings; i++)
            {
                double lat0 = Math.PI * (-0.5 + (double)(i - 1) / rings);
                double z0 = Math.Sin(lat0) * radius;
                double zr0 = Math.Cos(lat0) * radius;

                double lat1 = Math.PI * (-0.5 + (double)i / rings);
                double z1 = Math.Sin(lat1) * radius;
                double zr1 = Math.Cos(lat1) * radius;

                GL.Begin(PrimitiveType.TriangleStrip);
                for (int j = 0; j <= segments; j++)
                {
                    double lng = 2 * Math.PI * (double)(j - 1) / segments;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);

                    GL.Vertex3(x * zr0, y * zr0, z0);
                    GL.Vertex3(x * zr1, y * zr1, z1);
                }
                GL.End();
            }
        }

        private bool CheckCollision(Vector3 kolobokPos, float kolobokRadius, Tree tree)
        {
            float distance = Vector3.Distance(
                new Vector3(kolobokPos.X, 0, kolobokPos.Z),
                new Vector3(tree.Position.X, 0, tree.Position.Z)
            );

            return distance < (kolobokRadius + tree.TrunkRadius);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
                Close();

            if (input.IsKeyDown(Keys.W))
                _kolobokPosition.Z -= _movementSpeed;
            if (input.IsKeyDown(Keys.S))
                _kolobokPosition.Z += _movementSpeed;
            if (input.IsKeyDown(Keys.A))
                _kolobokPosition.X -= _movementSpeed;
            if (input.IsKeyDown(Keys.D))
                _kolobokPosition.X += _movementSpeed;

            _kolobokPosition.Y = _kolobokRadius;
            _kolobokPosition.X = MathHelper.Clamp(_kolobokPosition.X, -9.5f, 9.5f);
            _kolobokPosition.Z = MathHelper.Clamp(_kolobokPosition.Z, -600.0f, 0f);

            foreach (var tree in _trees)
            {
                if (CheckCollision(_kolobokPosition, _kolobokRadius, tree))
                {
                    _kolobokPosition = _startPosition;
                    break;
                }
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), Size.X / (float)Size.Y, 0.1f, 100f);
            _shader.Use();
            _shader.SetMatrix4("projection", projection);
            _kolobokShader.Use();
            _kolobokShader.SetMatrix4("projection", projection);
            _treeShader.Use();
            _treeShader.SetMatrix4("projection", projection);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_groundVAO);
            GL.DeleteBuffer(_groundVBO);
            GL.DeleteBuffer(_groundEBO);
            _groundTexture.Dispose();
            _shader.Dispose();
            _kolobokShader.Dispose();
            _treeShader.Dispose();
            base.OnUnload();
        }
    }
}