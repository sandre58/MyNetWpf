// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;

namespace MyNet.Wpf.Controls
{
    public class TransitioningContent : MaterialDesignThemes.Wpf.Transitions.TransitioningContent
    {
        static TransitioningContent() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningContent), new FrameworkPropertyMetadata(typeof(TransitioningContent)));

        #region TriggerObject

        /// <summary>Identifies the <see cref="TriggerObject"/> dependency property.</summary>
        public static readonly DependencyProperty TriggerObjectProperty
            = DependencyProperty.Register(nameof(TriggerObject),
                                          typeof(object),
                                          typeof(TransitioningContent),
                                          new FrameworkPropertyMetadata(OnTriggerObjectPropertyChanged));

        /// <summary>
        /// Gets or sets the name of the <see cref="SelectedImage"/>.
        /// </summary>
        public object? TriggerObject
        {
            get => GetValue(TriggerObjectProperty);
            set => SetValue(TriggerObjectProperty, value);
        }

        private static void OnTriggerObjectPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is TransitioningContent transitioningContent && e.OldValue != e.NewValue)
            {
                transitioningContent.RefreshDataTemplateSelector();
                transitioningContent.StartOpeningEffects();
            }
        }

        #endregion

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (oldContent != newContent)
                StartOpeningEffects();
        }

        public void StartOpeningEffects()
        {
            if (OpeningEffect is null && (OpeningEffects is null || !OpeningEffects.Any())) return;

            RunOpeningEffects();
        }

        private void RefreshDataTemplateSelector()
        {
            var oldSelector = ContentTemplateSelector;
            if (oldSelector != null)
            {
                ContentTemplateSelector = null;
                ContentTemplateSelector = oldSelector;
            }
        }
    }
}
