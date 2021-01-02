﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.ViewModels.Collections;

namespace Das.Views.Styles.Application
{
    public class AppliedStyle : IAppliedStyle
    {
        public AppliedStyle(IStyleSheet styleTemplate)
        {
            StyleTemplate = styleTemplate;
            AppliedRules = new List<AppliedStyleRule>();
            _monitoredProperties = new DoubleConcurrentDictionary<IVisualElement, IDependencyProperty, Byte>();
        }

        public void EnsureInverseForFilteredSelectors()
        {
            foreach (var rule in AppliedRules)
            {
                if (!rule.IsFilteringOnVisualState())
                    continue;

                var asUnfilteredSelector = rule.RuleTemplate.Selector.ToUnfiltered();

                foreach (var apple in AppliedRules)
                {
                    if (ReferenceEquals(apple, rule))
                        continue;

                    if (!apple.RuleTemplate.Selector.Equals(asUnfilteredSelector))
                        continue;

                    apple.EnsureDefaultAssignments(rule);

                    //foreach (var filteredAssignment in rule.Assignments)
                    //{
                    //    if (!apple.HasOverlappingAssignment(filteredAssignment))
                    //    {
                    //        apple.AddDefaultAssignment(filteredAssignment);
                    //    }
                    //}

                    break;
                }

            }
        }

        public IStyleSheet StyleTemplate { get; }

        IEnumerable<IAppliedStyleRule> IAppliedStyle.AppliedRules => AppliedRules;

        public void Execute(Boolean isUpdate)
        {
            foreach (var rule in AppliedRules)
                rule.Execute(isUpdate);
        }

        public List<AppliedStyleRule> AppliedRules { get; }

        public void MonitorPropertyChange(IDependencyProperty property,
                                          IVisualElement visual)
        {
            if (!_monitoredProperties.TryAdd(visual, property, 1))
                return;

            System.Diagnostics.Debug.WriteLine("executing due to change in " + property);

            property.AddOnChangedHandler(visual, d => Execute(true));
        }

        private readonly DoubleConcurrentDictionary<IVisualElement, IDependencyProperty, Byte> _monitoredProperties;
    }
}