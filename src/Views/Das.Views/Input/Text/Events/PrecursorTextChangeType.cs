using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text.Events
{
    // These are possible changes added to a change list.
    // ElementAdded/Extracted don't make sense after multiple
    // changes are combined.
    public enum PrecursorTextChangeType
    {
        ContentAdded = TextChangeType.ContentAdded,
        ContentRemoved = TextChangeType.ContentRemoved,
        PropertyModified = TextChangeType.PropertyModified,
        ElementAdded,
        ElementExtracted
    }
}
