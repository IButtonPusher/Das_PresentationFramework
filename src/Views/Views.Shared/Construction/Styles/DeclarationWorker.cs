using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Core;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.Declarations;

using Das.Views.Rendering;
using Das.Views.Styles;
using Das.Views.Styles.Declarations;

namespace Das.Views.Construction.Styles
{
    public class DeclarationWorker
    {
        public DeclarationWorker(Dictionary<IVisualElement, ValueCube> renderPositions)
        {
            _renderPositions = renderPositions;
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

        //private QuantifiedThickness GetQuadValue(QuadQuantityDeclaration quadQuan,
        //                                         IVisualElement visual,
        //                                         IVisualLineage lineage)
        //{
        //    switch (quadQuan.Units)
        //    {
        //        case LengthUnits.Px:
        //            var left = GetQuadValue(quadQuan.Left, lineage);
        //            var top = GetQuadValue(quadQuan.Top, lineage);
        //            var right = GetQuadValue(quadQuan.Right, lineage);
        //            var bottom = GetQuadValue(quadQuan.Bottom, lineage);

        //            return new Thickness(left, top, right, bottom);

        //        case LengthUnits.Percent:

        //            return new VisualPercentThickness(visual,
        //                quadQuan.Left.Value,
        //                quadQuan.Top.Value,
        //                quadQuan.Right.Value,
        //                quadQuan.Bottom.Value, _renderPositions);

        //        case LengthUnits.None:
        //            return Thickness.Empty;
        //    }

        //    throw new NotImplementedException();
        //}

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
    }
}