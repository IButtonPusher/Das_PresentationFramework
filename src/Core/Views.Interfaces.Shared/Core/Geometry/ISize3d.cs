using System;
using System.Threading.Tasks;

namespace Das.Views.Core.Geometry;

public interface ISize3d : ISize
{
   Double Depth { get; }
}