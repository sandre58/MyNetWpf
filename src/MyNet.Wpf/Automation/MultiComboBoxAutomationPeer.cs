// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Automation
{
    public class MultiComboBoxAutomationPeer(MultiComboBox multiComboBox) : SelectorAutomationPeer(multiComboBox), IValueProvider, IExpandCollapseProvider
    {
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item) =>
            // Use the same peer as ListBox
            new ListBoxItemAutomationPeer(item, this);

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ComboBox;

        protected override string GetClassNameCore() => "MultiComboBox";

        public override object? GetPattern(PatternInterface patternInterface)
        {
            object? iface = null;

            if (patternInterface == PatternInterface.Value)
            {
                var owner = (MultiComboBox)Owner;
                if (owner.IsEditable) iface = this;
            }
            else
            {
                iface = patternInterface == PatternInterface.ExpandCollapse ? this : base.GetPattern(patternInterface);
            }

            return iface;
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            var children = base.GetChildrenCore();

            //  include text box part into children collection
            var owner = (MultiComboBox)Owner;
            var textBox = owner.EditableTextBoxSite;
            if (textBox != null)
            {
                var peer = CreatePeerForElement(textBox);
                if (peer != null)
                {
                    children ??= [];

                    children.Insert(0, peer);
                }
            }

            return children;
        }

        protected override void SetFocusCore()
        {
            var owner = (MultiComboBox)Owner;
            if (owner.Focusable)
            {
                if (!owner.Focus())
                {
                    //The fox might have went to the TextBox inside MultiComboBox if it is editable.
                    if (owner.IsEditable)
                    {
                        var tb = owner.EditableTextBoxSite;
                        if (tb == null || !tb.IsKeyboardFocused)
                            throw new InvalidOperationException();
                    }
                    else
                        throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        #region Value

        /// <summary>
        /// Request to set the value that this UI element is representing
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if the UI element was successfully set to the specified value</returns>
        void IValueProvider.SetValue(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var owner = (MultiComboBox)Owner;

            if (!owner.IsEnabled)
                throw new ElementNotEnabledException();

            owner.SetCurrentValue(MultiComboBox.TextProperty, value);
        }

        ///<summary>Value of a value control, as a a string.</summary>
        string IValueProvider.Value => ((MultiComboBox)Owner).Text;

        ///<summary>Indicates that the value can only be read, not modified.
        ///returns True if the control is read-only</summary>
        bool IValueProvider.IsReadOnly
        {
            get
            {
                var owner = (MultiComboBox)Owner;
                return !owner.IsEnabled || owner.IsReadOnly;
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
            }
        }

        // Note: see bug 1555137 for details.
        // Never inline, as we don't want to unnecessarily link the
        // automation DLL via the ISelectionProvider interface type initialization.
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseSelectionEvents(SelectionChangedEventArgs e)
        {
            var owner = (MultiComboBox)Owner;

            if (owner.Items.Count == 0)
            {
                //  ItemPeers.Count == 0 if children were never fetched.
                //  in the case, when client probably is not interested in the details
                //  of selection changes. but we still want to notify client about it.
                RaiseAutomationEvent(AutomationEvents.SelectionPatternOnInvalidated);

                return;
            }

            // These counters are bound to selection only numAdded = number of items just added and included in the current selection,
            // numRemoved = number of items just removed from the selection, numSelected = total number of items currently selected after addition and removal.
            var numSelected = owner.SelectedItems.Count;
            var numAdded = e.AddedItems.Count;
            var numRemoved = e.RemovedItems.Count;
            if (numSelected == 1 && numAdded == 1)
            {
                if (FindOrCreateItemAutomationPeer(owner.ItemContainerGenerator.ContainerFromItem(owner.SelectedItems[0])) is SelectorItemAutomationPeer peer)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                }
            }
            else
            {
                // If more than InvalidateLimit element change their state then we invalidate the selection container
                // Otherwise we fire Add/Remove from selection events
                if (numAdded + numRemoved > AutomationInteropProvider.InvalidateLimit)
                {
                    RaiseAutomationEvent(AutomationEvents.SelectionPatternOnInvalidated);
                }
                else
                {
                    int i;

                    for (i = 0; i < numAdded; i++)
                    {
                        if (FindOrCreateItemAutomationPeer(e.AddedItems[i]) is SelectorItemAutomationPeer peer)
                        {
                            peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                        }
                    }

                    for (i = 0; i < numRemoved; i++)
                    {
                        if (FindOrCreateItemAutomationPeer(e.RemovedItems[i]) is SelectorItemAutomationPeer peer)
                        {
                            peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                        }
                    }
                }
            }
        }

        #endregion Value

        #region ExpandCollapse

        /// <summary>
        /// Blocking method that returns after the element has been expanded.
        /// </summary>
        /// <returns>true if the node was successfully expanded</returns>
        void IExpandCollapseProvider.Expand()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            var owner = (MultiComboBox)Owner;
            owner.SetCurrentValue(MultiComboBox.IsDropDownOpenProperty, true);
        }

        /// <summary>
        /// Blocking method that returns after the element has been collapsed.
        /// </summary>
        /// <returns>true if the node was successfully collapsed</returns>
        void IExpandCollapseProvider.Collapse()
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            var owner = (MultiComboBox)Owner;
            owner.SetCurrentValue(MultiComboBox.IsDropDownOpenProperty, false);
        }

        ///<summary>indicates an element's current Collapsed or Expanded state</summary>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                var owner = (MultiComboBox)Owner;
                return owner.IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue) => RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);

        #endregion ExpandCollapse
    }
}
