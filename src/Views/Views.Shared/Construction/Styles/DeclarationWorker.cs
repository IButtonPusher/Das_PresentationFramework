using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Declarations;

using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Application;
using Das.Views.Styles.Declarations;

namespace Das.Views.Construction.Styles
{
    public class DeclarationWorker : IDeclarationWorker
    {
        public DeclarationWorker(Dictionary<IVisualElement, ValueCube> renderPositions,
                                 IVisualBootstrapper visualBootstrapper)
        {
            _renderPositions = renderPositions;
            _visualBootstrapper = visualBootstrapper;
        }

        public IEnumerable<IStyleValueAssignment> BuildStyleValueAssignments(IVisualElement visual,
            IVisualLineage visualLineage,
            IStyleRule rule)
        {
            if (rule.Selector.TryGetContentAppendType(out var contentAppend))
            {
                yield return new PseudoVisualAssignment(visual, contentAppend,
                    rule, _visualBootstrapper, visualLineage, BuildStyleValueAssignments);

                //AddPseudoVisual(applicable, contentAppend, rule, visualLineage);
                //return true;
            }
            else
            {
                foreach (var assignment in BuildStyleValueAssignments(visual, visualLineage, rule.Selector,
                    rule.Declarations))
                {
                    yield return assignment;
                }

                //foreach (var declaration in rule.Declarations)
                //{
                //    var assignment = ApplyDeclarationToVisual(visual, visualLineage,
                //        declaration, rule.Selector);

                //    if (assignment != null)
                //        yield return assignment;
                //}
            }
        }

        private static IEnumerable<IStyleValueAssignment> BuildStyleValueAssignments(IVisualElement visual,
                                                                                     IVisualLineage lineage,
                                                                                     IStyleSelector selector,
                                                                                     IEnumerable<IStyleDeclaration> declarations)
        {
            foreach (var declaration in declarations)
            {
                var assignment = ApplyDeclarationToVisual(visual, lineage,
                    declaration, selector);

                if (assignment != null)
                    yield return assignment;
            }
        }

        private static IStyleValueAssignment? ApplyDeclarationToVisual(IVisualElement visual,
                                                                       IVisualLineage visualLineage,
                                                                       IStyleDeclaration declaration,
                                                                       IStyleSelector selector)
        {
            if (visual.TryGetDependencyProperty(declaration.Property, //declarationValue, 
                out var dependencyProperty))
            {
                Debug.WriteLine("Setting " + visual.GetType().Name + "->" + dependencyProperty +
                                " = " + declaration);

                return ApplyDeclarationToDependencyProperty(visual, visualLineage,
                    dependencyProperty, declaration, selector);
            }

            Debug.WriteLine("No dependency property found for " + declaration.Property +
                            " on " + visual);
            return default;
        }

        private static IStyleValueAssignment ApplyDeclarationToDependencyProperty(IVisualElement visual,
                                                                 IVisualLineage visualLineage,
                                                                 IDependencyProperty property,
                                                                 IStyleDeclaration declaration,
                                                                 IStyleSelector selector)
        {
            var declarationValue = GetDeclarationValue(visual, 
                declaration, visualLineage);

            switch (declarationValue)
            {
                case Func<IVisualElement, Object?> computed:
                    return new ComputedValueAssignment(visual, property, computed);

                default:
                    return new AppliedValueAssignment(visual, property, declarationValue);
            }
        }

        public static Object? GetDeclarationValue(IVisualElement visual,
                                                  IStyleDeclaration declaration,
                                                  IVisualLineage lineage)
        {
            switch (declaration)
            {
                case IStyleValueDeclaration scalar:
                    var res = scalar.Value;
                    res = ConvertDeclarationValue(res);
                    return res;

                //case QuadQuantityDeclaration quadQuan:
                //    return GetQuadValue(quadQuan, visual, lineage);

              
            }

            throw new NotImplementedException();
        }

        private static Object? ConvertDeclarationValue(Object? value)
        {
            switch (value)
            {
                case VerticalAlignType var:
                    switch (var)
                    {
                        case VerticalAlignType.Baseline:
                            break;
                        case VerticalAlignType.Length:
                            break;
                        case VerticalAlignType.Percent:
                            break;
                        case VerticalAlignType.Sub:
                            break;
                        case VerticalAlignType.Super:
                            break;
                        case VerticalAlignType.Top:
                            return VerticalAlignments.Top;
                            
                        case VerticalAlignType.TextTop:
                            break;
                        case VerticalAlignType.Middle:
                            return VerticalAlignments.Center;
                            
                        case VerticalAlignType.Bottom:
                            return VerticalAlignments.Bottom;
                            
                        case VerticalAlignType.TextBottom:
                            break;
                        case VerticalAlignType.Initial:
                            break;
                        case VerticalAlignType.Inherit:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    throw new NotImplementedException();

                case AppearanceType displayType:

                    switch (displayType)
                    {
                       case AppearanceType.Auto:
                           return Visibility.Visible;

                       case AppearanceType.None:
                           return Visibility.Hidden;
                        
                       
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

            }
            
            return value;
        }

        private static Double GetQuadValue(QuantityDeclaration declaration,
                                           IVisualLineage lineage)
        {
            if (declaration.Value.Units == LengthUnits.Px)
                return declaration.Value;

            if (declaration.Units == LengthUnits.None)
                return 0;

            throw new NotImplementedException();
        }

        private readonly Dictionary<IVisualElement, ValueCube> _renderPositions;
        private readonly IVisualBootstrapper _visualBootstrapper;
    }
}