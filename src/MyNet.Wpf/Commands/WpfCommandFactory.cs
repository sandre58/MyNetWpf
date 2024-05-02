// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using MyNet.UI.Commands;

namespace MyNet.Wpf.Commands
{
    public class WpfCommandFactory : ICommandFactory
    {
        public ICommand Create(Action execute) => new WpfCommand(execute);

        public ICommand Create(Action execute, Func<bool> canExectute) => new WpfCommand(execute, canExectute);

        public ICommand Create<T>(Action<T?> execute) => new WpfCommand<T>(execute);

        public ICommand Create<T>(Action<T?> execute, Func<T?, bool> canExectute) => new WpfCommand<T>(execute, canExectute);
    }
}
