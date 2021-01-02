using System;

namespace Das.Views.Styles.Application
{
    public interface IPropertyValueAssignment : IStyleValueAssignment
    {
        IDependencyProperty Property {get;}
    }
}
