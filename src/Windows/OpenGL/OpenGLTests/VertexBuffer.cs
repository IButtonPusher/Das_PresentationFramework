using System;
using Das.OpenGL;

namespace SharpGL.VertexBuffers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Very useful reference for management of VBOs and VBAs:
    /// http://stackoverflow.com/questions/8704801/glvertexattribpointer-clarification
    /// </remarks>
    public class VertexBuffer
    {
        public void Create()
        {
            //  Generate the vertex array.
            UInt32[] ids = new UInt32[1];
            GL.GenBuffers(1, ids);
            vertexBufferObject = ids[0];
        }

        public void SetData(UInt32 attributeIndex, Single[] rawData, Boolean isNormalised, Int32 stride)
        {
            //  Set the data, specify its shape and assign it to a vertex attribute (so shaders can bind to it).
            GL.BufferData(GL.GL_ARRAY_BUFFER, rawData, GL.GL_STATIC_DRAW);
            GL.VertexAttribPointer(attributeIndex, stride, GL.GL_FLOAT, isNormalised, 0, IntPtr.Zero);
            GL.EnableVertexAttribArray(attributeIndex);
        }

        public void Bind()
        {
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, vertexBufferObject);
        }

        public void Unbind()
        {
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, 0);
        }

        public Boolean IsCreated() { return vertexBufferObject != 0; }

        /// <summary>
        /// Gets the vertex buffer object.
        /// </summary>
        public UInt32 VertexBufferObject
        {
            get { return vertexBufferObject; }
        }

        private UInt32 vertexBufferObject;
    }
}