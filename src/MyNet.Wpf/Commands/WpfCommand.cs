﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using MyNet.Observable.Attributes;
using MyNet.UI.Commands;
using MyNet.Utilities.Messaging;

namespace MyNet.Wpf.Commands
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'.  This class does not allow you to accept command parameters in the
    /// Execute and CanExecute callback methods.
    /// </summary>
    [CanBeValidated(false)]
    [CanNotify(false)]
    [CanSetIsModified(false)]
    public class WpfCommand : RelayCommand
    {
        private readonly WeakAction _execute;

        private readonly WeakFunc<bool>? _canExecute;

        /// <summary>
        /// Initializes a new instance of the WpfCommand class that 
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public WpfCommand(Action execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WpfCommand class.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closures.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public WpfCommand(Action execute, Func<bool>? canExecute, bool keepTargetAlive = false) : base(execute, canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = new WeakAction(execute, keepTargetAlive);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
            }
        }

        private EventHandler? _requerySuggestedLocal;

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public override event EventHandler? CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    // add event handler to local handler backing field in a thread safe manner
                    EventHandler? handler2;
                    var canExecuteChanged = _requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        var handler3 = (EventHandler?)Delegate.Combine(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange(
                            ref _requerySuggestedLocal,
                            handler3,
                            handler2);
                    }
                    while (canExecuteChanged != handler2);

                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    // removes an event handler from local backing field in a thread safe manner
                    EventHandler? handler2;
                    var canExecuteChanged = _requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        var handler3 = (EventHandler?)Delegate.Remove(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange(
                            ref _requerySuggestedLocal,
                            handler3,
                            handler2);
                    }
                    while (canExecuteChanged != handler2);

                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public override void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public override bool CanExecute(object? parameter) => _canExecute == null
                || (_canExecute.IsStatic || _canExecute.IsAlive)
                    && _canExecute.Execute();

        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public override void Execute(object? parameter)
        {
            if (CanExecute(parameter)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }

    /// <summary>
    /// A generic command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'. This class allows you to accept command parameters in the
    /// Execute and CanExecute callback methods.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    [CanBeValidated(false)]
    [CanNotify(false)]
    [CanSetIsModified(false)]
    public class WpfCommand<T> : RelayCommand<T>
    {
        private readonly WeakAction<T?> _execute;

        private readonly WeakFunc<T?, bool>? _canExecute;

        /// <summary>
        /// Initializes a new instance of the WpfCommand class that 
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public WpfCommand(Action<T?> execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WpfCommand class.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public WpfCommand(Action<T?> execute, Func<T?, bool>? canExecute, bool keepTargetAlive = false) : base(execute, canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = new WeakAction<T?>(execute, keepTargetAlive);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T?, bool>(canExecute, keepTargetAlive);
            }
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public override event EventHandler? CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public override void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data 
        /// to be passed, this object can be set to a null reference</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public override bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            if (_canExecute.IsStatic || _canExecute.IsAlive)
            {
                if (parameter == null
                    && typeof(T).IsValueType)
                {
                    return _canExecute.Execute(default);
                }

                if (parameter is null or T)
                {
                    return _canExecute.Execute((T?)parameter);
                }
            }

            return false;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data 
        /// to be passed, this object can be set to a null reference</param>
        public override void Execute(object? parameter)
        {
            var val = parameter;

            if (CanExecute(val)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                if (val == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        _execute.Execute(default);
                    }
                    else
                    {
                        _execute.Execute((T?)val);
                    }
                }
                else
                {
                    _execute.Execute((T)val);
                }
            }
        }
    }
}
