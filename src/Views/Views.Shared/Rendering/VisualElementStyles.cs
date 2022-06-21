using System;
using System.Threading.Tasks;
using Das.Views.Styles.Declarations;

namespace Das.Views
{
    public partial class VisualElement
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

                case DeclarationProperty.Border:
                    dependencyProperty = BorderProperty;
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

                case DeclarationProperty.Top:
                    dependencyProperty = TopProperty;
                    break;

                case DeclarationProperty.Left:
                    dependencyProperty = LeftProperty;
                    break;

                case DeclarationProperty.Right:
                    dependencyProperty = RightProperty;
                    break;

                case DeclarationProperty.Bottom:
                    dependencyProperty = BottomProperty;
                    break;

                case DeclarationProperty.BoxShadow:
                    dependencyProperty = BoxShadowProperty;
                    break;

                case DeclarationProperty.Transition:
                    break;
            }

            property = dependencyProperty!;

            return dependencyProperty != null;
        }
    }
}