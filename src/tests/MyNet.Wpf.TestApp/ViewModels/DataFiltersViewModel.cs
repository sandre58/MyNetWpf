// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Observable;
using MyNet.UI.ViewModels;
using MyNet.UI.ViewModels.Display;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Filtering.Filters;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.UI.ViewModels.List.Sorting;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;
using MyNet.Utilities.Comparaison;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Generator.Extensions;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Sequences;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class DataFiltersViewModel : NavigableWorkspaceViewModel
    {
        public DataFiltersViewModel()
            => ItemsViewModel = new(EnumerableHelper.Range(0, RandomGenerator.Int(20, 200), 1).Select(_ => CreateRandomPerson()).ToList(),
                  parametersProvider: new PersonsControllerProvider());

        public SelectionListViewModel<PersonViewModel> ItemsViewModel { get; }

        private static PersonViewModel CreateRandomPerson()
        {
            var year = RandomGenerator.Int(1970, 2013);
            var gender = RandomGenerator.Enum<GenderType>();
            var birthdate = new DateTime(year, RandomGenerator.Int(1, 12), RandomGenerator.Int(1, 27), 0, 0, 0, DateTimeKind.Utc);

            var item = new PersonViewModel()
            {
                LastName = NameGenerator.LastName().ToSentence(),
                FirstName = NameGenerator.FirstName(gender == GenderType.Female ? GenderType.Female : GenderType.Male).ToSentence(),
                Country = RandomGenerator.ListItem(Enumeration.GetAll<Country>().ToList()),
                Birthdate = birthdate,
                FromDate = new DateTime(RandomGenerator.Int(year + 6, DateTime.Today.Year - 1), 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Gender = gender,
                Laterality = RandomGenerator.Enum<Laterality>(),
                PlaceOfBirth = AddressGenerator.City().ToSentence(),
                Size = RandomGenerator.ArrayElement("S;M;L;XL;XXL".Split(';')),
                LicenseNumber = string.Join("", RandomGenerator.Digits(10)),
                Description = string.Join(Environment.NewLine, SentenceGenerator.Paragraphs(10, 50, 1, 4, 1, 2)),
                IsMutation = RandomGenerator.Bool(),
                LicenseState = RandomGenerator.Enum<LicenseState>(),
                Height = RandomGenerator.Number(150, 210),
                Weight = RandomGenerator.Number(50, 110),
                ShoesSize = RandomGenerator.Number(35, 46),
                Number = RandomGenerator.Number(1, 99)
            };

            return item;
        }
    }

    internal class PersonsControllerProvider : ListParametersProvider
    {
        public override IFiltersViewModel ProvideFilters() => new ExtendedFiltersViewModel(new Dictionary<string, Func<IFilterViewModel>>
            {
                { nameof(PersonViewModel.InverseName), () => new StringFilterViewModel(nameof(PersonViewModel.InverseName)) },
                { nameof(PersonViewModel.Age), () => new IntegerFilterViewModel(nameof(PersonViewModel.Age), ComplexComparableOperator.IsBetween, PersonViewModel.AcceptableRangeAge) },
                { nameof(PersonViewModel.Number), () => new IntegerFilterViewModel(nameof(PersonViewModel.Number), ComplexComparableOperator.IsBetween, PersonViewModel.AcceptableRangeNumber) },
                { nameof(PersonViewModel.LicenseState), () => new EnumValuesFilterViewModel(nameof(PersonViewModel.LicenseState), typeof(LicenseState)) },
                { nameof(PersonViewModel.IsMutation), () => new BooleanFilterViewModel(nameof(PersonViewModel.IsMutation)) },
                { nameof(PersonViewModel.Gender), () => new EnumValueFilterViewModel<GenderType>(nameof(PersonViewModel.Gender)) },
                { nameof(PersonViewModel.FromDate), () => new DateFilterViewModel(nameof(PersonViewModel.FromDate), ComplexComparableOperator.LessThan, DateTime.Now, DateTime.Now) },
                { nameof(PersonViewModel.Country), () => new CountriesFilterViewModel(nameof(PersonViewModel.Country)) },
                { nameof(PersonViewModel.Laterality), () => new EnumValuesFilterViewModel(nameof(PersonViewModel.Laterality), typeof(Laterality)) },
                { nameof(PersonViewModel.Height), () => new IntegerFilterViewModel(nameof(PersonViewModel.Height), ComplexComparableOperator.IsBetween, PersonViewModel.AcceptableRangeHeight) },
                { nameof(PersonViewModel.Weight), () => new IntegerFilterViewModel(nameof(PersonViewModel.Weight), ComplexComparableOperator.IsBetween, PersonViewModel.AcceptableRangeWeight) },
                { nameof(PersonViewModel.Size), () => new StringFilterViewModel(nameof(PersonViewModel.Size)) },
                { nameof(PersonViewModel.ShoesSize), () => new IntegerFilterViewModel(nameof(PersonViewModel.ShoesSize), ComplexComparableOperator.IsBetween, PersonViewModel.AcceptableRangeShoesSize) }
        }, new PersonsSpeedFiltersViewModel());

        public override ISortingViewModel ProvideSorting()
            => new ExtendedSortingViewModel(
               [nameof(PersonViewModel.InverseName),
                nameof(PersonViewModel.Number),
                nameof(PersonViewModel.Gender),
                nameof(PersonViewModel.Age),
                nameof(PersonViewModel.Birthdate),
                nameof(PersonViewModel.Country),
                nameof(PersonViewModel.LicenseNumber),
                nameof(PersonViewModel.LicenseState),
                nameof(PersonViewModel.FromDate),
                nameof(PersonViewModel.Laterality),
                nameof(PersonViewModel.Height),
                nameof(PersonViewModel.Weight),
                nameof(PersonViewModel.Size),
                nameof(PersonViewModel.ShoesSize)],
               [nameof(PersonViewModel.InverseName)]);

        public override IGroupingViewModel ProvideGrouping()
            => new ExtendedGroupingViewModel(
               [nameof(PersonViewModel.Gender),
                nameof(PersonViewModel.Age),
                nameof(PersonViewModel.Country),
                nameof(PersonViewModel.Laterality),
                nameof(PersonViewModel.Size),
                nameof(PersonViewModel.ShoesSize)]);

        public override IDisplayViewModel ProvideDisplay() => new DisplayViewModel().AddMode<DisplayModeList>(true).AddMode<DisplayModeGrid>();
    }

    internal class PersonsSpeedFiltersViewModel : SpeedFiltersViewModel
    {
        public PersonsSpeedFiltersViewModel() => CompositeFilters.AddRange([NameFilter, AgeFilter, GenderFilter, CountryFilter]);

        public StringFilterViewModel NameFilter { get; } = new(nameof(PersonViewModel.InverseName));

        public IntegerFilterViewModel AgeFilter { get; } = new(nameof(PersonViewModel.Age), ComplexComparableOperator.IsBetween, PersonViewModel.AcceptableRangeAge);

        public EnumValueFilterViewModel<GenderType> GenderFilter { get; } = new(nameof(PersonViewModel.Gender)) { IsReadOnly = true };

        public CountryFilterViewModel CountryFilter { get; } = new(nameof(PersonViewModel.Country));
    }

    internal class PersonViewModel : EditableObject, IIdentifiable<Guid>
    {
        public static readonly AcceptableValueRange<int> AcceptableRangeAge = new(4, 110);
        public static readonly AcceptableValueRange<int> AcceptableRangeNumber = new(1, 99);
        public static readonly AcceptableValueRange<int> AcceptableRangeHeight = new(100, 230);
        public static readonly AcceptableValueRange<int> AcceptableRangeWeight = new(25, 130);
        public static readonly AcceptableValueRange<int> AcceptableRangeShoesSize = new(25, 50);

        public Guid Id { get; } = Guid.NewGuid();

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public DateTime? Birthdate { get; set; }

        public DateTime? FromDate { get; set; }

        public string? PlaceOfBirth { get; set; }

        public Country? Country { get; set; }

        public byte[]? Photo { get; set; }

        public GenderType Gender { get; set; } = GenderType.Male;

        public string? LicenseNumber { get; set; }

        public string? Description { get; set; }

        public string? Size { get; set; }

        public Laterality Laterality { get; set; } = Laterality.RightHander;

        public int? Height { get; set; }

        public int? Weight { get; set; }

        public int? ShoesSize { get; set; }

        public LicenseState LicenseState { get; set; }

        public bool IsMutation { get; set; }

        public int? Number { get; set; }

        public string InverseName => GetInverseName();

        public string FullName => GetFullName();

        public int? Age => GetAge();

        public string GetInverseName() => string.Join(" ", LastName, FirstName);

        public string GetFullName() => string.Join(" ", FirstName, LastName);

        public int? GetAge() => !Birthdate.HasValue ? null : Birthdate.Value.GetAge();

        public override string? ToString() => GetInverseName();

        public override bool Equals(object? obj) => obj is PersonViewModel other && Equals(Id, other.Id);

        public override int GetHashCode() => Id.GetHashCode();
    }

    public enum LicenseState
    {
        Unknown,

        Given,

        Back,

        Paid,
    }

    public enum Laterality
    {
        Unknown,

        RightHander,

        LeftHander
    }
}
