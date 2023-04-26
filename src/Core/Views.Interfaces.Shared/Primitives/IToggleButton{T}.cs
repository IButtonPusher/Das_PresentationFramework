using System;
using Das.Views.Controls;

namespace Das.Views.Primitives;

public interface IToggleButton<T> : IButtonBase<T>,
                                    IToggleButton
{
}