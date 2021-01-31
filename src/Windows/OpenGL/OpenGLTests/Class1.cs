using System;
using System.Text;
using Das.OpenGL;

namespace SharpGL.Shaders
{
    /// <summary>
    /// This is the base class for all shaders (vertex and fragment). It offers functionality
    /// which is core to all shaders, such as file loading and binding.
    /// </summary>
    public class Shader
    {
        public void Create(UInt32 shaderType, String source)
        {
            //  Create the OpenGL shader object.
            shaderObject = GL.CreateShader(shaderType);

            //  Set the shader source.
            GL.ShaderSource(shaderObject, source);

            //  Compile the shader object.
            GL.CompileShader(shaderObject);

            //  Now that we've compiled the shader, check it's compilation status. If it's not compiled properly, we're
            //  going to throw an exception.
            if (GetCompileStatus() == false)
            {
                throw new Exception();
            }
        }

        public void Delete()
        {
            GL.DeleteShader(shaderObject);
            shaderObject = 0;
        }

        public Boolean GetCompileStatus()
        {
            Int32[] parameters = new Int32[] { 0 };
            GL.GetShaderiv(shaderObject, GL.GL_COMPILE_STATUS, parameters);
            return parameters[0] == GL.GL_TRUE;
        }

        public String GetInfoLog()
        {
            //  Get the info log length.
            Int32[] infoLength = new Int32[] { 0 };
            GL.GetShaderiv(ShaderObject,
                GL.GL_INFO_LOG_LENGTH, infoLength);
            Int32 bufSize = infoLength[0];

            //  Get the compile info.
            StringBuilder il = new StringBuilder(bufSize);
            GL.GetShaderInfoLog(shaderObject, bufSize, IntPtr.Zero, il);

            return il.ToString();
        }

        /// <summary>
        /// The OpenGL shader object.
        /// </summary>
        private UInt32 shaderObject;

        /// <summary>
        /// Gets the shader object.
        /// </summary>
        public UInt32 ShaderObject
        {
            get { return shaderObject; }
        }
    }

    
}
