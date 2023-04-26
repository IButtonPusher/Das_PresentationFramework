using System;
using System.Collections.Generic;

namespace Das.Views.Construction;

/// <summary>
/// Metadata extracted from markup like xml or json
/// </summary>
public interface IAttributeDictionary
{
   Boolean TryGetAttributeValue(String key, 
                                out String value);
        
   IEnumerable<KeyValuePair<String, String>> GetAllAttributes();
}