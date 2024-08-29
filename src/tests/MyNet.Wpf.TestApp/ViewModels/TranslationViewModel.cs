// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using MyNet.Observable;
using MyNet.Observable.Attributes;
using MyNet.Observable.Translatables;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Shell;
using MyNet.Utilities;
using MyNet.Utilities.Localization;
using MyNet.Wpf.TestApp.Resources;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class TranslationViewModel : TimeAndLanguageViewModel
    {
        protected override string CreateTitle() => TestAppResources.Translation;

        public ObservableCollection<TranslatableString> UiResources { get; } = [];
        public ObservableCollection<TranslatableString> FormatResources { get; } = [];
        public ObservableCollection<TranslatableString> MessageResources { get; } = [];

        public DateTime LocalDateTime { get; set; } = DateTime.Now;

        public DateTime UtcDateTime { get; set; } = DateTime.UtcNow;

        public DateTime CurrentDateTime { get; set; } = GlobalizationService.Current.Convert(DateTime.UtcNow);

        public EditableDateTime LocalDate { get; set; } = new();

        public EditableDateTime UtcDate { get; set; } = new();

        public EditableDateTime CurrentDate { get; set; } = new();

        public ICommand SystemLanguageCommand { get; private set; }

        public ICommand LocalTimeZoneCommand { get; private set; }

        [UpdateOnCultureChanged]
        public virtual CultureInfo Culture => GlobalizationService.Current.Culture;

        public TranslationViewModel()
        {
            UiResources.AddRange(GetResources(UI.Resources.UiResources.ResourceManager));
            MessageResources.AddRange(GetResources(UI.Resources.MessageResources.ResourceManager));
            FormatResources.AddRange(GetResources(UI.Resources.FormatResources.ResourceManager));

            SystemLanguageCommand = CommandsManager.Create(() => GlobalizationService.Current.SetCulture(CultureInfo.InstalledUICulture));
            LocalTimeZoneCommand = CommandsManager.Create(() => GlobalizationService.Current.SetTimeZone(TimeZoneInfo.Local));
        }

        private static List<TranslatableString> GetResources(ResourceManager resourceManager)
        {
            var resourceSet = resourceManager.GetResourceSet(GlobalizationService.Current.Culture, true, true);

            if (resourceSet is not null)
            {
                var result = new List<TranslatableString>();
                foreach (DictionaryEntry entry in resourceSet)
                {
                    result.Add(new TranslatableString(entry.Key.ToString().OrEmpty()));
                }

                return [.. result.OrderBy(x => x.Key)];
            }

            return [];
        }
    }
}
