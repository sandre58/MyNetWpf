// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Wpf.TestApp.Resources;
using MyNet.UI.Commands;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities.Helpers;
using MyNet.Wpf.Controls;
using Wpf.Ui.Controls;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class IconsViewModel : NavigableWorkspaceViewModel
    {
        public ICommand OpenMaterialDesignUrlCommand { get; }

        public IconsListViewModel PackIconsListViewModel { get; } = new(PackIconGroups);

        public IconsListViewModel SymbolIconsListViewModel { get; } = new(SymbolIconGroups);

        public IconsViewModel() => OpenMaterialDesignUrlCommand = CommandsManager.Create(OpenMaterialDesignUrl);

        protected override string CreateTitle() => TestAppResources.Icons;

        private void OpenMaterialDesignUrl() => ProcessHelper.Start("https://materialdesignicons.com/");

        private static readonly IList<IconData> PackIconGroups = Enum.GetNames<MaterialDesignThemes.Wpf.PackIconKind>()
                    .GroupBy(k => (MaterialDesignThemes.Wpf.PackIconKind)Enum.Parse(typeof(MaterialDesignThemes.Wpf.PackIconKind), k))
                    .Select(g =>
                    {
                        PackIcon buildIcon() => new() { Kind = g.Key };

                        return new IconData((Func<PackIcon>)buildIcon, g.Key.ToString(), "<my:PackIcon Kind=\"" + g.Key + "\"/>", [.. g.OrderBy(x => x)], buildIcon().Data);
                    })
                    .ToList();

        private static readonly IList<IconData> SymbolIconGroups = Enum.GetNames<SymbolRegular>()
                    .GroupBy(k => (SymbolRegular)Enum.Parse(typeof(SymbolRegular), k))
                    .Select(g =>
                    {
                        Controls.SymbolIcon buildIcon() => new() { Symbol = g.Key };

                        return new IconData((Func<Controls.SymbolIcon>)buildIcon, g.Key.ToString(), "<my:SymbolIcon Symbol=\"" + g.Key + "\"/>", [.. g.OrderBy(x => x)], "\\u" + ((int)g.Key).ToString("X4"));
                    })
                    .ToList();
    }
}
