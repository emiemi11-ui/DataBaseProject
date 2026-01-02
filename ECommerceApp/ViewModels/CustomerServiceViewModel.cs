using ECommerceApp.Commands;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Customer Service view.
    /// Handles support ticket management.
    /// </summary>
    public class CustomerServiceViewModel : ViewModelBase
    {
        private readonly ISupportService _supportService;
        private ObservableCollection<SupportTicket> _tickets;
        private SupportTicket _selectedTicket;
        private string _selectedStatusFilter;
        private string _errorMessage;
        private string _successMessage;
        private bool _showTicketDetails;

        public CustomerServiceViewModel()
        {
            _supportService = new SupportService();
            Tickets = new ObservableCollection<SupportTicket>();
            StatusOptions = new ObservableCollection<string> { "All", TicketStatuses.Open, TicketStatuses.InProgress, TicketStatuses.Resolved, TicketStatuses.Closed };

            AssignToMeCommand = new RelayCommand(ExecuteAssignToMe, CanExecuteAssignToMe);
            ResolveTicketCommand = new RelayCommand(ExecuteResolveTicket, CanExecuteResolveTicket);
            CloseTicketCommand = new RelayCommand(ExecuteCloseTicket, CanExecuteCloseTicket);
            FilterByStatusCommand = new RelayCommand(ExecuteFilterByStatus);
            ViewUnassignedCommand = new RelayCommand(ExecuteViewUnassigned);
            ViewMyTicketsCommand = new RelayCommand(ExecuteViewMyTickets);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            ViewDetailsCommand = new RelayCommand(ExecuteViewDetails, CanExecuteViewDetails);
            CloseDetailsCommand = new RelayCommand(ExecuteCloseDetails);
            LogoutCommand = new RelayCommand(ExecuteLogout);

            LoadData();
        }

        /// <summary>
        /// Gets the welcome message.
        /// </summary>
        public string WelcomeMessage => $"Welcome, {CurrentUserStore.Instance.CurrentUser?.Username ?? "Support Agent"}!";

        /// <summary>
        /// Gets or sets the collection of tickets.
        /// </summary>
        public ObservableCollection<SupportTicket> Tickets
        {
            get => _tickets;
            set => SetProperty(ref _tickets, value);
        }

        /// <summary>
        /// Gets or sets the selected ticket.
        /// </summary>
        public SupportTicket SelectedTicket
        {
            get => _selectedTicket;
            set => SetProperty(ref _selectedTicket, value);
        }

        /// <summary>
        /// Gets the available status options.
        /// </summary>
        public ObservableCollection<string> StatusOptions { get; }

        /// <summary>
        /// Gets or sets the selected status filter.
        /// </summary>
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set => SetProperty(ref _selectedStatusFilter, value);
        }

        /// <summary>
        /// Gets or sets whether to show ticket details.
        /// </summary>
        public bool ShowTicketDetails
        {
            get => _showTicketDetails;
            set => SetProperty(ref _showTicketDetails, value);
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Gets or sets the success message.
        /// </summary>
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        /// <summary>
        /// Gets the open tickets count.
        /// </summary>
        public int OpenTicketsCount { get; private set; }

        /// <summary>
        /// Gets the in-progress tickets count.
        /// </summary>
        public int InProgressTicketsCount { get; private set; }

        // Commands
        public ICommand AssignToMeCommand { get; }
        public ICommand ResolveTicketCommand { get; }
        public ICommand CloseTicketCommand { get; }
        public ICommand FilterByStatusCommand { get; }
        public ICommand ViewUnassignedCommand { get; }
        public ICommand ViewMyTicketsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand CloseDetailsCommand { get; }
        public ICommand LogoutCommand { get; }

        private void LoadData()
        {
            try
            {
                ClearMessages();

                var tickets = _supportService.GetAllTickets();
                Tickets = new ObservableCollection<SupportTicket>(tickets);

                UpdateCounts();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load tickets: {ex.Message}";
            }
        }

        private void UpdateCounts()
        {
            var openTickets = _supportService.GetTicketsByStatus(TicketStatuses.Open);
            var inProgressTickets = _supportService.GetTicketsByStatus(TicketStatuses.InProgress);

            OpenTicketsCount = openTickets?.Count ?? 0;
            InProgressTicketsCount = inProgressTickets?.Count ?? 0;

            OnPropertyChanged(nameof(OpenTicketsCount));
            OnPropertyChanged(nameof(InProgressTicketsCount));
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private bool CanExecuteAssignToMe(object parameter)
        {
            return SelectedTicket != null &&
                   SelectedTicket.AssignedToID == null &&
                   SelectedTicket.Status != TicketStatuses.Closed;
        }

        private void ExecuteAssignToMe(object parameter)
        {
            if (SelectedTicket == null) return;

            ClearMessages();

            try
            {
                var currentUser = CurrentUserStore.Instance.CurrentUser;
                if (currentUser == null)
                {
                    ErrorMessage = "Please log in to assign tickets.";
                    return;
                }

                if (_supportService.AssignTicket(SelectedTicket.TicketID, currentUser.UserID))
                {
                    SuccessMessage = $"Ticket #{SelectedTicket.TicketID} assigned to you.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to assign ticket.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error assigning ticket: {ex.Message}";
            }
        }

        private bool CanExecuteResolveTicket(object parameter)
        {
            return SelectedTicket != null &&
                   SelectedTicket.Status != TicketStatuses.Resolved &&
                   SelectedTicket.Status != TicketStatuses.Closed;
        }

        private void ExecuteResolveTicket(object parameter)
        {
            if (SelectedTicket == null) return;

            ClearMessages();

            try
            {
                if (_supportService.ResolveTicket(SelectedTicket.TicketID))
                {
                    SuccessMessage = $"Ticket #{SelectedTicket.TicketID} marked as resolved.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to resolve ticket.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error resolving ticket: {ex.Message}";
            }
        }

        private bool CanExecuteCloseTicket(object parameter)
        {
            return SelectedTicket != null &&
                   SelectedTicket.Status != TicketStatuses.Closed;
        }

        private void ExecuteCloseTicket(object parameter)
        {
            if (SelectedTicket == null) return;

            ClearMessages();

            try
            {
                if (_supportService.CloseTicket(SelectedTicket.TicketID))
                {
                    SuccessMessage = $"Ticket #{SelectedTicket.TicketID} closed.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to close ticket.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error closing ticket: {ex.Message}";
            }
        }

        private void ExecuteFilterByStatus(object parameter)
        {
            ClearMessages();

            try
            {
                if (string.IsNullOrEmpty(SelectedStatusFilter) || SelectedStatusFilter == "All")
                {
                    LoadData();
                }
                else
                {
                    var filtered = _supportService.GetTicketsByStatus(SelectedStatusFilter);
                    Tickets = new ObservableCollection<SupportTicket>(filtered);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Filter failed: {ex.Message}";
            }
        }

        private void ExecuteViewUnassigned(object parameter)
        {
            ClearMessages();

            try
            {
                var unassigned = _supportService.GetUnassignedTickets();
                Tickets = new ObservableCollection<SupportTicket>(unassigned);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load unassigned tickets: {ex.Message}";
            }
        }

        private void ExecuteViewMyTickets(object parameter)
        {
            ClearMessages();

            try
            {
                var currentUser = CurrentUserStore.Instance.CurrentUser;
                if (currentUser == null)
                {
                    ErrorMessage = "Please log in.";
                    return;
                }

                var myTickets = _supportService.GetTicketsByAssignee(currentUser.UserID);
                Tickets = new ObservableCollection<SupportTicket>(myTickets);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load your tickets: {ex.Message}";
            }
        }

        private void ExecuteRefresh(object parameter)
        {
            SelectedStatusFilter = null;
            LoadData();
        }

        private bool CanExecuteViewDetails(object parameter) => SelectedTicket != null;

        private void ExecuteViewDetails(object parameter)
        {
            ShowTicketDetails = true;
        }

        private void ExecuteCloseDetails(object parameter)
        {
            ShowTicketDetails = false;
        }

        private void ExecuteLogout(object parameter)
        {
            CurrentUserStore.Instance.Logout();
            NavigationStore.Instance.Navigate(new LoginViewModel());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_supportService is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
