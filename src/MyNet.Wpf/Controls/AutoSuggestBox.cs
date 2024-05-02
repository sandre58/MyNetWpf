// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using MyNet.Wpf.Extensions;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
[TemplatePart(Name = PartPopup, Type = typeof(Popup))]
[TemplatePart(Name = PartSelector, Type = typeof(Selector))]
public class AutoSuggestBox : TextBox
{
    public const string PartPopup = "PART_Popup";
    public const string PartSelector = "PART_Selector";

    private bool _isUpdatingText;
    private bool _selectionCancelled;
    private Selector? _itemsSelector;
    private DispatcherTimer? _fetchTimer;
    private SelectionAdapter? _selectionAdapter;
    private readonly ObservableCollection<object> _suggestions = [];

    public event EventHandler? CommitSelectedItem;

    static AutoSuggestBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(typeof(AutoSuggestBox)));
        FocusableProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(true));
        IsEnabledProperty.OverrideMetadata(typeof(AutoSuggestBox), new FrameworkPropertyMetadata(true, OnIsEnabledChanged));
    }

    public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(false));
    public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(string.Empty));
    public static readonly DependencyProperty GroupMemberPathProperty = DependencyProperty.Register("GroupMemberPath", typeof(string), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(string.Empty));
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(AutoSuggestBox));
    public static readonly DependencyProperty LoadingContentProperty = DependencyProperty.Register("LoadingContent", typeof(object), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty PreviewSelectionProperty = DependencyProperty.Register("PreviewSelection", typeof(bool), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(true));
    public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register("Provider", typeof(ISuggestionProvider), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(null));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(null, OnSelectedItemChanged));
    public static readonly DependencyProperty MaxPopUpHeightProperty = DependencyProperty.Register("MaxPopupHeight", typeof(double), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(600d));
    public static readonly DependencyProperty PopUpWidthProperty = DependencyProperty.Register("PopupWidth", typeof(double), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(600d));
    public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(int), typeof(AutoSuggestBox), new FrameworkPropertyMetadata(200));
    public static readonly DependencyProperty PopupAnimationProperty = DependencyProperty.Register(nameof(PopupAnimation), typeof(PopupAnimation), typeof(AutoSuggestBox), new UIPropertyMetadata(PopupAnimation.Slide));

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ObservableCollection<GroupStyle> GroupStyle { get; } = [];

    public Style ItemContainerStyle
    {
        get => (Style)GetValue(ItemContainerStyleProperty);
        set => SetValue(ItemContainerStyleProperty, value);
    }

    public PopupAnimation PopupAnimation
    {
        get => (PopupAnimation)GetValue(PopupAnimationProperty);
        set => SetValue(PopupAnimationProperty, value);
    }

    public double MaxPopupHeight
    {
        get => (double)GetValue(MaxPopUpHeightProperty);
        set => SetValue(MaxPopUpHeightProperty, value);
    }
    public double PopupWidth
    {
        get => (double)GetValue(PopUpWidthProperty);
        set => SetValue(PopUpWidthProperty, value);
    }

    public int Delay
    {
        get => (int)GetValue(DelayProperty);

        set => SetValue(DelayProperty, value);
    }

    public string DisplayMemberPath
    {
        get => (string)GetValue(DisplayMemberPathProperty);

        set => SetValue(DisplayMemberPathProperty, value);
    }

    public string GroupMemberPath
    {
        get => (string)GetValue(GroupMemberPathProperty);

        set => SetValue(GroupMemberPathProperty, value);
    }

    public bool IsDropDownOpen
    {
        get => (bool)GetValue(IsDropDownOpenProperty);

        set => SetValue(IsDropDownOpenProperty, value);
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);

        set => SetValue(IsLoadingProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);

        set => SetValue(ItemTemplateProperty, value);
    }

    public DataTemplateSelector ItemTemplateSelector
    {
        get => ((DataTemplateSelector)(GetValue(ItemTemplateSelectorProperty)));
        set => SetValue(ItemTemplateSelectorProperty, value);
    }

    public object LoadingContent
    {
        get => GetValue(LoadingContentProperty);

        set => SetValue(LoadingContentProperty, value);
    }

    public bool PreviewSelection
    {
        get => (bool)GetValue(PreviewSelectionProperty);
        set => SetValue(PreviewSelectionProperty, value);
    }

    public ISuggestionProvider Provider
    {
        get => (ISuggestionProvider)GetValue(ProviderProperty);

        set => SetValue(ProviderProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);

        set => SetValue(SelectedItemProperty, value);
    }


    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AutoSuggestBox autoSuggestBox && !autoSuggestBox.IsEnabled)
        {
            autoSuggestBox.IsDropDownOpen = false;
        }
    }

    public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AutoSuggestBox autoSuggestBox && !autoSuggestBox._isUpdatingText)
        {
            autoSuggestBox._isUpdatingText = true;
            autoSuggestBox.Text = autoSuggestBox.GetDisplayText(e.NewValue);
            autoSuggestBox._isUpdatingText = false;
        }
    }

    private void ScrollToSelectedItem()
    {
        if (_itemsSelector is ListBox listBox && listBox.SelectedItem != null)
            listBox.ScrollIntoView(listBox.SelectedItem);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _itemsSelector = Template.FindName(PartSelector, this) as Selector;

        if (Template.FindName(PartPopup, this) is Popup popup)
        {
            popup.StaysOpen = false;
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;
        }
        if (_itemsSelector != null)
        {
            var source = new CollectionViewSource() { Source = _suggestions };
            if (!string.IsNullOrEmpty(GroupMemberPath))
            {
                source.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = GroupMemberPath });
            }
            _itemsSelector.GroupStyle.Set(GroupStyle);
            _itemsSelector.ItemsSource = source.View;
            _selectionAdapter = new SelectionAdapter(_itemsSelector);
            _selectionAdapter.Commit += OnSelectionAdapterCommit;
            _selectionAdapter.Cancel += OnSelectionAdapterCancel;
            _selectionAdapter.SelectionChanged += OnSelectionAdapterSelectionChanged;
            _itemsSelector.PreviewMouseDown += ItemsSelector_PreviewMouseDown;
        }
    }

    private void ItemsSelector_PreviewMouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject d && (d is Button || d.FindVisualParent<Button>() is not null)) return;
        if (e.OriginalSource is DependencyObject d1 && (d1 is ScrollBar || d1.FindVisualParent<ScrollBar>() is not null)) return;

        if (_itemsSelector is null || (e.OriginalSource as FrameworkElement)?.DataContext == null)
            return;
        if (!_itemsSelector.Items.Contains(((FrameworkElement?)e.OriginalSource)?.DataContext))
            return;
        _itemsSelector.SelectedItem = ((FrameworkElement?)e.OriginalSource)?.DataContext;
        OnSelectionAdapterCommit(SelectionAdapter.EventCause.MouseUp);
        e.Handled = true;
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
        base.OnTextChanged(e);

        if (_isUpdatingText)
            return;

        if (_fetchTimer == null)
        {
            _fetchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Delay) };
            _fetchTimer.Tick += OnFetchTimerTick;
        }

        _fetchTimer.IsEnabled = false;
        _fetchTimer.Stop();
        SetSelectedItem(null);
        if (Text.Length > 0)
        {
            _fetchTimer.IsEnabled = true;
            _fetchTimer.Start();
        }
        else
        {
            _suggestions.Clear();
            IsDropDownOpen = false;
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        if (_selectionAdapter != null)
        {
            if (IsDropDownOpen)
                _selectionAdapter.HandleKeyDown(e);
            else
                IsDropDownOpen = e.Key == Key.Down || e.Key == Key.Up;
        }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        if (!IsKeyboardFocusWithin)
            IsDropDownOpen = false;
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        base.OnGotKeyboardFocus(e);

        if (e.OldFocus == this)
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
    }

    private string? GetDisplayText(object? dataItem) => dataItem?.GetDeepPropertyValue(DisplayMemberPath)?.ToString();

    private void OnFetchTimerTick(object? sender, EventArgs e)
    {
        if (_fetchTimer is null) return;

        _fetchTimer.IsEnabled = false;
        _fetchTimer.Stop();
        if (Provider != null && _itemsSelector != null)
            SearchSuggestions(Text);
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        if (!_selectionCancelled)
        {
            OnSelectionAdapterCommit(SelectionAdapter.EventCause.PopupClosed);
        }
    }

    private void OnPopupOpened(object? sender, EventArgs e)
    {
        if (_itemsSelector is null) return;
        _selectionCancelled = false;
        _itemsSelector.SelectedItem = SelectedItem;
    }

    private void OnSelectionAdapterCancel(SelectionAdapter.EventCause cause)
    {
        if (PreSelectionEventSomeoneHandled(cause, SelectionAdapter.EventAction.Rollback))
            return;

        IsDropDownOpen = false;
        _selectionCancelled = true;

        if (!PreviewSelection)
            return;

        _isUpdatingText = true;
        Text = SelectedItem == null ? Text : GetDisplayText(SelectedItem).OrEmpty();
        SelectionStart = Text.Length;
        SelectionLength = 0;
        _isUpdatingText = false;
    }

    public event EventHandler<SelectionAdapter.PreSelectionAdapterFinishArgs>? PreSelectionAdapterFinish;

    private bool PreSelectionEventSomeoneHandled(SelectionAdapter.EventCause cause, SelectionAdapter.EventAction action)
    {
        var args = new SelectionAdapter.PreSelectionAdapterFinishArgs(action, cause);
        PreSelectionAdapterFinish?.Invoke(this, args);
        return args.Handled;

    }

    private void OnSelectionAdapterCommit(SelectionAdapter.EventCause cause)
    {
        if (_itemsSelector is null || PreSelectionEventSomeoneHandled(cause, SelectionAdapter.EventAction.Commit))
            return;

        if (_itemsSelector.SelectedItem != null)
        {
            SelectedItem = _itemsSelector.SelectedItem;
            _isUpdatingText = true;
            Text = GetDisplayText(_itemsSelector.SelectedItem).OrEmpty();
            SelectionStart = Text.Length;
            SelectionLength = 0;
            SetSelectedItem(_itemsSelector.SelectedItem);
            _isUpdatingText = false;
            IsDropDownOpen = false;
        }
    }

    private void OnSelectionAdapterSelectionChanged()
    {
        ScrollToSelectedItem();

        if (_itemsSelector is null || !PreviewSelection)
            return;

        _isUpdatingText = true;
        Text = _itemsSelector.SelectedItem == null ? Text : GetDisplayText(_itemsSelector.SelectedItem).OrEmpty();
        SelectionStart = Text.Length;
        SelectionLength = 0;
        _isUpdatingText = false;
    }

    private void SetSelectedItem(object? item)
    {
        _isUpdatingText = true;
        SelectedItem = item;
        CommitSelectedItem?.Invoke(this, EventArgs.Empty);
        _isUpdatingText = false;
    }

    public void SearchSuggestions(string searchText)
    {
        if (_itemsSelector is null) return;

        IsLoading = true;
        _suggestions.Clear();
        // Do not open drop down if control is not focused
        if (IsKeyboardFocusWithin)
            IsDropDownOpen = true;
        ParameterizedThreadStart thInfo = SearchSuggestionsAsync;
        var th = new Thread(thInfo);
        th.Start(new object[] { searchText, Provider });
    }

    private void DisplaySuggestions(IEnumerable suggestions, string filter)
    {
        if (_itemsSelector is null) return;

        _suggestions.AddRange(suggestions.OfType<object>());
        IsLoading = false;

        // Close drop down if there are no items
        if (IsDropDownOpen)
            IsDropDownOpen = _itemsSelector.HasItems;
    }

    private void SearchSuggestionsAsync(object? param)
    {
        if (param is object[] args)
        {
            var searchText = Convert.ToString(args[0]);
            if (args[1] is ISuggestionProvider provider)
            {
                var list = provider.GetSuggestions(searchText);
                Dispatcher.BeginInvoke(new Action<IEnumerable, string>(DisplaySuggestions), DispatcherPriority.Background, list, searchText);
            }
        }
    }
}

public class SuggestionProvider : ISuggestionProvider
{
    private readonly Func<string?, IEnumerable> _method;

    public SuggestionProvider(Func<string?, IEnumerable> method) => _method = method ?? throw new ArgumentNullException(nameof(method));

    public IEnumerable GetSuggestions(string? filter) => _method(filter);
}

public interface ISuggestionProvider
{
    IEnumerable GetSuggestions(string? filter);
}

public class SelectionAdapter
{
    public class PreSelectionAdapterFinishArgs
    {
        public PreSelectionAdapterFinishArgs(EventAction action, EventCause cause)
        {
            Action = action;
            Cause = cause;

            var commitAllow = action == EventAction.Commit && cause is EventCause.EnterPressed or EventCause.ItemClicked or EventCause.MouseUp;
            var rollbackAllow = action == EventAction.Rollback && cause is EventCause.EscapePressed or EventCause.TabPressed or EventCause.PopupClosed;
            Handled = !(commitAllow || rollbackAllow);
        }

        public EventCause Cause { get; }

        public EventAction Action { get; }

        public bool Handled { get; set; }
    }

    public SelectionAdapter(Selector selector)
    {
        SelectorControl = selector;
        SelectorControl.PreviewMouseUp += OnSelectorMouseUp;
    }

    public enum EventCause { Other, PopupClosed, ItemClicked, EnterPressed, EscapePressed, TabPressed, MouseUp }

    public enum EventAction { Commit, Rollback }

    public delegate void CancelEventHandler(EventCause cause);

    public delegate void CommitEventHandler(EventCause cause);

    public delegate void SelectionChangedEventHandler();

    public event CancelEventHandler? Cancel;
    public event CommitEventHandler? Commit;
    public event SelectionChangedEventHandler? SelectionChanged;

    public Selector SelectorControl { get; set; }

    public void HandleKeyDown(KeyEventArgs key)
    {
        switch (key.Key)
        {
            case Key.Down:
                IncrementSelection();
                break;
            case Key.Up:
                DecrementSelection();
                break;
            case Key.Enter:
                Commit?.Invoke(EventCause.EnterPressed);

                break;
            case Key.Escape:
                Cancel?.Invoke(EventCause.EscapePressed);

                break;
            case Key.Tab:
                Cancel?.Invoke(EventCause.EscapePressed);
                Commit?.Invoke(EventCause.TabPressed);
                return;
            default:
                return;
        }
        key.Handled = true;
    }

    private void DecrementSelection()
    {
        if (SelectorControl.SelectedIndex > 0)
            SelectorControl.SelectedIndex -= 1;

        SelectionChanged?.Invoke();
    }

    private void IncrementSelection()
    {
        if (SelectorControl.SelectedIndex < SelectorControl.Items.Count - 1)
            SelectorControl.SelectedIndex += 1;

        SelectionChanged?.Invoke();
    }

    private void OnSelectorMouseUp(object sender, MouseButtonEventArgs e)
    {
        // If sender is the RepeatButton from the scrollbar we need to 
        // to skip this event otherwise focus get stuck in the RepeatButton
        // and list is scrolled up or down til the end.
        if (e.OriginalSource is DependencyObject d && (d is ButtonBase || d.FindVisualParent<ButtonBase>() is not null)) return;
        if (e.OriginalSource is DependencyObject d1 && (d1 is ScrollBar || d1.FindVisualParent<ScrollBar>() is not null)) return;

        Commit?.Invoke(EventCause.MouseUp);
        e.Handled = true;
    }
}
