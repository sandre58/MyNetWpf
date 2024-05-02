// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using MyNet.UI.Commands;
using MyNet.UI.Resources;
using MyNet.UI.Toasting;
using MyNet.Utilities;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Logging;
using MyNet.Utilities.Resources;

namespace MyNet.Wpf.Commands
{
    public static class AppCommands
    {
        public static ICommand CopyInClipboardCommand { get; } = CommandsManager.Create<object>(x =>
        {
            try
            {
                Clipboard.SetDataObject(x?.ToString());
                ToasterManager.ShowInformation(MessageResources.CopyInClipBoardSuccess);
            }
            catch (Exception)
            {
                ToasterManager.ShowError(MessageResources.CopyInClipBoardError);
            }
        });

        public static ICommand OpenFileCommand { get; } = CommandsManager.CreateNotNull<string>(x =>
        {
            try
            {
                ProcessHelper.Start(x);
            }
            catch (Exception e)
            {
                ToasterManager.ShowError(MessageResources.FileXOpenError.FormatWith(x));
                LogManager.Error(e);
            }
        });
    }
}
