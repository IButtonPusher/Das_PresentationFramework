using Das.OpenGL;
using System;
// ReSharper disable All

namespace SharpGL.VertexBuffers
{
    /// <summary>
    /// A VertexBufferArray is a logical grouping of VertexBuffers. Vertex Buffer Arrays
    /// allow us to use a set of vertex buffers for vertices, indicies, normals and so on,
    /// without having to use more complicated interleaved arrays.
    /// </summary>
    public class VertexBufferArray
    {
        public void Create()
        {
            //  Generate the vertex array.
            UInt32[] ids = new UInt32[1];
            GL.GenVertexArrays(1, ids);
            vertexArrayObject = ids[0];
        }

        public void Delete()
        {
            GL.DeleteVertexArrays(1, new UInt32[] { vertexArrayObject });
        }

        public void Bind()
        {
            GL.BindVertexArray(vertexArrayObject);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Gets the vertex buffer array object.
        /// </summary>
        public UInt32 VertexBufferArrayObject
        {
            get { return vertexArrayObject; }
        }

        private UInt32 vertexArrayObject;
    }
}