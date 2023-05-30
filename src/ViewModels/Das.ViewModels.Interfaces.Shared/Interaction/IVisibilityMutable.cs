using System;
using System.Threading.Tasks;

namespace Das.Views.Mvvm;

public interface IVisibilityMutable : IViewModel
{
   Boolean IsVisible { get; set; }
}