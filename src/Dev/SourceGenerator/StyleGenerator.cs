using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Text;
using Das.Serializer;
using Das.Views.Styles.Construction;
using Microsoft.CodeAnalysis.Text;

namespace Das.Views
{
    [Generator]
    public class StyleGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            Console.WriteLine("hey 2 f u man");
            //throw new NotImplementedException();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.AdditionalFiles;
            var serializer = new DasSerializer();
            var styler = new DefaultStyleInflater(serializer.TypeInferrer);

            var allMyFiles = context.AdditionalFiles;

            foreach (var fileName in allMyFiles)
            {
                var txt = File.ReadAllText(fileName.Path);
                var sheeit = styler.InflateXml(txt);
                context.AddSource("helloWorldGenerator", SourceText.From(txt, Encoding.UTF8));
            }

        }

        public void Execute2(GeneratorExecutionContext context)
        {
            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"
using System;
namespace HelloWorldGenerated
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
            Console.WriteLine(""Hello from generated code!"");
            Console.WriteLine(""The following deez nutz existed in the compilation that created this program:"");
");

            // using the context, get a list of syntax trees in the users compilation
            //var syntaxTrees = context.Compilation.SyntaxTrees;
            var syntaxTrees = context.AdditionalFiles;

            // add the filepath of each tree to the class we're building
            foreach (var tree in syntaxTrees)
            {
                sourceBuilder.AppendLine($@"Console.WriteLine(@"" - {tree.Path}"");");
            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation
            context.AddSource("helloWorldGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

        }
    }
}
