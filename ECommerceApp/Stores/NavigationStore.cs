using ECommerceApp.ViewModels;
using System;

namespace ECommerceApp.Stores
{
    /// <summary>
    /// Stores the current ViewModel for navigation purposes.
    /// Enables view switching without coupling ViewModels to Views.
    /// </summary>
    public class NavigationStore
    {
        private static NavigationStore _instance;
        private static readonly object _lock = new object();

        private ViewModelBase _currentViewModel;

        /// <summary>
        /// Gets the singleton instance of the NavigationStore.
        /// </summary>
        public static NavigationStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new NavigationStore();
                        }
                    }
                }
                return _instance;
            }
        }

        private NavigationStore() { }

        /// <summary>
        /// Gets or sets the current ViewModel.
        /// Setting this value raises the CurrentViewModelChanged event.
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                // Dispose the old ViewModel if it implements IDisposable
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event raised when the current ViewModel changes.
        /// </summary>
        public event Action CurrentViewModelChanged;

        /// <summary>
        /// Navigates to the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">Type of ViewModel to navigate to</typeparam>
        /// <param name="viewModel">The ViewModel instance</param>
        public void Navigate<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
        {
            CurrentViewModel = viewModel;
        }
    }
}
