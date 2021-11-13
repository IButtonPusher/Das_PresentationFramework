using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.OpenGL;
using Das.Views.Windows;

namespace OpenGLTests.Samples
{
   public class HelloShaders : HelloBase
   {
      public HelloShaders(Form form,
                          IntPtr handle) : base(form, handle)
      {
         var vertexShaderSource = File.ReadAllText("Samples\\03_Shader.vs");
         var fragmentShaderSource = File.ReadAllText("Samples\\03_Shader.fs");

         _ourShader = new Shader(vertexShaderSource, fragmentShaderSource);

         var vertices = new[]
         {
            // positions         // colors
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom left
            0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f // top 
         };

         var buff = new UInt32[1];

         GL.GenVertexArrays(1, buff);
         VAO = buff[0];


         GL.GenBuffers(1, buff);
         VBO = buff[0];

         GL.GenBuffers(1, buff);
         EBO = buff[0];

         GL.BindVertexArray(VAO);
         GL.BindBuffer(GL.GL_ARRAY_BUFFER, VBO);
         GL.BufferData(GL.GL_ARRAY_BUFFER, vertices, GL.GL_STATIC_DRAW);

         GL.VertexAttribPointer(0, 3, GL.GL_FLOAT, false, 6 * sizeof(Single), IntPtr.Zero);
         GL.EnableVertexAttribArray(0);

         GL.VertexAttribPointer(1, 3, GL.GL_FLOAT, false, //GL.GL_FALSE, 
            6 * sizeof(Single), (IntPtr) (3 * sizeof(Single)));
         GL.EnableVertexAttribArray(1);
      }

      public void Paint()
      {
         // render
         // ------
         GL.glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
         GL.glClear(GL.GL_COLOR_BUFFER_BIT);

         _ourShader.Use();
         GL.BindVertexArray(VAO);
         GL.glDrawArrays(GL.GL_TRIANGLES, 0, 3);

         var w = _roundedWidth;
         var h = _roundedHeight;

         GL.glReadPixels(0, 0, w, h, GL.BGRA,
            GL.UNSIGNED_BYTE, _dibSection.Bits);

         Native.BitBlt(_hostDc, 0, 0, w, h,
            _dibSectionDeviceContext, 0, 0, Native.SRCCOPY);
      }

      private readonly Shader _ourShader;
   }
}
