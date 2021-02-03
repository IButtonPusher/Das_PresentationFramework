using System;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class ClassStyleSelector : SelectorBase
    {
        public ClassStyleSelector(String className) : base(String.Intern(className).GetHashCode())
        {
            if (!className.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();

            ClassName = className.Substring(1);
        }

        public String ClassName { get; }

        public sealed override Boolean Equals(IStyleSelector other)
        {
            return other is ClassStyleSelector classy &&
                   String.Equals(classy.ClassName, ClassName);
        }

        public override String ToString()
        {
            return "Class: " + ClassName;
        }

        public sealed override Boolean TryGetClassName(out String className)
        {
            className = ClassName;
            return true;
        }
    }
}
