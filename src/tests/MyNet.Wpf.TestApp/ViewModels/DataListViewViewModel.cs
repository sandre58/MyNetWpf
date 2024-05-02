// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using MyNet.Observable;
using MyNet.UI.Dialogs;
using MyNet.UI.Locators;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels;
using MyNet.UI.ViewModels.Display;
using MyNet.UI.ViewModels.Edition;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Filtering.Filters;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Geography.Extensions;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class DataListViewViewModel : NavigableWorkspaceViewModel
    {
        public CountrieListViewModel CountrieListViewModel { get; set; } = new();
    }

    internal class CountrieListViewModel : SelectionListViewModel<CountryViewModel>, IDropTarget
    {
        public CountrieListViewModel()
            : base(Enumeration.GetAll<Country>().Select(x => new CountryViewModel(x)).ToList(),
                  parametersProvider: new CountriesParametersProvider())
        { }

        protected override void EditItemCore(CountryViewModel oldItem, CountryViewModel newItem) => oldItem.SetFrom(newItem);

        protected override async Task<CountryViewModel?> CreateNewItemAsync()
        {
            var vm = ViewModelManager.Get<CountryEditViewModel>();
            vm.SetOriginalItem(null);
            _ = await DialogManager.ShowDialogAsync(vm).ConfigureAwait(false);

            return vm.DialogResult.IsTrue() ? vm.Item : null;
        }

        protected override async Task<CountryViewModel?> UpdateItemAsync(CountryViewModel oldItem)
        {
            var vm = ViewModelManager.Get<CountryEditViewModel>();
            vm.SetOriginalItem(oldItem);
            _ = await DialogManager.ShowDialogAsync(vm).ConfigureAwait(false);

            return vm.DialogResult.IsTrue() ? vm.Item : null;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IWrapper<CountryViewModel> wrapper)
                ToasterManager.ShowInformation($"{wrapper.Item} moved to index {dropInfo.InsertIndex}");
            else if (dropInfo.Data is ICollection list)
                ToasterManager.ShowInformation($"{list.Count} items moved to index {dropInfo.InsertIndex}");
        }
    }

    internal class CountriesParametersProvider : ListParametersProvider
    {
        public override IFiltersViewModel ProvideFilters() => new StringFilterViewModel(nameof(CountryViewModel.Name));

        public override ISortingViewModel ProvideSorting() => new ExtendedSortingViewModel([nameof(CountryViewModel.Name), nameof(CountryViewModel.Alpha2), nameof(CountryViewModel.Alpha3)], [nameof(CountryViewModel.Name)]);

        public override IDisplayViewModel ProvideDisplay() => new DisplayViewModel([], new DisplayModeList());
    }

    internal class CountryEditViewModel : ItemEditionViewModel<CountryViewModel>
    {
        public CountryEditViewModel() { }

        protected override CountryViewModel CreateNewItem() => new("New Country");

        protected override CountryViewModel? SaveItem(CountryViewModel? item) => item;
    }

    internal class CountryViewModel : EditableObject, ICloneable, ISettable, IIdentifiable<Guid>
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Country? Country { get; }

        public string? Alpha2 { get; }

        public string? Alpha3 { get; }

        public int? Iso { get; }

        public string Name { get; set; }

        public CountryViewModel(Country country)
        {
            Name = country.GetDisplayName();
            Country = country;
            Alpha2 = country.Alpha2;
            Alpha3 = country.Alpha3;
            Iso = country.Iso;
        }

        public CountryViewModel(string name)
        {
            Name = name;
            Alpha2 = name[..2];
            Alpha3 = name[..3];
        }

        public object Clone() => Country is not null ? new CountryViewModel(Country!) { Name = Name } : new CountryViewModel(Name) { Name = Name };

        public override string? ToString() => Name;

        public override bool Equals(object? obj) => obj is CountryViewModel c && Name == c.Name;

        public void SetFrom(object? from)
        {
            if (from is CountryViewModel country)
            {
                Name = country.Name;
            }
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}
