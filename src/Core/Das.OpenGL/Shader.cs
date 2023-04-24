using System;

namespace Das.OpenGL;

public class Shader
{
   public UInt32 Id { get; }

   private readonly String _vertexProgram;
   private readonly String _fragmentProgram;

   public Shader(String vertexProgram,
                 String fragmentProgram)
   {
      _vertexProgram = vertexProgram;
      _fragmentProgram = fragmentProgram;

         
      var vertex = GL.CreateShader(GL.GL_VERTEX_SHADER);
      GL.ShaderSource(vertex, vertexProgram);
      GL.CompileShader(vertex);

      Int32[] parameters = new Int32[] { 0 };
      GL.GetShaderiv(vertex, GL.GL_COMPILE_STATUS, parameters);
      //return parameters[0] == GL.GL_TRUE;



      var fragment = GL.CreateShader(GL.GL_FRAGMENT_SHADER);
      GL.ShaderSource(fragment, fragmentProgram);
      GL.CompileShader(fragment);

      parameters = new Int32[] { 0 };
      GL.GetShaderiv(vertex, GL.GL_COMPILE_STATUS, parameters);

      Id = GL.CreateProgram();


      GL.AttachShader(Id, vertex);
      GL.AttachShader(Id, fragment);
      GL.LinkProgram(Id);

      GL.DeleteShader(vertex);
      GL.DeleteShader(fragment);
   }

   public void Use()
   {
      GL.UseProgram(Id);
   }
}