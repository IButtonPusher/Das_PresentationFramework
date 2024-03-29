﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Das.OpenGL;

// ReSharper disable All

namespace SharpGL.Shaders
{
   public class ShaderProgram
   {
      public void Create(string vertexShaderSource,
                         string fragmentShaderSource,
                         Dictionary<uint, string> attributeLocations)
      {
         //  Create the shaders.
         vertexShader.Create(GL.GL_VERTEX_SHADER, vertexShaderSource);
         fragmentShader.Create(GL.GL_FRAGMENT_SHADER, fragmentShaderSource);

         //  Create the program, attach the shaders.
         shaderProgramObject = GL.CreateProgram();
         GL.AttachShader(shaderProgramObject, vertexShader.ShaderObject);
         GL.AttachShader(shaderProgramObject, fragmentShader.ShaderObject);

         //  Before we link, bind any vertex attribute locations.
         if (attributeLocations != null)
         {
            foreach (var vertexAttributeLocation in attributeLocations)
               GL.BindAttribLocation(shaderProgramObject, vertexAttributeLocation.Key, vertexAttributeLocation.Value);
         }

         //  Now we can link the program.
         GL.LinkProgram(shaderProgramObject);

         //  Now that we've compiled and linked the shader, check it's link status. If it's not linked properly, we're
         //  going to throw an exception.
         if (GetLinkStatus() == false)
         {
            throw new Exception();
         }
      }


      public int GetAttributeLocation(string attributeName)
      {
         return GL.GetAttribLocation(shaderProgramObject, attributeName);
      }

      public void BindAttributeLocation(uint location,
                                        string attribute)
      {
         GL.BindAttribLocation(shaderProgramObject, location, attribute);
      }

      public void Bind()
      {
         GL.UseProgram(shaderProgramObject);
      }

      public void Unbind()
      {
         GL.UseProgram(0);
      }

      public bool GetLinkStatus()
      {
         int[] parameters = new int[] {0};
         GL.GetProgram(shaderProgramObject, GL.GL_LINK_STATUS, parameters);
         return parameters[0] == GL.GL_TRUE;
      }

      public string GetInfoLog()
      {
         //  Get the info log length.
         int[] infoLength = new int[] {0};
         GL.GetProgram(shaderProgramObject, GL.GL_INFO_LOG_LENGTH, infoLength);
         int bufSize = infoLength[0];

         //  Get the compile info.
         StringBuilder il = new StringBuilder(bufSize);
         GL.GetProgramInfoLog(shaderProgramObject, bufSize, IntPtr.Zero, il);

         return il.ToString();
      }

      public void AssertValid()
      {
         if (vertexShader.GetCompileStatus() == false)
            throw new Exception(vertexShader.GetInfoLog());
         if (fragmentShader.GetCompileStatus() == false)
            throw new Exception(fragmentShader.GetInfoLog());
         if (GetLinkStatus() == false)
            throw new Exception(GetInfoLog());
      }

     

      public void SetUniformMatrix4(string uniformName,
                                    float[] m)
      {
         GL.UniformMatrix4(GetUniformLocation(uniformName), 1, false, m);
      }

      public int GetUniformLocation(string uniformName)
      {
         //  If we don't have the uniform name in the dictionary, get it's 
         //  location and add it.
         if (uniformNamesToLocations.ContainsKey(uniformName) == false)
         {
            uniformNamesToLocations[uniformName] = GL.GetUniformLocation(shaderProgramObject, uniformName);
            //  TODO: if it's not found, we should probably throw an exception.
         }

         //  Return the uniform location.
         return uniformNamesToLocations[uniformName];
      }

      /// <summary>
      ///    Gets the shader program object.
      /// </summary>
      /// <value>
      ///    The shader program object.
      /// </value>
      public uint ShaderProgramObject
      {
         get { return shaderProgramObject; }
      }

      private readonly Shader fragmentShader = new Shader();

      /// <summary>
      ///    A mapping of uniform names to locations. This allows us to very easily specify
      ///    uniform data by name, quickly looking up the location first if needed.
      /// </summary>
      private readonly Dictionary<string, int> uniformNamesToLocations = new Dictionary<string, int>();

      private readonly Shader vertexShader = new Shader();

      private uint shaderProgramObject;
   }
}
