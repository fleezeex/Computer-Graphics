using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.IO;

namespace KolobokGame
{
    public class Shader : IDisposable
    {
        private readonly int _handle;

        public Shader(string vertPath, string fragPath)
        {
            string vertShaderSource;
            try
            {
                vertShaderSource = File.ReadAllText(Path.Combine("Shaders", vertPath));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load vertex shader from {vertPath}: {ex.Message}");
            }
            string fragShaderSource;
            try
            {
                fragShaderSource = File.ReadAllText(Path.Combine("Shaders", fragPath));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load fragment shader from {fragPath}: {ex.Message}");
            }
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertShaderSource);
            GL.CompileShader(vertexShader);


            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertSuccess);
            if (vertSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new Exception($"Vertex shader compilation failed: {infoLog}");
            }


            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragShaderSource);
            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragSuccess);
            if (fragSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception($"Fragment shader compilation failed: {infoLog}");
            }


            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);
            GL.LinkProgram(_handle);


            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int programSuccess);
            if (programSuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(_handle);
                throw new Exception($"Shader program linking failed: {infoLog}");
            }


            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use() => GL.UseProgram(_handle);
        public void SetInt(string name, int value) => GL.Uniform1(GL.GetUniformLocation(_handle, name), value);
        public void SetFloat(string name, float value) => GL.Uniform1(GL.GetUniformLocation(_handle, name), value);
        public void SetVector3(string name, Vector3 value) => GL.Uniform3(GL.GetUniformLocation(_handle, name), value);
        public void SetMatrix4(string name, Matrix4 matrix) => GL.UniformMatrix4(GL.GetUniformLocation(_handle, name), false, ref matrix);

        public void Dispose()
        {
            GL.DeleteProgram(_handle);
            GC.SuppressFinalize(this);
        }
    }
}