// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Helpers
{
    public static class WpfHelper
    {
        public static bool IsAnimationsEnabled => SystemParameters.ClientAreaAnimation && RenderCapability.Tier > 0;

        public static object GetResource(object key) => GetResource(Application.Current.Resources, key);

        public static T GetResource<T>(object key)
        {
            var resource = GetResource(key);

            return (T)resource;
        }

        public static object GetResource(ResourceDictionary dictionary, object key)
        {
            if (dictionary.Contains(key)) return dictionary[key];

            foreach (var item in dictionary.MergedDictionaries)
            {
                if (GetResource(item, key) is object value)
                {
                    return value;
                }
            }

            return key;
        }

        public static T? GetResource<T>(ResourceDictionary dictionary, object key)
        {
            var resource = GetResource(dictionary, key);

            return resource == null ? default : (T)resource;
        }

        public static T? GetResourceOrDefault<T>(object key)
        {
            var resource = GetResource(key);

            return resource is T t ? t : default;
        }

        public static void SyncProperty(DependencyObject obj1, DependencyObject obj2, DependencyProperty contentProperty, DependencyProperty columnProperty)
        {
            if (IsDefaultValue(obj1, columnProperty))
                obj2.ClearValue(contentProperty);
            else
                obj2.SetValue(contentProperty, obj1.GetValue(columnProperty));
        }

        private static bool IsDefaultValue(DependencyObject d, DependencyProperty dp)
            => DependencyPropertyHelper.GetValueSource(d, dp).BaseValueSource == BaseValueSource.Default;
    }

}
