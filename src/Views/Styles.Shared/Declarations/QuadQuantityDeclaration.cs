using Das.Views.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Styles.Declarations
{
    public abstract class QuadQuantityDeclaration : ValueDeclaration<QuantifiedThickness>
    {
        protected QuadQuantityDeclaration(String value,
            IStyleVariableAccessor variableAccessor, 
                                          DeclarationProperty property) 
            : base(GetValue(value), variableAccessor, property)
        {
        }

        private static QuantifiedThickness GetValue(String value)
        {
            var tokens = value.Split();

            QuantifiedDouble top, bottom, left, right, leftRight, topBottom;

            switch (tokens.Length)
            {
                case 1:
                    var all = QuantifiedDouble.Parse(tokens[0]);
                    return new QuantifiedThickness(all);

                case 2:
                    leftRight = QuantifiedDouble.Parse(tokens[1]);
                    topBottom = QuantifiedDouble.Parse(tokens[0]);
                    return new QuantifiedThickness(leftRight, topBottom);

                case 4:
                    
                    top = QuantifiedDouble.Parse(tokens[0]);
                    right = QuantifiedDouble.Parse(tokens[1]);
                    bottom = QuantifiedDouble.Parse(tokens[2]);
                    left = QuantifiedDouble.Parse(tokens[3]);
                    return new QuantifiedThickness(left, top, right, bottom);

                case 3:
                    top = QuantifiedDouble.Parse(tokens[0]);
                    right = QuantifiedDouble.Parse(tokens[1]);
                    left = right;
                    bottom = QuantifiedDouble.Parse(tokens[2]);
                    return new QuantifiedThickness(left, top, right, bottom);
            }

            throw new InvalidOperationException();
        }
    }

    //public abstract class QuadQuantityDeclaration : DeclarationBase
    //{
    //    public QuadQuantityDeclaration(String value,
    //                                   IStyleVariableAccessor variableAccessor, 
    //                                   DeclarationProperty property,
    //                                   DeclarationProperty topProperty,
    //                                   DeclarationProperty rightProperty,
    //                                   DeclarationProperty bottomProperty,
    //                                   DeclarationProperty leftProperty) 
    //        : base(variableAccessor, property)
    //    {
    //        var tokens = value.Split();

    //        switch (tokens.Length)
    //        {
    //            case 4:
    //                Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
    //                Right = new QuantityDeclaration(tokens[1], variableAccessor, rightProperty);
    //                Bottom = new QuantityDeclaration(tokens[2], variableAccessor, bottomProperty);
    //                Left = new QuantityDeclaration(tokens[3], variableAccessor, leftProperty);
    //                break;
                
    //            case 3:
    //                Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
                    
    //                Right = new QuantityDeclaration(tokens[1], variableAccessor, rightProperty);
    //                Left = new QuantityDeclaration(tokens[1], variableAccessor, leftProperty);
                    
    //                Bottom = new QuantityDeclaration(tokens[2], variableAccessor, bottomProperty);
    //                break;
                
    //            case 2:
    //                Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
    //                Bottom = new QuantityDeclaration(tokens[0], variableAccessor, bottomProperty);
                    
    //                Right = new QuantityDeclaration(tokens[1], variableAccessor, rightProperty);
    //                Left = new QuantityDeclaration(tokens[1], variableAccessor, leftProperty);
    //                break;
                
    //            case 1:
    //                Top = new QuantityDeclaration(tokens[0], variableAccessor, topProperty);
    //                Bottom = new QuantityDeclaration(tokens[0], variableAccessor, bottomProperty);
    //                Right = new QuantityDeclaration(tokens[0], variableAccessor, rightProperty);
    //                Left = new QuantityDeclaration(tokens[0], variableAccessor, leftProperty);
    //                break;
                
    //            default:
    //                throw new NotImplementedException();
    //        }

    //        //var allMyUnits = new HashSet<LengthUnits>
    //        //{
    //        //    Top.Units, Right.Units,
    //        //    Bottom.Units, Left.Units
    //        //};

    //        //if (allMyUnits.Count == 1)
    //        //{
    //        //    Units = allMyUnits.First();
    //        //}
    //        //else
    //        //{
    //        //    allMyUnits.Remove(LengthUnits.None);

    //        //    if (allMyUnits.Count == 1)
    //        //        Units = allMyUnits.First();
    //        //    else Units = LengthUnits.Invalid;
    //        //}

    //        //if (Top.Units == Right.Units && Right.Units == Bottom.Units &&
    //        //    Bottom.Units == Left.Units)
    //        //{
    //        //    Units = Top.Units;
    //        //}
    //        //else
    //        //    Units = LengthUnits.Invalid;

    //    }
        
    //    ///// <summary>
    //    ///// The units used by all 4 values.  If they aren't all the same, returns Invalid
    //    ///// </summary>
    //    //public LengthUnits Units { get; }

    //    public QuantityDeclaration Left { get; }
        
    //    public QuantityDeclaration Top { get; }
        
    //    public QuantityDeclaration Right { get; }
        
    //    public QuantityDeclaration Bottom { get; }
    //}
}
