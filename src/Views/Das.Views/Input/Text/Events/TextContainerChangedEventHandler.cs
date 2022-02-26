using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text.Events
{
    /// <summary>
    ///     The TextChangedEventHandler delegate is called with TextContainerChangedEventArgs every time
    ///     content is added to or removed from the TextContainer
    /// </summary>
    public delegate void TextContainerChangedEventHandler(Object sender,
                                                          TextContainerChangedEventArgs e);
}
