using System;
using System.Threading.Tasks;

namespace Das.Views.Wpf;

[AttributeUsage(AttributeTargets.Class)]
// ReSharper disable once ClassNeverInstantiated.Global
public class VisualShouldBeReusedAttribute : Attribute
{
}