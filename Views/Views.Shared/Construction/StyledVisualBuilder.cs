using System;
using System.Threading.Tasks;
using Das.Views.Construction.Styles;
using Das.Views.Rendering;
using Das.Views.Styles;

namespace Das.Views.Construction
{
    public class StyledVisualBuilder : IStyledVisualBuilder
    {
        private readonly IVisualStyleProvider _styleProvider;

        public StyledVisualBuilder(IVisualStyleProvider styleProvider)
        {
            _styleProvider = styleProvider;
        }
        
        
        public async Task ApplyStylesToVisualAsync(IVisualElement visual, 
                                        String? styleClassName, 
                                        IVisualLineage visualLineage)
        {
            if (!String.IsNullOrEmpty(styleClassName))
            {
                var classStyles = _styleProvider.GetStylesByClassNameAsync(styleClassName!);

                await foreach (var style in classStyles)
                {

                }
            }

            
        }
    }
}
