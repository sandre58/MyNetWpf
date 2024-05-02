// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Automation;
using MyNet.Wpf.Extensions;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls
{
    [Localizability(LocalizationCategory.ComboBox)]
    [TemplatePart(Name = EditableTextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = ClearButtonTemplateName, Type = typeof(Popup))]
    [TemplatePart(Name = SelectedItemsControlTemplateName, Type = typeof(MultiComboBoxSelectedItems))]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MultiComboBoxItem))]
    public class MultiComboBox : ListBox
    {
        private enum CacheBits
        {
            HasMouseEnteredItemsHost = 0x02,
            IsContextMenuOpen = 0x04,
            UpdatingText = 0x08,
            IsWaitingForTextComposition = 0x20,
        }

        private const string SelectedItemsControlTemplateName = "PART_MultiComboBoxSelectedItems";
        private const string EditableTextBoxTemplateName = "PART_EditableTextBox";
        private const string PopupTemplateName = "PART_Popup";
        private const string ClearButtonTemplateName = "PART_ClearButton";
        private const string RemoveItemButtonTemplateName = "PART_RemoveItemButton";
        private BitVector32 _cacheValid = new(0);
        private int _textBoxSelectionStart; // the location of selection before call to TextUpdated.
        private MatchedTextInfo? _currentSearchSelectedItem;

        internal TextBox? EditableTextBoxSite { get; set; }
        internal Popup? DropDownPopup { get; set; }
        internal Button? ClearButton { get; set; }
        internal MultiComboBoxSelectedItems? SelectedItemsControl { get; set; }

        /// <summary>
        ///     If control has a scrollviewer in its style and has a custom keyboard scrolling behavior when HandlesScrolling should return true.
        /// Then ScrollViewer will not handle keyboard input and leave it up to the control.
        /// </summary>
        protected override bool HandlesScrolling => true;

        private bool HasCapture => Mouse.Captured == this;

        private bool HasMouseEnteredItemsHost
        {
            get => _cacheValid[(int)CacheBits.HasMouseEnteredItemsHost];
            set => _cacheValid[(int)CacheBits.HasMouseEnteredItemsHost] = value;
        }

        private bool IsContextMenuOpen
        {
            get => _cacheValid[(int)CacheBits.IsContextMenuOpen];
            set => _cacheValid[(int)CacheBits.IsContextMenuOpen] = value;
        }

        // Used to indicate that the Text Properties are changing
        // Don't reenter callbacks
        private bool UpdatingText
        {
            get => _cacheValid[(int)CacheBits.UpdatingText];
            set => _cacheValid[(int)CacheBits.UpdatingText] = value;
        }

        // A text composition is active (in the EditableTextBoxSite);  postpone Text changes
        private bool IsWaitingForTextComposition
        {
            get => _cacheValid[(int)CacheBits.IsWaitingForTextComposition];
            set => _cacheValid[(int)CacheBits.IsWaitingForTextComposition] = value;
        }

        private static readonly DependencyPropertyKey HasSelectedItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasSelectedItems), typeof(bool), typeof(MultiComboBox),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty HasSelectedItemsProperty = HasSelectedItemsPropertyKey.DependencyProperty;

        [Bindable(false)]
        [Browsable(false)]
        public bool HasSelectedItems
        {
            get => (bool)GetValue(HasSelectedItemsProperty);
            private set => SetValue(HasSelectedItemsPropertyKey, value);
        }

        static MultiComboBox()
        {
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));

            // Disable tooltips on combo box when it is open
            ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceToolTipIsEnabled)));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(typeof(MultiComboBox)));

            IsTextSearchEnabledProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(true));

            EventManager.RegisterClassHandler(typeof(MultiComboBox), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture), true);
            EventManager.RegisterClassHandler(typeof(MultiComboBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true); // call us even if the transparent button in the style gets the click.
            EventManager.RegisterClassHandler(typeof(MultiComboBox), Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButtonUp), true);
            EventManager.RegisterClassHandler(typeof(MultiComboBox), Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
            EventManager.RegisterClassHandler(typeof(MultiComboBox), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseButtonDown));
            EventManager.RegisterClassHandler(typeof(MultiComboBox), GotFocusEvent, new RoutedEventHandler(OnGotFocus), true); // call us even if textbox in the style get focus

            // Listen for ContextMenu openings/closings
            EventManager.RegisterClassHandler(typeof(MultiComboBox), ContextMenuService.ContextMenuOpeningEvent, new ContextMenuEventHandler(OnContextMenuOpen), true);
            EventManager.RegisterClassHandler(typeof(MultiComboBox), ContextMenuService.ContextMenuClosingEvent, new ContextMenuEventHandler(OnContextMenuClose), true);
        }

        /// <summary>
        /// Called when the Template's tree has been generated
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DropDownPopup != null)
            {
                DropDownPopup.Closed -= OnPopupClosed;
            }

            EditableTextBoxSite = GetTemplateChild(EditableTextBoxTemplateName) as TextBox;
            DropDownPopup = GetTemplateChild(PopupTemplateName) as Popup;
            ClearButton = GetTemplateChild(ClearButtonTemplateName) as Button;
            SelectedItemsControl = GetTemplateChild(SelectedItemsControlTemplateName) as MultiComboBoxSelectedItems;

            // EditableTextBoxSite should have been set by now if it's in the visual tree
            if (EditableTextBoxSite != null)
            {
                EditableTextBoxSite.TextChanged += new TextChangedEventHandler(OnEditableTextBoxTextChanged);
                EditableTextBoxSite.SelectionChanged += new RoutedEventHandler(OnEditableTextBoxSelectionChanged);
                EditableTextBoxSite.PreviewTextInput += new TextCompositionEventHandler(OnEditableTextBoxPreviewTextInput);
                EditableTextBoxSite.KeyDown += new KeyEventHandler(OnEditableTextBoxKeyDown);
                EditableTextBoxSite.LostFocus += new RoutedEventHandler(OnEditableTextBoxLoastFocus);
            }

            if (DropDownPopup != null)
            {
                DropDownPopup.Closed += OnPopupClosed;
            }

            if (ClearButton != null)
            {
                ClearButton.Click += OnClearButtonClick;
            }
        }

        #region Properties

        /// <summary>
        ///     DependencyProperty for MaxDropDownHeight
        /// </summary>
        public static readonly DependencyProperty ShowRemoveItemButtonProperty
            = DependencyProperty.Register("ShowRemoveItemButton", typeof(bool), typeof(MultiComboBox), new FrameworkPropertyMetadata(true));

        /// <summary>
        ///     The maximum height of the popup
        /// </summary>
        [Bindable(true), Category("Layout")]
        public bool ShowRemoveItemButton
        {
            get => (bool)GetValue(ShowRemoveItemButtonProperty);
            set => SetValue(ShowRemoveItemButtonProperty, value);
        }

        /// <summary>
        ///     DependencyProperty for MaxDropDownHeight
        /// </summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty
            = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(MultiComboBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     The maximum height of the popup
        /// </summary>
        [Bindable(true), Category("Layout")]
        public DataTemplate SelectedItemTemplate
        {
            get => (DataTemplate)GetValue(SelectedItemTemplateProperty);
            set => SetValue(SelectedItemTemplateProperty, value);
        }

        /// <summary>
        ///     DependencyProperty for MaxDropDownHeight
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty
            = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MultiComboBox),
                                          new FrameworkPropertyMetadata(320d));

        /// <summary>
        ///     The maximum height of the popup
        /// </summary>
        [Bindable(true), Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        /// <summary>
        /// DependencyProperty for IsDropDownOpen
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
                DependencyProperty.Register(
                        "IsDropDownOpen",
                        typeof(bool),
                        typeof(MultiComboBox),
                        new FrameworkPropertyMetadata(
                                false,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnIsDropDownOpenChanged),
                                new CoerceValueCallback(CoerceIsDropDownOpen)));

        /// <summary>
        /// Whether or not the "popup" for this control is currently open
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                var cb = (MultiComboBox)d;
                if (!cb.IsLoaded)
                {
                    cb.RegisterToOpenOnLoad();
                    return false;
                }
            }

            return value;
        }

        private static object CoerceToolTipIsEnabled(DependencyObject d, object value)
        {
            var cb = (MultiComboBox)d;
            return !cb.IsDropDownOpen && (bool)value;
        }

        private void RegisterToOpenOnLoad() => Loaded += new RoutedEventHandler(OpenOnLoad);

        private void OpenOnLoad(object sender, RoutedEventArgs e) =>
            // Open combobox after it has rendered (Loaded is fired before 1st render)
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object param)
            {
                CoerceValue(IsDropDownOpenProperty);

                return null;
            }), null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropDownOpened(EventArgs e) => DropDownOpened?.Invoke(this, e);

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropDownClosed(EventArgs e) => DropDownClosed?.Invoke(this, e);

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = (MultiComboBox)d;

            comboBox.HasMouseEnteredItemsHost = false;

            var newValue = (bool)e.NewValue;
            var oldValue = !newValue;

            // Fire accessibility event
            if (UIElementAutomationPeer.FromElement(comboBox) is MultiComboBoxAutomationPeer peer)
            {
                peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
            }

            if (newValue)
            {
                // When the drop down opens, take capture
                _ = Mouse.Capture(comboBox, CaptureMode.SubTree);

                // Select text if editable
                if (comboBox.IsEditable && comboBox.EditableTextBoxSite != null)
                    comboBox.EditableTextBoxSite.SelectAll();

                comboBox.OnDropDownOpened(EventArgs.Empty);
            }
            else
            {
                // If focus is within the subtree, make sure we have the focus so that focus isn't in the disposed hwnd
                if (comboBox.IsKeyboardFocusWithin)
                {
                    if (comboBox.IsEditable)
                    {
                        if (comboBox.EditableTextBoxSite != null && !comboBox.EditableTextBoxSite.IsKeyboardFocusWithin)
                        {
                            _ = comboBox.Focus();
                        }
                    }
                    else
                    {
                        // It's not editable, make sure the combobox has focus
                        _ = comboBox.Focus();
                    }
                }

                if (comboBox.HasCapture)
                {
                    _ = Mouse.Capture(null);
                }

                // No Popup in the style so fire closed now
                if (comboBox.DropDownPopup == null)
                {
                    comboBox.OnDropDownClosed(EventArgs.Empty);
                }
            }

            comboBox.CoerceValue(ToolTipService.IsEnabledProperty);
        }

        private void OnPopupClosed(object? source, EventArgs e) => OnDropDownClosed(EventArgs.Empty);

        /// <summary>
        /// DependencyProperty for IsEditable
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
                DependencyProperty.Register(
                        "IsEditable",
                        typeof(bool),
                        typeof(MultiComboBox),
                        new FrameworkPropertyMetadata(
                                false));

        /// <summary>
        ///     True if this ComboBox is editable.
        /// </summary>
        /// <value></value>
        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        /// <summary>
        ///     DependencyProperty for StaysOpenOnEdit
        /// </summary>
        public static readonly DependencyProperty StaysOpenOnEditProperty
            = DependencyProperty.Register("StaysOpenOnEdit", typeof(bool), typeof(MultiComboBox),
                                          new FrameworkPropertyMetadata(false));

        /// <summary>
        ///     Determines whether the ComboBox will remain open when clicking on
        ///     the text box when the drop down is open
        /// </summary>
        /// <value></value>
        public bool StaysOpenOnEdit
        {
            get => (bool)GetValue(StaysOpenOnEditProperty);
            set => SetValue(StaysOpenOnEditProperty, value);
        }

        /// <summary>
        ///     DependencyProperty for Text
        /// </summary>
        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register(
                        "Text",
                        typeof(string),
                        typeof(MultiComboBox),
                        new FrameworkPropertyMetadata(
                                string.Empty,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        ///     The text of the currently selected item.  When there is no SelectedItem and IsEditable is true
        ///     this is the text entered in the text box.  When IsEditable is false, this value represent the string version of the selected item.
        /// </summary>
        /// <value></value>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        ///     DependencyProperty for Text
        /// </summary>
        public static readonly DependencyProperty PrefixProperty =
                DependencyProperty.Register(
                        "Prefix",
                        typeof(string),
                        typeof(MultiComboBox),
                        new FrameworkPropertyMetadata(
                                string.Empty,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>
        ///     The text of the currently selected item.  When there is no SelectedItem and IsEditable is true
        ///     this is the text entered in the text box.  When IsEditable is false, this value represent the string version of the selected item.
        /// </summary>
        /// <value></value>
        public string Prefix
        {
            get => (string)GetValue(PrefixProperty);
            set => SetValue(PrefixProperty, value);
        }

        /// <summary>
        ///     DependencyProperty for the IsReadOnlyProperty
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
                TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(MultiComboBox));

        /// <summary>
        ///     When the ComboBox is Editable, if the TextBox within it is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// DependencyProperty for ShouldPreserveUserEnteredPrefix.
        /// </summary>
        ///

        public static readonly DependencyProperty ShouldPreserveUserEnteredPrefixProperty =
               DependencyProperty.Register(
                       "ShouldPreserveUserEnteredPrefix",
                       typeof(bool),
                       typeof(MultiComboBox),
                       new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Whether or not the user entered prefix should be preserved in auto lookup matched text.
        /// </summary>
        public bool ShouldPreserveUserEnteredPrefix
        {
            get => (bool)GetValue(ShouldPreserveUserEnteredPrefixProperty);
            set => SetValue(ShouldPreserveUserEnteredPrefixProperty, value);
        }

        #endregion Properties

        #region Events

        public event EventHandler<EventArgs>? DropDownOpened;

        public event EventHandler<EventArgs>? DropDownClosed;

        public event EventHandler<EventArgs>? TextChanged;

        #endregion Events

        #region Capture

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Mouse.Captured == null && e.OriginalSource == this)
            {
                EditableTextBoxSite?.Clear();
            }
            base.OnLostFocus(e);
        }

        private void OnEditableTextBoxLoastFocus(object sender, RoutedEventArgs e)
        {
            if (Mouse.Captured == null && !IsKeyboardFocusWithin)
            {
                EditableTextBoxSite?.Clear();
            }
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var comboBox = (MultiComboBox)sender;

            if (!comboBox.HasCapture)
            {
                if (((DependencyObject)e.OriginalSource).IsDescendantOf(comboBox))
                {
                    // Take capture if one of our children gave up capture (by closing their drop down)
                    if (comboBox.IsDropDownOpen && Mouse.Captured == null)
                    {
                        _ = Mouse.Capture(comboBox, CaptureMode.SubTree);
                        e.Handled = true;
                    }
                }
                else
                {
                    comboBox.Close();
                }
            }
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var comboBox = (MultiComboBox)sender;

            // If we (or one of our children) are clicked, claim the focus (don't steal focus if our context menu is clicked)
            if (!comboBox.IsContextMenuOpen && !comboBox.IsKeyboardFocusWithin)
            {
                _ = comboBox.Focus();
            }

            var originalSource = (Visual)e.OriginalSource;

            if (comboBox.ClearButton != null && Mouse.Captured == comboBox.ClearButton && originalSource.IsDescendantOf(comboBox.ClearButton))
            {
                return;
            }

            if (Mouse.Captured == comboBox && e.OriginalSource == comboBox)
            {
                comboBox.Close();
            }

            e.Handled = true;   // Always handle so that parents won't take focus away
        }

        private static void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var comboBox = (MultiComboBox)sender;

            var originalSource = (Visual)e.OriginalSource;

            if (comboBox.SelectedItemsControl != null && originalSource.IsDescendantOf(comboBox.SelectedItemsControl) && originalSource is ButtonBase button && button.Name == RemoveItemButtonTemplateName)
            {
                var item = button.FindVisualParent<MultiComboBoxSelectedItem>()?.DataContext;
                comboBox.RemoveSelectedItem(item);
            }
        }

        private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var comboBox = (MultiComboBox)sender;

            if (comboBox.IsEditable)
            {
                Visual? textBox = comboBox.EditableTextBoxSite;

                if (e.OriginalSource is Visual originalSource && textBox != null
                    && originalSource.IsDescendantOf(textBox))
                {
                    if (comboBox.IsDropDownOpen && !comboBox.StaysOpenOnEdit)
                    {
                        // When combobox is not editable, clicks anywhere outside
                        // the combobox will close it.  When the combobox is editable
                        // then clicking the text box should close the combobox as well.
                        comboBox.Close();
                    }
                    else if (!comboBox.IsContextMenuOpen && !comboBox.IsKeyboardFocusWithin)
                    {
                        // If textBox is clicked, claim focus
                        _ = comboBox.Focus();
                        e.Handled = true;   // Handle so that textbox won't try to update cursor position
                    }
                }
            }
        }

        /// <summary>
        ///     An event reporting the left mouse button was released.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Ignore the first mouse button up if we haven't gone over the popup yet
            // And ignore all mouse ups over the items host.
            if (HasMouseEnteredItemsHost && IsDropDownOpen && SelectionMode == SelectionMode.Single && !StaysOpenOnEdit)
            {
                Close();
                e.Handled = true;
            }

            base.OnMouseLeftButtonUp(e);
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            var comboBox = (MultiComboBox)sender;

            // The mouse moved, see if we're over the items host yet
            if (comboBox.IsDropDownOpen)
            {
                var isMouseOverItemsHost = comboBox.DropDownPopup != null && comboBox.DropDownPopup.IsMouseOver;

                comboBox.HasMouseEnteredItemsHost |= isMouseOverItemsHost;
            }
        }

        /// <summary>
        ///     Called when this element or any below gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When ComboBox gets logical focus, select the text inside us.
            var comboBox = (MultiComboBox)sender;

            // If we're an editable combobox, forward focus to the TextBox element
            if (!e.Handled && comboBox.IsEditable && comboBox.EditableTextBoxSite != null)
            {
                if (e.OriginalSource == comboBox)
                {
                    _ = comboBox.EditableTextBoxSite.Focus();
                    e.Handled = true;
                }
                else if (e.OriginalSource == comboBox.EditableTextBoxSite)
                {
                    comboBox.EditableTextBoxSite.SelectAll();
                }
            }
        }

        /// <summary>
        ///     An event reporting that the IsKeyboardFocusWithin property changed.
        /// </summary>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            // This is for the case when focus goes elsewhere and the popup is still open; make sure it is closed.
            if (IsDropDownOpen && !IsKeyboardFocusWithin && (Keyboard.FocusedElement is not DependencyObject currentFocus || (!IsContextMenuOpen && ItemsControlFromItemContainer(currentFocus) != this)))
            {
                Close();
            }
        }

        private static void OnContextMenuOpen(object sender, ContextMenuEventArgs e) => ((MultiComboBox)sender).IsContextMenuOpen = true;

        private static void OnContextMenuClose(object sender, ContextMenuEventArgs e) => ((MultiComboBox)sender).IsContextMenuOpen = false;

        private void Close()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }

        #endregion Capture

        #region Selection Changed

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            HasSelectedItems = SelectedItems != null && SelectedItems.Count > 0;

            base.OnSelectionChanged(e);

            if ((AutomationPeer.ListenerExists(AutomationEvents.SelectionPatternOnInvalidated)
                || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected)
                || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection)
                || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection)) && UIElementAutomationPeer.CreatePeerForElement(this) is MultiComboBoxAutomationPeer peer)
                peer.RaiseSelectionEvents(e);
        }

        private MultiComboBoxItem? GetItemContainer(object? item) => ItemContainerGenerator.ContainerFromItem(item) as MultiComboBoxItem;

        private void RemoveSelectedItem(object? item)
        {
            if (GetItemContainer(item) is MultiComboBoxItem container && container.IsSelected)
            {
                container.IsSelected = false;
            }
            else if (SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
            }
        }

        private bool AddSelectedItem(object? item)
        {
            if (GetItemContainer(item) is MultiComboBoxItem container && !container.IsSelected)
            {
                container.IsSelected = true;
                return true;
            }
            else if (!SelectedItems.Contains(item))
            {
                _ = SelectedItems.Add(item);
                return true;
            }

            return false;
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e) => UnselectAll();

        // When the Text Property changes, search for an item exactly
        // matching the new text and set the selected index to that item
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cb = (MultiComboBox)d;

            // Raise the propetyChangeEvent for Value if Automation Peer exist, the new Value must
            // be the one in SelctionBoxItem(selected value is the one user will care about)
            if (UIElementAutomationPeer.FromElement(cb) is MultiComboBoxAutomationPeer peer)
                peer.RaiseValuePropertyChangedEvent((string)e.OldValue, (string)e.NewValue);

            cb.TextUpdated((string)e.NewValue, false);

            cb.TextChanged?.Invoke(cb, EventArgs.Empty);
        }

        // When the user types in the TextBox, search for an item that partially
        // matches the new text and set the selected index to that item
        private void OnEditableTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsEditable)
            {
                // Don't do any work if we're not editable.
                return;
            }

            TextUpdated(EditableTextBoxSite?.Text, true);
        }

        // When selection changes, save the location of the selection start
        // (ignoring changes during compositions)
        private void OnEditableTextBoxSelectionChanged(object sender, RoutedEventArgs e) => _textBoxSelectionStart = EditableTextBoxSite?.SelectionStart ?? 0;

        // When the IME composition we're waiting for completes, run the text search logic
        private void OnEditableTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsWaitingForTextComposition)
            {
                IsWaitingForTextComposition = false;
                TextUpdated(EditableTextBoxSite?.Text, true);
            }
        }

        // If TextSearch is enabled search for an item matching the new text
        // (partial search if user is typing, exact search if setting Text)
        private void TextUpdated(string? newText, bool textBoxUpdated)
        {
            // Only process this event if it is coming from someone outside setting Text directly
            if (!UpdatingText)
            {
                try
                {
                    // Set the updating flags so we don't reenter this function
                    UpdatingText = true;

                    SetCurrentValue(PrefixProperty, newText);

                    // Try searching for an item matching the new text
                    if (IsTextSearchEnabled)
                    {
                        _currentSearchSelectedItem = FindMatchingPrefix(newText);

                        if (_currentSearchSelectedItem.MatchedItemIndex >= 0)
                        {
                            // Allow partial matches when updating textbox
                            if (textBoxUpdated)
                            {
                                var selectionStart = EditableTextBoxSite?.SelectionStart;
                                // Perform type search when the selection is at the end
                                // of the textbox and the selection start increased
                                if (selectionStart == newText?.Length &&
                                    selectionStart > _textBoxSelectionStart)
                                {
                                    // Replace the currently typed text with the text
                                    // from the matched item
                                    var matchedText = _currentSearchSelectedItem.MatchedText;

                                    if (ShouldPreserveUserEnteredPrefix)
                                    {
                                        // Retain the user entered prefix in the matched text.
                                        matchedText = string.Concat(newText, matchedText[_currentSearchSelectedItem.MatchedPrefixLength..]);
                                    }

                                    // when there's no IME, do it synchronously
                                    UpdateTextBox(matchedText, _currentSearchSelectedItem);

                                    // ComboBox's text property should be updated with the matched text
                                    newText = matchedText;
                                }
                            }
                            else //Text Property Set
                            {
                                // Require exact matches when setting TextProperty
                                var matchedText = _currentSearchSelectedItem.MatchedText;
                                if (!string.Equals(newText, matchedText, StringComparison.CurrentCulture))
                                {
                                    // Strings not identical, no match
                                    _currentSearchSelectedItem = MatchedTextInfo.NoMatch;
                                }
                            }
                        }
                    }

                    // Update TextProperty when TextBox changes and TextBox when TextProperty changes
                    if (textBoxUpdated)
                    {
                        SetCurrentValue(TextProperty, newText);
                    }
                    else if (EditableTextBoxSite != null)
                    {
                        EditableTextBoxSite.Text = newText;
                    }
                }
                finally
                {
                    // Clear the updating flag
                    UpdatingText = false;
                }
            }
        }

        private void UpdateTextBox(string matchedText, MatchedTextInfo matchedTextInfo)
        {
            if (EditableTextBoxSite == null) return;

            // Replace the TextBox's text with the matched text and
            // select the text beyond what the user typed
            EditableTextBoxSite.Text = matchedText;
            EditableTextBoxSite.SelectionStart = matchedText.Length - matchedTextInfo.TextExcludingPrefixLength;
            EditableTextBoxSite.SelectionLength = matchedTextInfo.TextExcludingPrefixLength;
        }

        /// <summary>
        ///     Helper function called by Editable ComboBox to search through items.
        /// </summary>
        internal MatchedTextInfo FindMatchingPrefix(string? prefix)
        {
            MatchedTextInfo matchedTextInfo;

            (var matchedItemIndex, var matchedText, var matchedItem) = FindMatchingItem(prefix);

            if (matchedItemIndex >= 0)
            {
                GetMatchingPrefixAndRemainingTextLength(matchedText, prefix, GetCulture(), !IsTextSearchCaseSensitive, out var matchedPrefixLength, out var textExcludingPrefixLength);
                matchedTextInfo = new MatchedTextInfo(matchedItemIndex, matchedText, matchedItem, matchedPrefixLength, textExcludingPrefixLength);
            }
            else
            {
                matchedTextInfo = MatchedTextInfo.NoMatch;
            }

            return matchedTextInfo;
        }

        private (int index, string text, object? matchedItem) FindMatchingItem(string? prefix)
        {
            var count = Items.Count;

            if (count == 0 || string.IsNullOrEmpty(prefix))
            {
                return (-1, string.Empty, null);
            }

            for (var currentIndex = 0; currentIndex < count; currentIndex++)
            {
                var item = Items[currentIndex];

                if (item != null)
                {
                    var itemString = GetItemText(item);

                    if (itemString != null && itemString.StartsWith(prefix, !IsTextSearchCaseSensitive, GetCulture()) && (SelectedItem == null || !SelectedItems.Contains(item)))
                    {
                        return (currentIndex, itemString, item);
                    }
                }
            }

            return (-1, string.Empty, null);
        }

        private string? GetItemText(object item)
        {
            var path = !string.IsNullOrEmpty(TextSearch.GetTextPath(this)) ? TextSearch.GetTextPath(this) : DisplayMemberPath;

            return !string.IsNullOrEmpty(path) ? item.GetDeepPropertyValue(path)?.ToString() : item.ToString();
        }

        /// <summary>
        /// Gets the length of the prefix (the prefix of matchedText matched by newText) and the rest of the string from the matchedText
        /// It takes care of compressions or expansions in both matchedText and newText  which could be impacting the length of the string
        /// For example: length of prefix would be 5 and the rest would be 2 if matchedText is "Grosses" and newText is ""Groß"
        /// length of prefix would be 4 and the rest would be 2 if matchedText is ""Großes" and newText is "Gross" as "ß" = "ss"
        /// </summary>
        /// /// <param name="matchedText">string that is assumed to contain prefix which matches newText</param>
        /// <param name="newText">string that is assumed to match a prefix of matchedText</param>
        /// <param name="cultureInfo"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="matchedPrefixLength"></param>
        /// <param name="textExcludingPrefixLength"></param>
        private static void GetMatchingPrefixAndRemainingTextLength(string? matchedText, string? newText, CultureInfo? cultureInfo,
                                                                bool ignoreCase, out int matchedPrefixLength, out int textExcludingPrefixLength)
        {
            matchedPrefixLength = 0;
            textExcludingPrefixLength = 0;

            if (matchedText == null || newText == null || matchedText.Length < newText.Length)
            {
                matchedPrefixLength = matchedText?.Length ?? 0;
                textExcludingPrefixLength = 0;
            }
            else
            {
                // mostly compression or expansion is not involved. So start with length of newText
                var i = newText.Length;
                var j = i + 1;

                do
                {
                    string temp;

                    if (i >= 1)
                    {
                        temp = matchedText[..i];
                        if (string.Compare(newText, temp, ignoreCase, cultureInfo) == 0)
                        {
                            matchedPrefixLength = i;
                            textExcludingPrefixLength = matchedText.Length - i;
                            break;
                        }
                    }
                    if (j <= matchedText.Length)
                    {
                        temp = matchedText[..j];
                        if (string.Compare(newText, temp, ignoreCase, cultureInfo) == 0)
                        {
                            matchedPrefixLength = j;
                            textExcludingPrefixLength = matchedText.Length - j;
                            break;
                        }
                    }

                    i--;
                    j++;
                } while (i >= 1 || j <= matchedText.Length);
            }
        }

        private CultureInfo? GetCulture()
        {
            var o = GetValue(LanguageProperty);
            CultureInfo? culture = null;

            if (o != null)
            {
                var language = (XmlLanguage)o;
                try
                {
                    culture = language.GetSpecificCulture();
                }
                catch (InvalidOperationException)
                {
                    // No action
                }
            }

            return culture;
        }

        #endregion Selection Changed

        #region Key input

        private void OnEditableTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditable)
                return;

            var key = e.Key;

            switch (key)
            {
                case Key.Escape:
                    if (IsDropDownOpen)
                    {
                        Close();
                    }
                    break;

                case Key.Tab:
                case Key.Enter:
                    e.Handled = true;
                    CommitSelection();
                    break;
            }
        }

        private void CommitSelection()
        {
            if (IsEditable && _currentSearchSelectedItem != null && EditableTextBoxSite != null && AddSelectedItem(_currentSearchSelectedItem.MatchedItem))
            {
                EditableTextBoxSite.Clear();
                _ = EditableTextBoxSite.Focus();
            }
        }

        #endregion Key input

        #region Item container

        protected override bool IsItemItsOwnContainerOverride(object item) => item is MultiComboBoxItem;

        protected override DependencyObject GetContainerForItemOverride() => new MultiComboBoxItem();

        #endregion Item container

        #region Accessibility

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer() => new MultiComboBoxAutomationPeer(this);

        #endregion Accessibility

        internal class MatchedTextInfo
        {
            internal MatchedTextInfo(int matchedItemIndex, string matchedText, object? matchdItem, int matchedPrefixLength, int textExcludingPrefixLength)
            {
                MatchedItemIndex = matchedItemIndex;
                MatchedItem = matchdItem;
                MatchedText = matchedText;
                MatchedPrefixLength = matchedPrefixLength;
                TextExcludingPrefixLength = textExcludingPrefixLength;
            }

            #region Internal Properties

            /// <summary>
            /// No match from text search
            /// </summary>
            internal static MatchedTextInfo NoMatch { get; } = new MatchedTextInfo(-1, string.Empty, null, 0, 0);

            /// <summary>
            /// Matched text from text search
            /// </summary>
            internal string MatchedText { get; }

            /// <summary>
            /// Matched text from text search
            /// </summary>
            internal object? MatchedItem { get; }

            /// <summary>
            /// Index of the matched item
            /// </summary>
            internal int MatchedItemIndex { get; }

            /// <summary>
            /// Length of the matched prefix
            /// </summary>
            internal int MatchedPrefixLength { get; }

            /// <summary>
            /// Length of the text excluding prefix
            /// </summary>
            internal int TextExcludingPrefixLength { get; }

            #endregion Internal Properties
        }
    }
}
