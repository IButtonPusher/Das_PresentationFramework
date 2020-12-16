using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Extensions;
using Das.Views.Core.Drawing;
using Das.Views.Core.Enums;
using Das.Views.Core.Geometry;
using Das.Views.DataBinding;
using Das.Views.Input;
using Das.Views.ItemsControls;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;

namespace Das.Views.Defaults
{
    public class DefaultTabHeaderTemplate : DefaultContentTemplate
    {
        public DefaultTabHeaderTemplate(IVisualBootstrapper visualBootstrapper,
                                        ITabControl tabControl)
            : base(visualBootstrapper)
        {
            _itemsControl = tabControl;
        }

        public override IVisualElement? BuildVisual(Object? dataContext)
        {
            return new DefaultTabHeaderPanel(_itemsControl, _visualBootstrapper);
        }


        private readonly ITabControl _itemsControl;
    }
}