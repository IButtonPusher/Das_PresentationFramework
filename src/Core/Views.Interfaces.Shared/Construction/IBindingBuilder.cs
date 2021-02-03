using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.DataBinding;

namespace Das.Views.Construction
{
    public interface IBindingBuilder
    {
        Task<Dictionary<String, IPropertyBinding>> GetBindingsDictionaryAsync(IMarkupNode node,
                                                                              Type? dataContextType,
                                                                              Dictionary<String, String> nameSpaceAssemblySearch);

        //Dictionary<String, IDataBinding> GetBindingsDictionary(IMarkupNode node,
        //                                                       Type? dataContextType,
        //                                                       Dictionary<String, String> nameSpaceAssemblySearch);

        Type? InferDataContextTypeFromBindings(IEnumerable<IDataBinding> bindings,
                                               Type? currentGenericArg);
    }
}
