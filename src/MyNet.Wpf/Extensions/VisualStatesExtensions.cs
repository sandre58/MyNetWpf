// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MyNet.Wpf.Controls.VisualStates;

namespace MyNet.Wpf.Extensions
{
    public static class VisualStatesExtensions
    {
        public static IEnumerable<VisualStateGroup> GetActiveVisualStateGroups(this FrameworkElement element) => element.GetVisualStateGroupsByName(IndicatorVisualStateGroupNames.ActiveStates.Name);

        public static IEnumerable<VisualState> GetActiveVisualStates(this FrameworkElement element) => element
.GetActiveVisualStateGroups().GetAllVisualStatesByName(IndicatorVisualStateNames.ActiveState.Name);

        public static IEnumerable<VisualState> GetAllVisualStatesByName(
            this IEnumerable<VisualStateGroup> visualStateGroups, string name) => visualStateGroups.SelectMany(vsg => vsg.GetVisualStatesByName(name));

        public static IEnumerable<VisualState> GetVisualStatesByName(this VisualStateGroup visualStateGroup,
            string name)
        {
            if (visualStateGroup is null)
            {
                return [];
            }

            var visualStates = visualStateGroup.GetVisualStates();

            return string.IsNullOrWhiteSpace(name) ? visualStates : visualStates.Where(vs => vs.Name == name);
        }

        public static IEnumerable<VisualStateGroup> GetVisualStateGroupsByName(this FrameworkElement element,
            string name)
        {
            var groups = VisualStateManager.GetVisualStateGroups(element);

            if (groups is null)
            {
                return [];
            }

            IEnumerable<VisualStateGroup> castedVisualStateGroups;

            try
            {
                castedVisualStateGroups = groups.Cast<VisualStateGroup>().ToArray();
                if (!castedVisualStateGroups.Any())
                {
                    return [];
                }
            }
            catch (InvalidCastException)
            {
                return [];
            }

            return string.IsNullOrWhiteSpace(name)
                ? castedVisualStateGroups
                : castedVisualStateGroups.Where(vsg => vsg.Name == name);
        }

        public static IEnumerable<VisualState> GetVisualStates(this VisualStateGroup visualStateGroup) => visualStateGroup is null
                ? []
                : visualStateGroup.States.Count == 0 ? [] : visualStateGroup.States.Cast<VisualState>();
    }
}
