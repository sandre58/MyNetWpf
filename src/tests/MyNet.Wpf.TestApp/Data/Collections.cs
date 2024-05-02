// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MyNet.Humanizer;
using MyNet.Observable.Translatables;
using MyNet.UI.Selection.Models;
using MyNet.Utilities;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Geography.Extensions;
using MyNet.Utilities.Helpers;

namespace MyNet.Wpf.TestApp.Data
{
    public class SelectedCountry : SelectedWrapper<Country>
    {
        public string? DisplayName { get; set; }

        public int Iso { get; set; }

        public int Order { get; set; }

        public DateTime CreatedDate { get; set; }

        public SelectedCountry(Country item, int order) : base(item)
        {
            DisplayName = item.GetDisplayName();
            Iso = item.Iso;
            CreatedDate = DateTime.Now;
            Order = order;
        }
    }

    public class CountriesWrapper : DisplayWrapper<IEnumerable<Country>>
    {
        public CountriesWrapper(IEnumerable<Country> item, string key) : base(item, key)
        {
        }
    }

    public static class Collections
    {
        public static readonly ImmutableList<int> Integers = EnumerableHelper.Range(1, 100, 1).ToImmutableList();

        public static readonly ImmutableList<string> Countries = [.. Enumeration.GetAll<Country>().Select(x => x.GetDisplayName()).NotNull().OrderBy(x => x)];

        public static readonly ImmutableList<CountriesWrapper> CountriesByAplha =
        [
            .. Enumeration.GetAll<Country>().GroupBy(x => x.Humanize()![..1])
                                                                                                                           .Select(x => new CountriesWrapper(x.OrderBy(x => x.GetDisplayName()), x.Key))
                                                                                                                           .OrderBy(x => x.DisplayName.Value)
,
        ];

        public static readonly ImmutableList<SelectedCountry> CountriesDetails = [.. Enumeration.GetAll<Country>().Select((x, index) => new SelectedCountry(x, index)).OrderBy(x => x.Item?.GetDisplayName())];
    }
}
