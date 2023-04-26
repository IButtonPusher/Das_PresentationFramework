using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry;

public interface ICube : IRectangle
{
   Double Depth { get; }
}