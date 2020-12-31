using System;
using System.Threading.Tasks;
using Das.Views.Styles.Declarations;

namespace Das.Views
{
    public abstract partial class VisualElement
    {
        public virtual Boolean TryGetDependencyProperty(DeclarationProperty declarationProperty,
                                                        out IDependencyProperty property)
        {
            IDependencyProperty? dependencyProperty = default;


            switch (declarationProperty)
            {
                case DeclarationProperty.BackgroundColor:
                    dependencyProperty = BackgroundProperty;
                    break;

                case DeclarationProperty.BorderRadius:
                case DeclarationProperty.BorderRadiusBottom:
                case DeclarationProperty.BorderRadiusLeft:
                case DeclarationProperty.BorderRadiusRight:
                case DeclarationProperty.BorderRadiusTop:
                    dependencyProperty = BorderRadiusProperty;
                    break;

                case DeclarationProperty.Height:
                    dependencyProperty = HeightProperty;
                    break;

                case DeclarationProperty.Margin:
                case DeclarationProperty.MarginBottom:
                case DeclarationProperty.MarginLeft:
                case DeclarationProperty.MarginRight:
                case DeclarationProperty.MarginTop:
                    dependencyProperty = MarginProperty;
                    break;

                case DeclarationProperty.Width:
                    dependencyProperty = WidthProperty;
                    break;

                case DeclarationProperty.ZIndex:
                    dependencyProperty = ZIndexProperty;
                    break;

                case DeclarationProperty.VerticalAlign:
                    dependencyProperty = VerticalAlignmentProperty;
                    break;

                case DeclarationProperty.Appearance:
                    dependencyProperty = VisibilityProperty;
                    break;

                case DeclarationProperty.Transform:
                    dependencyProperty = TransformProperty;
                    break;
            }

            property = dependencyProperty!;

            return dependencyProperty != null;
        }
    }
}