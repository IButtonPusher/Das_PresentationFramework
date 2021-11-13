using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Das.OpenGL;
using Das.OpenGL.Windows;
using Das.Views.Windows;
using OpenGLTests.Samples;

// ReSharper disable All


namespace OpenGLTests
{
   /// <summary>
   /// https://learnopengl.com/Getting-started/Hello-Triangle
   /// </summary>
   public class HelloTriangle : HelloBase
   {
      public HelloTriangle(Form form,
                           IntPtr handle)
         : base(form, handle)
      {

         const String vertexShaderSource = "#version 330 core\n" +
                                           "layout (location = 0) in vec3 aPos;\n" +
                                           "void main()\n" +
                                           "{\n" +
                                           "   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n" +
                                           "}\0";

         const String fragmentShaderSource = "#version 330 core\n" +
                                             "out vec4 FragColor;\n" +
                                             "void main()\n" +
                                             "{\n" +
                                             "   FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);\n" +
                                             "}\n\0";


         var vertexShader = GL.CreateVertexShader();
         GL.ShaderSource(vertexShader, vertexShaderSource);
         GL.CompileShader(vertexShader);
         var rdrr = GL.GetCompileStatus(vertexShader);
         if (!rdrr)
            throw new InvalidProgramException("Unable to compile vertex shader");

         var fragmentShader = GL.CreateShader(GL.GL_FRAGMENT_SHADER);
         GL.ShaderSource(fragmentShader, fragmentShaderSource);
         GL.CompileShader(fragmentShader);
         rdrr = GL.GetCompileStatus(fragmentShader);
         if (!rdrr)
            throw new InvalidProgramException("Unable to compile fragment shader");


         _shaderProgram = GL.CreateProgram();
         GL.AttachShader(_shaderProgram, vertexShader);
         GL.AttachShader(_shaderProgram, fragmentShader);
         GL.LinkProgram(_shaderProgram);


         // check for linking errors
         var parameters = new[] {0};
         GL.GetProgram(_shaderProgram, GL.GL_LINK_STATUS, parameters);
         var ok = parameters[0] == GL.GL_TRUE;
         if (!ok)
            throw new InvalidProgramException("Unable to link shader");

         GL.DeleteShader(vertexShader);
         GL.DeleteShader(fragmentShader);


         var vertices = new[]
         {
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f // top left 
         };

         UInt16[] indices =
         {
            // note that we start from 0!
            0, 1, 3, // first Triangle
            1, 2, 3 // second Triangle
         };

         var buff = new UInt32[1];

         GL.GenVertexArrays(1, buff);
         VAO = buff[0];


         GL.GenBuffers(1, buff);
         VBO = buff[0];

         GL.GenBuffers(1, buff);
         EBO = buff[0];

         //// bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
         GL.BindVertexArray(VAO);


         GL.BindBuffer(GL.GL_ARRAY_BUFFER, VBO);
         GL.BufferData(GL.GL_ARRAY_BUFFER, vertices, GL.GL_STATIC_DRAW);

         GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, EBO);
         GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indices, GL.GL_STATIC_DRAW);

         GL.VertexAttribPointer(0, 3, GL.GL_FLOAT, false, 3 * sizeof(Single), IntPtr.Zero);
         GL.EnableVertexAttribArray(0);

         //// note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
         GL.BindBuffer(GL.GL_ARRAY_BUFFER, 0);

         // remember: do NOT unbind the EBO while a VAO is active as the bound element buffer object IS stored in the VAO; keep the EBO bound.
         //glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

         // You can unbind the VAO afterwards so other VAO calls won't accidentally modify this VAO, but this rarely happens. Modifying other
         // VAOs requires a call to glBindVertexArray anyways so we generally don't unbind VAOs (nor VBOs) when it's not directly necessary.
         GL.BindVertexArray(0);
      }


      public void Paint()
      {
         // render
         // ------
         GL.glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
         GL.glClear(GL.GL_COLOR_BUFFER_BIT);

         // draw our first triangle
         GL.UseProgram(_shaderProgram);

         // seeing as we only have a single VAO there's no need to bind it every time, but we'll do so to keep things a bit more organized
         GL.BindVertexArray(VAO); 
         //GL.glDrawArrays(GL.GL_TRIANGLES, 0, 6);
         GL.glDrawElements(GL.GL_TRIANGLES, 6, GL.GL_UNSIGNED_SHORT, IntPtr.Zero);
         // glBindVertexArray(0); // no need to unbind it every time 

         var w = _roundedWidth;
         var h = _roundedHeight;

         GL.glReadPixels(0, 0, w, h, GL.BGRA,
            GL.UNSIGNED_BYTE, _dibSection.Bits);

         Native.BitBlt(_hostDc, 0, 0, w, h,
            _dibSectionDeviceContext, 0, 0, Native.SRCCOPY);
      }
      private void CreateDBOs()
      {
         _dibSectionDeviceContext = Native.CreateCompatibleDC(_deviceContextHandle);
         _dibSection.Create(_dibSectionDeviceContext, _roundedWidth,
            _roundedHeight, _bitDepth);
      }


      private void OnSizeChanged()
      {
         SetSizeFromHost();
         var w = _roundedWidth;
         var h = _roundedHeight;

         GLWindowBuilder.ResizeNativeWindow(_windowHandle, w, h);

         _dibSection.Resize(w, h, _bitDepth);

         GL.glViewport(0, 0, w, h);
      }

      private void SetSizeFromHost()
      {
         var w = _form.Width;
         var h = _form.Height;

         _roundedWidth = Convert.ToInt32(w);
         _roundedHeight = Convert.ToInt32(h);

         _currentSize.Width = w;
         _currentSize.Height = h;
      }

      //public const Int32 WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
      //public const Int32 WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
      //public const Int32 WGL_CONTEXT_FLAGS_ARB = 0x2094;
      //public const Int32 WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x0002;

      private readonly UInt32 _shaderProgram;
      
      protected UInt32 _colorRenderBufferID;
      
      protected UInt32 _depthRenderBufferID;
      protected UInt32 _frameBufferID;
   }
}
