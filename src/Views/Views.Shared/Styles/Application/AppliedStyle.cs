using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Das.ViewModels.Collections;

namespace Das.Views.Styles.Application
{
    public class AppliedStyle : IAppliedStyle
    {
        public AppliedStyle(IStyleSheet styleTemplate)
        {
            StyleTemplate = styleTemplate;
            _appliedRules = new List<AppliedStyleRule>();
            _monitoredProperties = new DoubleConcurrentDictionary<IVisualElement, IDependencyProperty, Byte>();
        }

        public IStyleSheet StyleTemplate { get; }

        IEnumerable<IAppliedStyleRule> IAppliedStyle.AppliedRules => _appliedRules;

        public void Execute(Boolean isUpdate)
        {
            foreach (var rule in _appliedRules)
            {
                rule.Execute(isUpdate);
            }
        }

        public Boolean HasAppliedRules => _appliedRules.Count > 0;

        public void AddAppliedRule(AppliedStyleRule rule)
        {
            _appliedRules.Add(rule);
        }

        public void EnsureInverseForFilteredSelectors()
        {
            var isAnyInverted = false;

            foreach (var rule in _appliedRules)
            {
                if (!rule.IsFilteringOnVisualState())
                    continue;

                var asUnfilteredSelector = rule.RuleTemplate.Selector.ToUnfiltered();

                foreach (var applied in _appliedRules)
                {
                    if (ReferenceEquals(applied, rule))
                        continue;

                    if (!applied.RuleTemplate.Selector.Equals(asUnfilteredSelector))
                        continue;

                    if (applied.EnsureDefaultAssignments(rule))
                        isAnyInverted = true;

                    break;
                }
            }

            if (!isAnyInverted)
                return;

            var sorted = new List<AppliedStyleRule>(
                _appliedRules.OrderBy(r => r.RuleTemplate.Selector.IsFilteringOnVisualState()));

            _appliedRules.Clear();
            _appliedRules.AddRange(sorted);
        }

        public void MonitorPropertyChange(IDependencyProperty property,
                                          IVisualElement visual)
        {
            if (!_monitoredProperties.TryAdd(visual, property, 1))
                return;

            property.AddOnChangedHandler(visual, _ => Execute(true));
        }

        private readonly List<AppliedStyleRule> _appliedRules;

        private readonly DoubleConcurrentDictionary<IVisualElement, IDependencyProperty, Byte> _monitoredProperties;
    }
}
