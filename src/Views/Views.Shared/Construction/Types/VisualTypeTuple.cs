using System;
using System.Threading.Tasks;

namespace Das.Views.Construction
{
    public class VisualTypeTuple
    {
        public VisualTypeTuple(Type type)
            : this(type, type)
        {
        }

        public VisualTypeTuple(Type childlessType,
                               Type panelType)
        {
            ChildlessType = childlessType;
            PanelType = panelType;
        }

        public override String ToString()
        {
            return ChildlessType == PanelType
                ? "Type: " + ChildlessType
                : "Childless Type: " + ChildlessType + " Panel: " + PanelType;
        }

        public Type ChildlessType { get; }

        public Type PanelType { get; }
    }
}