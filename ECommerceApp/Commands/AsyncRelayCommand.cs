using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ECommerceApp.Commands
{
    /// <summary>
    /// An async command that relays its functionality to delegates.
    /// Supports async/await pattern in WPF commands.
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Predicate<object> _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Gets whether the command is currently executing
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Creates a new AsyncRelayCommand that can always execute
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        public AsyncRelayCommand(Func<object, Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new AsyncRelayCommand
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        public AsyncRelayCommand(Func<object, Task> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Event raised when the ability to execute changes
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>True if this command can be executed; otherwise, false</returns>
        public bool CanExecute(object parameter)
        {
            return !IsExecuting && (_canExecute == null || _canExecute(parameter));
        }

        /// <summary>
        /// Executes the command asynchronously
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <summary>
        /// Executes the command asynchronously and returns a Task
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>A task representing the async operation</returns>
        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    IsExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }

        /// <summary>
        /// Raises the CanExecuteChanged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// A generic async command that relays its functionality to delegates.
    /// Provides type-safe command parameters with async/await support.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter</typeparam>
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Predicate<T> _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Gets whether the command is currently executing
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Creates a new AsyncRelayCommand that can always execute
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        public AsyncRelayCommand(Func<T, Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new AsyncRelayCommand
        /// </summary>
        /// <param name="execute">The async execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Event raised when the ability to execute changes
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>True if this command can be executed; otherwise, false</returns>
        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                return !IsExecuting && _canExecute == null;
            }

            return !IsExecuting && (_canExecute == null || _canExecute((T)parameter));
        }

        /// <summary>
        /// Executes the command asynchronously
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        /// <summary>
        /// Executes the command asynchronously and returns a Task
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>A task representing the async operation</returns>
        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    IsExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }

        /// <summary>
        /// Raises the CanExecuteChanged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
