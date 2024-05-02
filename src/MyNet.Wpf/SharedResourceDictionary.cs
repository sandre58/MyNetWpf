// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf
{
    /// <summary>
    /// The shared resource dictionary is a specialized resource dictionary
    /// that loads it content only once. If a second instance with the same source
    /// is created, it only merges the resources from the cache.
    /// </summary>
    public class SharedResourceDictionary : ResourceDictionary
    {
        /// <summary>
        /// Internal cache of loaded dictionaries 
        /// </summary>
        private static readonly Dictionary<Uri, ResourceDictionary> SharedDictionaries = [];

        /// <summary>
        /// Local member of the source uri
        /// </summary>
        private Uri? _sourceUri;

        /// <summary>
        /// Indicates if source is only used in design mode
        /// </summary>
        public bool DesignMode { get; set; }

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        public new Uri? Source
        {
            get => DesignerHelper.IsInDesignMode ? base.Source : _sourceUri;
            set
            {
                _sourceUri = value;

                if (value == null) return;
                if (DesignerHelper.IsInDesignMode)
                {
                    base.Source = value;
                    return;
                }

                if (DesignMode) return;

                if (!SharedDictionaries.TryGetValue(value, out var val))
                {
                    // If the dictionary is not yet loaded, load it by setting
                    // the source of the base class
                    base.Source = value;

                    // add it to the cache
                    if (!SharedDictionaries.ContainsKey(value))
                        SharedDictionaries.Add(value, this);
                }
                else
                    // If the dictionary is already loaded, get it from the cache
                    MergedDictionaries.Add(val);
            }
        }
    }
}
