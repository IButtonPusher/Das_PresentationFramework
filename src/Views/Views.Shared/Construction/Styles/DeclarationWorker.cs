﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Enums;
using Das.Views.Declarations;

using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Application;
using Das.Views.Styles.Declarations;
using Das.Views.Styles.Declarations.Transform;
using Das.Views.Styles.Declarations.Transition;
using Das.Views.Styles.Transitions;
using Das.Views.Transforms;
using Das.Views.Transitions;

namespace Das.Views.Construction.Styles
{
    public class DeclarationWorker : IDeclarationWorker
    {
        

        public IEnumerable<IStyleValueAssignment> BuildStyleValueAssignments(IVisualElement visual,
            IVisualLineage visualLineage,
            IStyleRule rule,
            IVisualBootstrapper visualBootstrapper)
        {
            if (rule.Selector.TryGetContentAppendType(out var contentAppend))
            {
                yield return new PseudoVisualAssignment(visual, contentAppend,
                    rule, visualBootstrapper, visualLineage, BuildStyleValueAssignments);
            }
            else
            {
                foreach (var assignment in BuildStyleValueAssignments(visual, visualLineage, rule.Selector,
                    rule.Declarations))
                {
                    yield return assignment;
                }
            }
        }

        private static IEnumerable<IStyleValueAssignment> BuildStyleValueAssignments(IVisualElement visual,
                                                                                     IVisualLineage lineage,
                                                                                     IStyleSelector selector,
                                                                                     IEnumerable<IStyleDeclaration> declarations)
        {
            foreach (var declaration in declarations)
            {
                if (declaration is IStyleMultiValueDeclaration<TransitionDeclaration> transitions)
                {
                    ApplyTransitionsToVisual(visual, transitions);
                    continue;
                }

                var assignment = ApplyDeclarationToVisual(visual,
                    declaration);

                if (assignment != null)
                    yield return assignment;
            }
        }

        private static void ApplyTransitionsToVisual(IVisualElement visual,
                                                     IStyleMultiValueDeclaration<TransitionDeclaration> transitions)
        {
            foreach (var transition in transitions.Values)
            {
                if (visual.TryGetDependencyProperty(transition.TransitionProperty.Value,
                    out var dependencyProperty))
                {
                    var res = TransitionBuilder.BuildTransition(visual, dependencyProperty,
                        transition.Duration.ToTimeSpan(),
                        transition.Delay?.ToTimeSpan() ?? TimeSpan.Zero,
                        transition.TimingFunction?.Value ?? TransitionFunctionType.Ease);

                    dependencyProperty.AddTransition(visual, res);
                }
            }
        }

        private static IStyleValueAssignment? ApplyDeclarationToVisual(IVisualElement visual,
                                                                       IStyleDeclaration declaration)
        {
            if (visual.TryGetDependencyProperty(declaration.Property, 
                out var dependencyProperty))
            {
                //Debug.WriteLine("Setting " + visual.GetType().Name + "->" + dependencyProperty +
                //                " = " + declaration);

                return ApplyDeclarationToDependencyProperty(visual,
                    dependencyProperty, declaration);
            }

            //Debug.WriteLine("No dependency property found for " + declaration.Property +
            //                " on " + visual);
            return default;
        }

        private static IStyleValueAssignment ApplyDeclarationToDependencyProperty(IVisualElement visual,
                                                                 IDependencyProperty property,
                                                                 IStyleDeclaration declaration)
        {
            var declarationValue = GetDeclarationValue(declaration);

            switch (declarationValue)
            {
                case Func<IVisualElement, Object?> computed:
                    return new ComputedValueAssignment(visual, property, computed);

                default:
                    return new AppliedValueAssignment(visual, property, declarationValue);
            }
        }

        //private static IStyleValueAssignment<TVisual, TValue>  ApplyDeclarationToDependencyProperty<
        //    TVisual, TValue>(TVisual visual,
        //                     IDependencyProperty<TVisual, TValue>  property,
        //                     IStyle declaration)
        //where TVisual : IVisualElement
        //{
        //    var declarationValue = GetDeclarationValue(declaration);

        //    switch (declarationValue)
        //    {
        //        case Func<IVisualElement, Object?> computed:
        //            return new ComputedValueAssignment(visual, property, computed);

        //        default:
        //            return new AppliedValueAssignment<TVisual, TValue>(visual, property, declarationValue);
        //    }
        //}

        //public static TValue GetDeclarationValue<TVisual, TValue>(IStyleDeclaration<TValue> declaration)
        //{

        //}


        public static Object? GetDeclarationValue(IStyleDeclaration declaration)
        {
            switch (declaration)
            {
                case TransformDeclaration xform:
                    return ConvertTransform(xform).Value;

                case IStyleValueDeclaration scalar:
                    var res = scalar.Value;
                    res = ConvertDeclarationValue(res);
                    return res;

                //case IStyleMultiValueDeclaration multiScalar:
                //    return GetMultiScalarValues(multiScalar);

            }

            throw new NotImplementedException();
        }

        //private static IEnumerable<Object?> GetMultiScalarValues(IStyleMultiValueDeclaration multi)
        //{
        //    foreach (var value in multi.Values)
        //        yield return ConvertDeclarationValue(value);
        //}

        private static Object? ConvertDeclarationValue(Object? value)
        {
            switch (value)
            {
                case VerticalAlignType vat:
                    return ConvertVerticalAlign(vat);

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

        private static VerticalAlignments ConvertVerticalAlign(VerticalAlignType vat)
        {
            switch (vat)
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
        }

        private static ITransform ConvertTransform(TransformDeclaration xform)
        {
            Double[] paramValues;
            //Object?[] oParamValues;
            String[] sParamValues;

            switch (xform.TransformType)
            {
                case TransformType.Scale:
                    paramValues = xform.Function.GetParameterValues<Double>();

                    switch (paramValues.Length)
                    {
                        case 1:
                            return new ScaleTransform(paramValues[0]);

                        case 2:
                            return new ScaleTransform(paramValues[0], paramValues[1]);

                        default:
                            throw new InvalidOperationException();

                    }

                case TransformType.TranslateX:
                    sParamValues = xform.Function.GetParameterValues<String>();
                    if (sParamValues.Length != 1)
                        throw new InvalidOperationException();

                    var xValue = QuantifiedDouble.Parse(sParamValues[0]);

                    //var quantifiedValues = new QuantifiedDouble[sParamValues.Length];
                    //for (var c = 0; c < sParamValues.Length; c++)
                    //{
                    //    var current = sParamValues[c];
                    //    quantifiedValues[c] = QuantifiedDouble.Parse(current);
                    //}

                    return new TranslateTransform(xValue);

            }

            //xform.Function.
            throw new NotImplementedException();
        }
    }
}