using System;
using System.Collections.Generic;
using Das.ViewModels.Collections;

namespace Das.Views.Styles.Application
{
    public class AppliedStyle : IAppliedStyle
    {
        public AppliedStyle(IStyleSheet styleTemplate)
        {
            StyleTemplate = styleTemplate;
            AppliedRules = new List<IAppliedStyleRule>();
            _monitoredProperties = new DoubleConcurrentDictionary<IVisualElement, IDependencyProperty, Byte>();
        }

        public IStyleSheet StyleTemplate { get; }

        public List<IAppliedStyleRule> AppliedRules { get; }

        IEnumerable<IAppliedStyleRule> IAppliedStyle.AppliedRules => AppliedRules;

        private DoubleConcurrentDictionary<IVisualElement, IDependencyProperty, Byte> _monitoredProperties;

        public void MonitorPropertyChange(IDependencyProperty property,
                                          IVisualElement visual)
        {
            if (!_monitoredProperties.TryAdd(visual, property, 1))
                return;

            property.AddOnChangedHandler(visual, d => Execute());
        }

        public void Execute()
        {
            foreach (var rule in AppliedRules)
            {
                rule.Execute();
            }
        }
    }
}
