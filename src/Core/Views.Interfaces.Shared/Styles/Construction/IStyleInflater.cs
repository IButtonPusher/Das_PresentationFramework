using System;
using System.Threading.Tasks;
using Das.Views.Styles;

namespace Das.Views.Construction;

public interface IStyleInflater
{
   ICascadingStyle InflateCss(String css);

   Task<ICascadingStyle> InflateResourceCssAsync(String resourceName);

   ICascadingStyle InflateResourceCss(String resourceName);
        
   IStyleSheet InflateXml(String xml);

   Task<IStyleSheet> InflateResourceXmlAsync(String resourceName);
}