// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Filtering.Filters;
using MyNet.UI.ViewModels.List.Sorting;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class IconsListViewModel : SelectionListViewModel<IconData>
    {
        public IconsListViewModel(ICollection<IconData> collection) : base(collection, new IconsControllerProvider(), selectionMode: UI.Selection.SelectionMode.Single) { }
    }

    internal class IconsControllerProvider : ListParametersProvider
    {
        public override IFiltersViewModel ProvideFilters() => new StringFilterViewModel(nameof(IconData.Aliases));

        public override ISortingViewModel ProvideSorting() => new SortingViewModel(nameof(IconData.Name));
    }

    internal class IconData
    {
        private readonly Func<object> _buildIcon;

        public IconData(Func<object> buildIcon, string name, string codeBlock, string[] aliases, string? data)
        {
            _buildIcon = buildIcon;
            Name = name;
            CodeBlock = codeBlock;
            Aliases = aliases;
            Data = data;
        }

        public object Icon => _buildIcon();

        public string Name { get; }

        public string CodeBlock { get; }

        public string[] Aliases { get; }

        public string? Data { get; }
    }
}
