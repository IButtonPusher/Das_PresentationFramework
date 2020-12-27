using System;
using System.Collections.Generic;

namespace Das.Views.Construction
{
    public interface IAttributeDictionary
    {
        Boolean TryGetAttributeValue(String key, 
                                     out String value);
        
        IEnumerable<KeyValuePair<String, String>> GetAllAttributes();
    }
}
