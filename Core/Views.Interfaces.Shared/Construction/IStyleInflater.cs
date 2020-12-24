using System;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Construction
{
    public interface IStyleInflater
    {
        ICascadingStyle InflateCss(String css);

        Task<ICascadingStyle> InflateResourceCssAsync(String resourceName);
        
        ICascadingStyle InflateXml(String xml);

        Task<ICascadingStyle> InflateResourceXmlAsync(String resourceName);
    }
}
