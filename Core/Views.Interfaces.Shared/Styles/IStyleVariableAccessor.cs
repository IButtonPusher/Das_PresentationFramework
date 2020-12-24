using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views.Styles
{
    /// <summary>
    /// Provides get and set access to style variables
    /// </summary>
    public interface IStyleVariableAccessor
    {
        T GetVariableValue<T>(String variableName);

        void SetVariableValue<T>(String variableName,
                                 T value);

        void SetVariableValue<T>(String variableName,
                                 Func<T> value);
    }
}
