using System;
using System.Collections.Generic;
using Das.Views.DataBinding;

namespace Das.Views.Construction
{
    public interface IBindingBuilder
    {
        Dictionary<String, IDataBinding> GetBindingsDictionary(IMarkupNode node,
                                                               Type? dataContextType,
                                                               Dictionary<String, String> nameSpaceAssemblySearch);

        Type? InferDataContextTypeFromBindings(IEnumerable<IDataBinding> bindings,
                                               Type? currentGenericArg);
    }
    
    
}
